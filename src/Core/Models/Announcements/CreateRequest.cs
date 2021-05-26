using Pb.Api.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pb.Api.Models.Announcements
{
    public class CreateRequest
    {
        [Required]
        public Account Account { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int HouseNumber { get; set; }

        [Required]
        public AddressRepetitionIndex AddressRepetitionIndexId { get; set; }

        [Required]
        public string Street { get; set; }

        public string StreetComplement { get; set; }

        [Required]
        public string ZipCode { get; set; }

        [Required]
        public string CountryIsoCode { get; set; }

        [Required]
        public int MinCapacity { get; set; }

        [Required]
        public int MaxCapacity { get; set; }

        public List<Equipment> EquipmentIds { get; set; }

        [Required]
        public Presence Present { get; set; }

        public bool CanChildren { get; set; }

        public bool CanEvents { get; set; }

        public bool CanAlcohol { get; set; }

        public bool CanSmoke { get; set; }

        public bool CanPets { get; set; }

        public bool CanMusic { get; set; }

        public string Comments { get; set; }

        [Required]
        public decimal HalfDayUnitPrice { get; set; }

        [Required]
        [MinLength(1)]
        public List<LegalSafety> LegalSafeties { get; set; }
    }
}