using IoLabs.ECH0160.Models;

namespace IoLabs.ECH0160.Conversions
{
    /// <summary>Defines the contract for managing the conversion of files to the eCH-0160 standard.</summary>
    public interface IFilesConversionManager
    {
        /// <summary>Converts files to the eCH-0160 standard.</summary>
        /// <param name="recordInput">The archival record to convert</param>
        Task<ArchivalRecordDocument> ConvertFile(ArchivalRecordInput recordInput);

        /// <summary>Converts directories structure to the eCH-0160 standard.</summary>
        /// <param name="directory">The archival record directory to convert</param>
        /// <returns>The archival record document</returns>
        Task<ArchivalRecordDocument> ConvertDirectory(Dossier directory);
    }
}