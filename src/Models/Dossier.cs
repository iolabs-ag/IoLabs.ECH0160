namespace IoLabs.ECH0160.Models
{
    /// <summary>Represents a dossier in the archival record.</summary>
    public class Dossier
    {
        /// <summary>Title for the Dossier.</summary>
        public string Title { get; set; }

        /// <summary>Converted Title for the Dossier.</summary>
        public string TitleAccordingToStandard { get; set; }

        /// <summary>List of Archival Record Documents.</summary>
        public IList<ArchivalRecordDocument> ArchivalRecordsDocuments { get; set; }

        /// <summary>List of Archival Record Inputs.</summary>
        public IList<ArchivalRecordInput> ArchivalRecordsInputs { get; set; }

        /// <summary>List of subfolders.</summary>
        public IList<Dossier> Subfolders { get; set; }

        /// <summary>Gets or sets the temp directory path.</summary>
        public string TempPath { get; set; }

        /// <summary>Gets or sets the temp directory path.</summary>
        public string PathAccordingToStandard { get; set; }

        /// <summary>Gets or sets the directory path.</summary>
        public string Path { get; set; }

        /// <summary>Gets or sets a flag indicating whether the dossier has been converted.</summary>
        public bool IsExcluded { get; set; }

        /// <summary>Initializes a new instance of the Dossier class.</summary>
        public Dossier()
        {
            ArchivalRecordsDocuments = new List<ArchivalRecordDocument>();
            ArchivalRecordsInputs = new List<ArchivalRecordInput>();
            Subfolders = new List<Dossier>();
        }

        /// <summary>Get archiving input files.</summary>
        /// <returns>List of archiving input files</returns>
        public IList<ArchivalRecordInput> GetFilesRecursive()
        {
            List<ArchivalRecordInput> files = ArchivalRecordsInputs.ToList();

            foreach (Dossier directory in Subfolders)
            {
                files.AddRange(directory.GetFilesRecursive());
            }

            return files;
        }

        /// <summary>Get archiving directories.</summary>
        /// <returns>List of archiving directories</returns>
        public IEnumerable<Dossier> GetDossiersRecursive()
        {
            List<Dossier> dossiers = new List<Dossier>();

            foreach (Dossier nonConvertedSubDossier in Subfolders.Where(f => !f.IsExcluded))
            {
                dossiers.Add(nonConvertedSubDossier);
                dossiers.AddRange(nonConvertedSubDossier.GetDossiersRecursive());
            }

            return dossiers;
        }

        /// <summary>Get Date Range of the Dossier.</summary>
        /// <returns>Date Range of the Dossier</returns>
        public DateRange GetDateRange()
        {
            DateRange dateRange = new DateRange();

            foreach (ArchivalRecordDocument document in ArchivalRecordsDocuments)
            {
                dateRange.Extend(document.GetDateRange());
            }

            foreach (Dossier subfolder in Subfolders)
            {
                dateRange.Extend(subfolder.GetDateRange());
            }

            return dateRange;
        }

        public bool ContainsFiles()
        {
            return ArchivalRecordsDocuments.SelectMany(ard => ard.DocumentFiles).Any() || Subfolders.Any(f => f.ContainsFiles());
        }
    }
}