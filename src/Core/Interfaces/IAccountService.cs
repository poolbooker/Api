using System.Collections.Generic;
using Pb.Api.Models.Accounts;

namespace Pb.Api.Interfaces
{
    public interface IAccountService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);

        AuthenticateResponse RefreshToken(string token, string ipAddress);

        void RevokeToken(string token, string ipAddress);

        bool Register(RegisterRequest model, string target);

        void VerifyEmail(string token);

        void ForgotPassword(ForgotPasswordRequest model, string target);

        void ValidateResetToken(ValidateResetTokenRequest model);

        void ResetPassword(string token, string password, string confirmPassword);

        IEnumerable<AccountResponse> GetAll();

        AccountResponse GetById(int id);

        AccountResponse Create(CreateRequest model);

        AccountResponse Update(int id, UpdateRequest model);

        void Delete(int id);
    }
}
