using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pb.Api.Entities
{
    public class Account
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string PwdHash { get; set; }

        [Column("CustomerTypeId")]
        public CustomerType CustomerType { get; set; }

        [Column("RoleId")]
        public Role Role { get; set; }

        public string VerificationToken { get; set; }

        public DateTime? Verified { get; set; }

        public bool IsVerified => Verified.HasValue || PwdReset.HasValue;

        public string ResetToken { get; set; }

        public DateTime? ResetTokenExpires { get; set; }

        public DateTime? PwdReset { get; set; }

        public bool AcceptTerms { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; }

        public bool OwnsToken(string token) 
        {
            return RefreshTokens?.Find(x => x.Token == token) != null;
        }
    }
}