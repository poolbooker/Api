using System.ComponentModel.DataAnnotations;
using Pb.Api.Entities;

namespace Pb.Api.Models.Accounts
{
    public class CreateRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public Role Role { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public CustomerType CustomerType { get; set; }

        public int HouseNumber { get; set; }

        public AddressRepetitionIndex AddressRepetitionIndexId { get; set; }

        [Required]
        public string Street { get; set; }

        public string StreetComplement { get; set; }

        [Required]
        [StringLength(5)]
        public string ZipCode { get; set; }

        [Required]
        public string CountryIsoCode { get; set; }

        public byte[] IdCard { get; set; }

        public byte[] ProofOfAddress { get; set; }

        [MaxLength(34)]
        public string Iban { get; set; }
    }
}