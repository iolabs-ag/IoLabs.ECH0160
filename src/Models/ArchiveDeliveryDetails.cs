namespace IoLabs.ECH0160.Models
{
    /// <summary>ArchiveDeliveryDetails encapsulates various properties related to the delivery, protection, and archiving of records.</summary>
    public class ArchiveDeliveryDetails
    {
        /// <summary>The agency responsible for delivering the records.</summary>
        public string DeliveringAgency { get; set; }

        /// <summary>The agency responsible for delivering the records.</summary>
        public string DeliveringAgencyShort { get; set; }

        /// <summary>The category of protection applied to the records.</summary>
        public string ProtectionCategory { get; set; }

        /// <summary>The period during which the records are to be protected.</summary>
        public int ProtectionPeriod { get; set; }

        /// <summary>The name of the creator of the records.</summary>
        public string RecordCreatorName { get; set; }

        /// <summary>The name of the system where the records were created or are managed.</summary>
        public string SystemName { get; set; }

        /// <summary>Authority name.</summary>
        public string Reference { get; set; }

        /// <summary>Form Appearance for xml generation.</summary>
        public string FormAppearance { get; set; }

        /// <summary>Registratur: Fixer Wert für alle SIP.</summary>
        public string Registry { get; set; }

        /// <summary>Delivery type.</summary>
        public string DeliveryType { get; set; } = DeliveryTypes.FilesType; // TODO: use enums
    }
}