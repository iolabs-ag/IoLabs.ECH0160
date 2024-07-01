using IoLabs.ECH0160.Models;
using Microsoft.Extensions.Logging;

namespace IoLabs.ECH0160.Metadata
{
    /// <summary>Defines the contract for generating metadata and managing the structure of a SIP package.</summary>
    public interface IMetadataGenerator
    {
        /// <summary>Generates metadata XML for files in the SIP package.</summary>
        /// <param name="headerArchivalRecords">List of header files for metadata generation</param>
        /// <param name="archivalDetails">Archival records from the institutional entity</param>
        /// <param name="structureValues">Paths for saving the metadata XML</param>
        /// <param name="sysInformation">System position for XML Nodes and it's depth related to it</param>
        public void CreateXmlMetadataFile(IList<ArchivalRecordInput> headerArchivalRecords, ArchiveDeliveryDetails archivalDetails, Dictionary<string, string> structureValues, OrdnungSystem sysInformation);

        /// <summary>Creates the SIP package directory structure and initial setup.</summary>
        /// <param name="baseDirectory">Base directory for the SIP package</param>
        /// <param name="processID">Project specific Id</param>
        /// <param name="deliveryType">Type of delivery for the SIP package</param>
        /// <returns>Dictionary with key paths for base, header, xsd, and content directories</returns>
        public Dictionary<string, string> GetSIPPackageMetadataXMLStructure(Guid processID, string baseDirectory, string deliveryType);

        /// <summary>Copies XSD files to a target directory asynchronously.</summary>
        /// <param name="targetDirectory">Target directory for the XSD files</param>
        /// <param name="logger">Logger for capturing log messages (optional)</param>
        /// <returns>A Task representing the asynchronous operation</returns>
        public Task AddXsdSwissStandardFilesAsync(string targetDirectory, ILogger logger = null);

        /// <summary>Retrieves header files for the SIP Package.</summary>
        /// <param name="xmlStructure">XML structure for the header files</param>
        /// <returns>List of archival records representing header files</returns>
        public Task<IList<ArchivalRecordInput>> GetArchivalRecordFilesHeaderAsync(Dictionary<string, string> xmlStructure);
    }
}