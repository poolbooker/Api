using System;
using System.Collections.Generic;

namespace Pb.Api.Entities
{
    public class Announcement
    {
        public int Id { get; set; }

        public Account Account { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int HouseNumber { get; set; }

        public AddressRepetitionIndex AddressRepetitionIndexId { get; set; }

        public string Street { get; set; }

        public string StreetComplement { get; set; }

        public string ZipCode { get; set; }

        public int CountryId { get; set; }

        public int MinCapacity { get; set; }

        public int MaxCapacity { get; set; }

        public List<Equipment> EquipmentIds { get; set; }

        public Presence Present { get; set; }

        public bool CanChildren { get; set; }

        public bool CanEvents { get; set; }

        public bool CanAlcohol { get; set; }

        public bool CanSmoke { get; set; }

        public bool CanPets { get; set; }

        public bool CanMusic { get; set; }

        public string Comments { get; set; }

        public decimal HalfDayUnitPrice { get; set; }

        public List<LegalSafety> LegalSafeties { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }
    }
}