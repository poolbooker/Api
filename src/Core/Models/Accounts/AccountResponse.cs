using Pb.Api.Entities;
using System;

namespace Pb.Api.Models.Accounts
{
    public class AccountResponse
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public CustomerType CustomerType { get; set; }

        public Role Role { get; set; }

        public int HouseNumber { get; set; }

        public AddressRepetitionIndex AddressRepetitionIndexId { get; set; }

        public string Street { get; set; }

        public string StreetComplement { get; set; }

        public string ZipCode { get; set; }

        public int CountryId { get; set; }

        public string Iban { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public bool IsVerified { get; set; }
    }
}