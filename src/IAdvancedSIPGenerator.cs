using IoLabs.ECH0160.Models;

namespace IoLabs.ECH0160
{
    /// <summary>Generates a SIP package structure as per the eCH-0160 standard.</summary>
    public interface IAdvancedSIPGenerator
    {
        /// <summary>Creates a SIP package for the specified archive delivery.</summary>
        /// <param name="archivalDetails">Details of the archive delivery</param>
        /// <param name="temporaryProcessID">Project specific Id</param>
        /// <param name="sysInformation">System position for XML Nodes and it's depth related to it</param>
        /// <param name="baseDirectory">Base directory for the SIP package</param>
        /// <param name="convertedFilesDirectory">Directory for the converted files</param>
        /// <param name="onRecordConversionPassed">Action to be executed after the conversion of a record is done</param>
        /// <param name="onRecordNameNormalizationDone">Action to be executed after the name normalization of a record is done</param>
        /// <returns>Sip Package</returns>
        public Task<SipPackage> CreateSIPPackage(ArchiveDeliveryDetails archivalDetails, Guid temporaryProcessID, OrdnungSystem sysInformation, string baseDirectory, string convertedFilesDirectory,
                                                 Func<ArchivalRecordDocument, ArchivalRecordInput, Task> onRecordConversionPassed = null, Func<ArchivalRecordDocument, Task> onRecordNameNormalizationDone = null);
    }
}