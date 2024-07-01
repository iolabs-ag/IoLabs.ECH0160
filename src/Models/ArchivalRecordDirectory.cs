namespace IoLabs.ECH0160.Models
{
    /// <summary>Represents a directory in the archival record.</summary>
    public class ArchivalRecordDirectory
    {
        /// <summary>Gets or sets the name of the directory.</summary>
        public IList<ArchivalRecordDirectory> Directories { get; set; }

        /// <summary>Gets or sets the list of files in the directory.</summary>
        public IList<ArchivalRecordInput> Files { get; set; }

        /// <summary>Gets or sets the name of the directory.</summary>
        public string Path { get; set; }

        /// <summary>Gets or sets the temporary path of the directory.</summary>
        public string TempPath { get; set; }

        /// <summary>Gets the name of the leaf directory.</summary>
        public string LeafFolderName
        {
            get
            {
                return Path.Split(new char[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar }).Last();
            }
        }

        /// <summary>Initializes a new instance of the ArchivalRecordDirectory class.</summary>
        /// <param name="path">The path of the directory</param>
        public ArchivalRecordDirectory(string path = null)
        {
            Path = path;
            Directories = new List<ArchivalRecordDirectory>();
            Files = new List<ArchivalRecordInput>();
        }

        /// <summary>Recursively gets all files in the directory and its subdirectories.</summary>
        /// <returns>A list of all files in the directory and its subdirectories</returns>
        public IList<ArchivalRecordInput> GetFilesRecursive()
        {
            List<ArchivalRecordInput> files = Files.ToList();

            foreach (ArchivalRecordDirectory directory in Directories)
            {
                files.AddRange(directory.GetFilesRecursive());
            }

            return files;
        }
    }
}