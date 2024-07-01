namespace IoLabs.ECH0160.Models
{
    /// <summary>Model describes the SIP archive.</summary>
    public class SipPackage
    {
        /// <summary>Gets or sets the base directory for the SIP archive.</summary>
        public string BaseDirectory { get; set; }

        /// <summary>Gets or sets the list of archival records included in the SIP archive.</summary>
        public IList<ArchivalRecordInput> ArchRecords { get; set; }

        /// <summary>Gets or sets the details about the archive's delivery.</summary>
        public ArchiveDeliveryDetails DeliveryDetails { get; set; }

        /// <summary>Gets or sets the XML metadata structure associated with the SIP archive.</summary>
        public Dictionary<string, string> XmlMetaDataStructure { get; set; }

        /// <summary>Name of the SIP.</summary>
        public string SipPackageName { get; set; }

        /// <summary>Path to the SIP package result.</summary>
        public string SipPackagePath { get; set; }
    }
}