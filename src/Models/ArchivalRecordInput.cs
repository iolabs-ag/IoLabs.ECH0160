using System.ComponentModel.DataAnnotations.Schema;

namespace IoLabs.ECH0160.Models
{
    /// <summary>Represents a file that has been downloaded as part of the archivation process.</summary>
    public class ArchivalRecordInput
    {
        /// <summary>Gets or sets the unique identifier for the file.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Gets or sets the name of the file before conversion.</summary>
        public string OriginalName { get; set; }

        /// <summary>Full path to the source file.</summary>
        public string FullPathWithFileName { get; set; }

        /// <summary>Gets or sets the conversion file path.</summary>
        public string DirectoryConversionPath { get; set; }

        /// <summary>Relative path to the folder which contains the file. Based on this path, the file will be stored in the archive.</summary>
        public string DirectoryRelativePath { get; set; }

        /// <summary>Gets or sets the name of the file hash.</summary>
        public string Hash { get; set; }

        /// <summary>Gets or sets the algorithm used for file integrity check.</summary>
        public string HashAlgorithm { get; set; }

        /// <summary>Gets or sets the size of the downloaded file.</summary>
        public long Size { get; set; }

        /// <summary>Indicates whether the file has been converted successfully.</summary>
        public bool IsConvertedOrSkipped { get; set; }

        /// <summary>Gets or sets the date and time when the file was downloaded.</summary>
        public DateTime DownloadedAt { get; set; }

        /// <summary>The last modification of the file.</summary>
        public DateTime LastUpdateDate { get; set; }

        /// <summary>Creation time of the file.</summary>
        public DateTime CreationDate { get; set; }

        /// <summary>If true, stop processing the file any further.</summary>
        public bool StopProcessingRecord { get; set; }

        /// <summary>Gets or sets the identifier of the converter used to convert the file.</summary>
        public string ConverterIdentifier { get; set; }

        /// <summary>Gets or sets the success of the conversion.</summary>
        public bool ConversionFailed { get; set; } = false;

        /// <summary>Gets or sets the error message of the conversion.</summary>
        public string ConversionErrorMessage { get; set; }

        /// <summary>Gets or sets the error stacktrace of the conversion.</summary>
        public string ConversionErrorStackTrace { get; set; }

        /// <summary>List of files representing the document.</summary>
        public virtual List<ArchivalRecordDocumentFile> Files { get; set; }

        /// <summary>Additional data for the file.</summary>
        public Dictionary<string, string> AdditionalData { get; set; }

        /// <summary>Gets oncversion path.</summary>
        /// <param name="fileName">File name</param>
        /// <returns>Converstion path</returns>
        public string GetConversionPath(string fileName = null)
        {
            return Path.Combine(DirectoryConversionPath, fileName ?? OriginalName);
        }

        /// <summary>Initializes a new instance of the ArchivalRecordInput class.</summary>
        public ArchivalRecordInput()
        {
            Files = new List<ArchivalRecordDocumentFile>();
            AdditionalData = new Dictionary<string, string>();
        }

        /// <summary>Clones the ArchivalRecordInput instance.</summary>
        /// <param name="generateNewId">If true, a new unique identifier is generated</param>
        /// <returns>A new ArchivalRecordInput instance</returns>
        public ArchivalRecordInput Clone(bool generateNewId = false)
        {
            return new ArchivalRecordInput()
            {
                Id = generateNewId ? Guid.NewGuid() : Id,
                OriginalName = OriginalName,
                FullPathWithFileName = FullPathWithFileName,
                DirectoryRelativePath = DirectoryRelativePath,
                Hash = Hash,
                HashAlgorithm = HashAlgorithm,
                Size = Size,
                IsConvertedOrSkipped = IsConvertedOrSkipped,
                DownloadedAt = DownloadedAt,
                LastUpdateDate = LastUpdateDate,
                CreationDate = CreationDate,
                StopProcessingRecord = StopProcessingRecord,
                Files = Files,
                AdditionalData = AdditionalData,
                ConverterIdentifier = ConverterIdentifier
            };
        }

        /// <summary>Checks if the file has a specific extension.</summary>
        /// <param name="extensions">List of extensions to check for</param>
        /// <returns>True if the file has one of the specified extensions</returns>
        public bool HasExtension(IEnumerable<string> extensions)
        {
            if (OriginalName == null)
            {
                return false;
            }

            string extension = Path.GetExtension(OriginalName).Substring(1);

            return extensions.Contains(extension);
        }
    }
}