using BC = BCrypt.Net.BCrypt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Pb.Api.Entities;
using Pb.Api.Helpers;
using Pb.Api.Models.Accounts;
using AutoMapper;
using System.IO;
using Pb.Api.Interfaces;

namespace Pb.Api.Services
{
    public class AccountService : IAccountService
    {
        private readonly PoolBookerDbContext _context;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IEmailService _emailService;

        public AccountService(
            PoolBookerDbContext context,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _emailService = emailService;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

            if (account == null || !account.IsVerified || !BC.Verify(model.Password, account.PwdHash))
                throw new AppException("Email or password is incorrect");

            // Authentication successful => Generate jwt and refresh tokens
            var jwtToken = GenerateJwtToken(account);
            var refreshToken = GenerateRefreshToken(ipAddress);
            account.RefreshTokens.Add(refreshToken);

            // Remove old refresh tokens from account
            RemoveOldRefreshTokens(account);

            // Save changes to db
            _context.Update(account);
            _context.SaveChanges();

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = refreshToken.Token;
            return response;
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var (refreshToken, account) = GetRefreshToken(token);

            // Replace old refresh token with a new one and save
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            account.RefreshTokens.Add(newRefreshToken);

            RemoveOldRefreshTokens(account);

            _context.Update(account);
            _context.SaveChanges();

            // Generate new jwt
            var jwtToken = GenerateJwtToken(account);

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = newRefreshToken.Token;
            return response;
        }

        public void RevokeToken(string token, string ipAddress)
        {
            var (refreshToken, account) = GetRefreshToken(token);

            // Revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _context.Update(account);
            _context.SaveChanges();
        }

        public bool Register(RegisterRequest model, string target)
        {
            if (_context.Accounts.Any(x => x.Email == model.Email))
                return false;

            var account = _mapper.Map<Account>(model);

            // First registered account is an admin
            var isFirstAccount = _context.Accounts.Count() == 0;
            account.RoleId = isFirstAccount ? Role.Admin : Role.User;
            account.CountryId = _context.Countries.First(c => c.IsoCode == model.CountryIsoCode).Id;
            account.Created = DateTime.UtcNow;
            account.VerificationToken = RandomTokenString();

            // Hash password
            account.PwdHash = BC.HashPassword(model.Password);

            // Save account
            _context.Accounts.Add(account);
            _context.SaveChanges();

            // Send verification email
            SendVerificationEmail(account, $"https://{target}");
            return true;
        }

        public void VerifyEmail(string token)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.VerificationToken == token);

            if (account == null) throw new AppException("Verification failed");

            account.Verified = DateTime.UtcNow;
            account.VerificationToken = null;

            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public void ForgotPassword(ForgotPasswordRequest model, string target)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

            // Always return ok response to prevent email enumeration
            if (account == null)
                return;

            // Create reset token that expires after 1 day
            account.ResetToken = RandomTokenString();
            account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

            _context.Accounts.Update(account);
            _context.SaveChanges();

            // Send email
            SendPasswordResetEmail(account, $"https://{target}");
        }

        public void ValidateResetToken(ValidateResetTokenRequest model)
        {
            var account = _context.Accounts.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (account == null)
                throw new AppException("Invalid token");
        }

        public void ResetPassword(string token, string password, string confirmPassword)
        {
            var account = _context.Accounts.SingleOrDefault(x =>
                x.ResetToken == token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (account == null)
                throw new AppException("Invalid token");

            // Update password and remove reset token
            account.PwdHash = BC.HashPassword(password);
            account.PwdReset = DateTime.UtcNow;
            account.ResetToken = null;
            account.ResetTokenExpires = null;

            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public IEnumerable<AccountResponse> GetAll()
        {
            var accounts = _context.Accounts;
            return _mapper.Map<IList<AccountResponse>>(accounts);
        }

        public AccountResponse GetById(int id)
        {
            var account = GetAccount(id);
            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse Create(CreateRequest model)
        {
            if (_context.Accounts.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already registered");

            var account = _mapper.Map<Account>(model);
            account.Created = DateTime.UtcNow;
            account.Verified = DateTime.UtcNow;

            // Hash password
            account.PwdHash = BC.HashPassword(model.Password);

            // Save account
            _context.Accounts.Add(account);
            _context.SaveChanges();

            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse Update(int id, UpdateRequest model)
        {
            var account = GetAccount(id);

            if (account.Email != model.Email && _context.Accounts.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already taken");

            // Hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                account.PwdHash = BC.HashPassword(model.Password);

            // Copy model to account and save
            _mapper.Map(model, account);
            account.Updated = DateTime.UtcNow;
            _context.Accounts.Update(account);
            _context.SaveChanges();

            return _mapper.Map<AccountResponse>(account);
        }

        public void Delete(int id)
        {
            var account = GetAccount(id);
            _context.Accounts.Remove(account);
            _context.SaveChanges();
        }

        private Account GetAccount(int id)
        {
            var account = _context.Accounts.Find(id);
            if (account == null)
                throw new KeyNotFoundException("Account not found");
            return account;
        }

        private (RefreshToken, Account) GetRefreshToken(string token)
        {
            var account = _context.Accounts.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            if (account == null)
                throw new AppException("Invalid token");
            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive)
                throw new AppException("Invalid token");
            return (refreshToken, account);
        }

        private string GenerateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private void RemoveOldRefreshTokens(Account account)
        {
            account.RefreshTokens.RemoveAll(x => 
                !x.IsActive && 
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            
            // Convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private void SendVerificationEmail(Account account, string target)
        {
            string message;
            if (!string.IsNullOrEmpty(target))
            {
                using (var reader = new StreamReader(Path.Combine(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Templates"), "VerificationEmail.html")))
                {
                    message = reader.ReadToEnd()
                        .Replace("{User}", account.FirstName)
                        .Replace("{BannerImageSrc}", $"{target}/app-images/LogoBanner.png")
                        .Replace("{VerificationUrl}", $"{target}/accounts/verify-email?token={account.VerificationToken}")
                        .Replace("{SecurityUrl}", "https://www.poolbooker.com/securite");
                }
            }
            else
            {
                message = $@"<p>Merci d'utiliser le jeton suivant pour vérifier votre adresse email address avec le <code>/accounts/verify-email</code> api route:</p>
                             <p><code>{account.VerificationToken}</code></p>";
            }

            _emailService.Send(
                to: account.Email,
                subject: "Vérification de votre adresse email",
                //html: $@"<h4>Verifiez votre adresse Email</h4>
                //         <p>Merci de votre inscription!</p>
                //         {message}",
                html: $@"{message}"
            );
        }

        private void SendPasswordResetEmail(Account account, string target)
        {
            string message;
            if (!string.IsNullOrEmpty(target))
            {
                var resetUrl = $"{target}/reset-password?token={account.ResetToken}&password=toto";
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                             <p><code>{account.ResetToken}</code></p>";
            }

            _emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                         {message}"
            );
        }
    }
}
