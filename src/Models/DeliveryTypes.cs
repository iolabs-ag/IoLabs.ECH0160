namespace IoLabs.ECH0160.Models
{
    /// <summary>Defines the types of deliveries for archival purposes according to eCH-0160 standards.</summary>
    public class DeliveryTypes
    {
        /// <summary>GEVER_TYPE represents deliveries originating from a business administration system (GEVER system).</summary>
        public const string GeverType = "GEVER";

        /// <summary>FILES_TYPE represents deliveries coming from file repositories, databases, or other systems not classified as GEVER.</summary>
        public const string FilesType = "FILES";
    }
}