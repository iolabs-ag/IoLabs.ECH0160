using IoLabs.ECH0160.Models;

namespace IoLabs.ECH0160.Conversions
{
    /// <summary>Defines the contract for file converters to the eCH-0160 standard.</summary>
    public interface IFileConverter
    {
        /// <summary>Checks if the file is supported by the converter. Consideration: might evolve to provide more detailed support information beyond a boolean.</summary>
        /// <param name="recordInput">The archival record to check for support</param>
        /// <returns>True if the file is supported, False otherwise</returns>
        public bool IsFileSupported(ArchivalRecordInput recordInput);

        /// <summary>Gets the priority of the converter to determine the order of application. Higher values indicate higher priority.</summary>
        public int Priority { get; }

        /// <summary>Identifier of the Converter.</summary>
        public string Identifier { get; }

        /// <summary>Converts an archival record to the eCH-0160 standard.</summary>
        /// <param name="recordInput">The archival record to convert</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the converted ArchivalRecordDocument</returns>
        public Task<ArchivalRecordDocument> ConvertFile(ArchivalRecordInput recordInput);
    }
}