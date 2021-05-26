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
        public CustomerType CustomerTypeId { get; set; }

        [Column("RoleId")]
        public Role RoleId { get; set; }

        public string VerificationToken { get; set; }

        public DateTime? Verified { get; set; }

        public bool IsVerified => Verified.HasValue || PwdReset.HasValue;

        public string ResetToken { get; set; }

        public DateTime? ResetTokenExpires { get; set; }

        public DateTime? PwdReset { get; set; }

        public int HouseNumber { get; set; }

        public AddressRepetitionIndex AddressRepetitionIndexId { get; set; }

        public string Street { get; set; }

        public string StreetComplement { get; set; }

        public string ZipCode { get; set; }

        public int CountryId { get; set; }

        public byte[] IdCard { get; set; }

        public byte[] ProofOfAddress { get; set; }

        public string Iban { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; }

        public bool OwnsToken(string token) 
        {
            return RefreshTokens?.Find(x => x.Token == token) != null;
        }
    }
}