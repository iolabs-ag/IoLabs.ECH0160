namespace IoLabs.ECH0160.Models
{
    /// <summary>Represents a file in the archival record.</summary>
    public class ArchivalRecordDocumentFile
    {
        /// <summary>Gets or sets the unique identifier for the file.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Gets or sets the original folder path.</summary>
        public string DirectoryOriginalPath { get; set; }

        /// <summary>Gets or sets the name of the file after conversion.</summary>
        public string TempFileName { get; set; }

        /// <summary>Gets or sets the conversion file path.</summary>
        public string DirectoryConversionPath { get; set; }

        /// <summary>Gets or sets the name of the file.</summary>
        public string NameAccordingToStandard { get; set; }

        /// <summary>Gets or sets the name of the file hash.</summary>
        public string Hash { get; set; }

        /// <summary>Gets or sets the algorithm used for file integrity check.</summary>
        public string HashAlgorithm { get; set; }

        /// <summary>Gets or sets the size of the downloaded file.</summary>
        public long Size { get; set; }

        /// <summary>Gets or sets the created folder path according to standard.</summary>
        public string DirectoryPathAccordingToStandard { get; set; }

        /// <summary>Gets or sets the relative folder path.</summary>
        public string DirectoryRelativePath { get; set; }

        /// <summary>Gets or sets a flag indicating whether the file has been converted.</summary>
        public bool IsConverted { get; set; }
    }
}