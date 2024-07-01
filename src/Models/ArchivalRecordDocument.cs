namespace IoLabs.ECH0160.Models
{
    /// <summary>Represents a document in the archival record.</summary>
    public class ArchivalRecordDocument
    {
        /// <summary>Gets or sets the unique identifier for the document.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Gets or sets the name of the document before conversion.</summary>
        public string Title { get; set; }

        /// <summary>Gets or sets the folder path.</summary>
        public string DirectoryRelative { get; set; }

        /// <summary>Addition data for the document.</summary>
        public Dictionary<string, string> AdditionalData { get; set; }

        /// <summary>Gets or sets the list of inut files creating the document.</summary>
        public IList<ArchivalRecordInput> InputFiles { get; set; }

        /// <summary>Gets or sets the list of inut files creating the document.</summary>
        public IList<ArchivalRecordDocumentFile> DocumentFiles { get; set; }

        /// <summary>Initializes a new instance of the ArchivalRecordDocument class.</summary>
        public ArchivalRecordDocument()
        {
            InputFiles = new List<ArchivalRecordInput>();
            DocumentFiles = new List<ArchivalRecordDocumentFile>();
            AdditionalData = new Dictionary<string, string>();
        }

        /// <summary>Creates a new ArchivalRecordDocument from an ArchivalRecordInput.</summary>
        /// <param name="input">Input file to create the document from</param>
        /// <param name="directory">Directory path for the document</param>
        /// <param name="inputs">List of input files creating the document</param>
        /// <returns>A new ArchivalRecordDocument instance</returns>
        public static ArchivalRecordDocument FromArchivalRecordInput(ArchivalRecordInput input, string directory = null, List<ArchivalRecordInput> inputs = null)
        {
            return new ArchivalRecordDocument()
            {
                Id = Guid.NewGuid(),
                Title = input.OriginalName,
                AdditionalData = input.AdditionalData,
                DirectoryRelative = directory ?? input.DirectoryRelativePath,
                InputFiles = inputs ?? new List<ArchivalRecordInput>() { input }
            };
        }

        /// <summary>Gets the date range of the document based on the creation and last update dates of the input files.</summary>
        /// <returns>A DateRange object representing the date range of the document</returns>
        public DateRange GetDateRange()
        {
            DateRange dateRange = new DateRange();

            foreach (ArchivalRecordInput file in InputFiles)
            {
                dateRange.Extend(new DateRange { MinDate = file.CreationDate, MaxDate = file.LastUpdateDate });
            }

            return dateRange;
        }
    }
}