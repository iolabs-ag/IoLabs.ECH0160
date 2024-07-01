namespace IoLabs.ECH0160.Models
{
    /// <summary>
    /// Represents the structure of a Submission Information Package (SIP) according to eCH-0160 standard.
    /// This class provides constants for the names of the folders used within the SIP for storing documentation
    /// and data, especially for FILES-SIPs with integrated documentation such as archived relational databases.
    /// </summary>
    public class SipStructure
    {
        /// <summary>The documentation in a FILES-SIP with integrated documentation must be stored in this folder.</summary>
        public const string DocuType = "1_DOK";

        /// <summary>The data in a FILES-SIP with integrated documentation (e.g., the data of an archived relational database) must be stored in this folder.</summary>
        public const string DataType = "2_DATEN";
    }
}