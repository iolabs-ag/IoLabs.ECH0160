using IoLabs.ECH0160.Models;

namespace IoLabs.ECH0160.Conversions
{
    /// <summary>Represents an abstract base for file converters that operate based on file extensions.</summary>
    public abstract class ExtensionBasedFileConverter : IFileConverter
    {
        /// <summary>Gets a list of supported file extensions.</summary>
        protected abstract List<string> SupportedExtensions { get; }

        /// <summary>Determines if a file is supported by checking its extension against supported extensions.</summary>
        /// <param name="recordInput">The archival record to check</param>
        /// <returns>True if the file is supported, otherwise false</returns>
        public bool IsFileSupported(ArchivalRecordInput recordInput)
        {
            string extension = Path.GetExtension(recordInput.OriginalName)?.ToLower().TrimStart('.');

            return extension != null && SupportedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>Identifier of the Converter.</summary>
        public abstract string Identifier { get; }

        /// <summary>Gets the priority of the file converter.</summary>
        public abstract int Priority { get; }

        /// <summary>Converts the file associated with the provided archival record.</summary>
        /// <param name="recordInput">The archival record whose file is to be converted</param>
        /// <returns>A task that represents the asynchronous operation, with a result of the converted ArchivalRecordDocument</returns>
        public abstract Task<ArchivalRecordDocument> ConvertFile(ArchivalRecordInput recordInput);
    }
}