namespace IoLabs.ECH0160.Normalizations
{
    /// <summary>Provides utilities for path normalization and validation within archival records.</summary>
    public interface IPathNormalizer
    {
        /// <summary>Normalizes the path of the given archival record with respect to all records.</summary>
        /// <param name="originalDirectoryPath">The archival record to normalize</param>
        /// <returns>The archival record with normalized path</returns>
        public string NormalizePathStructureAccordingToStandard(string originalDirectoryPath);

        /// <summary>Normalizes the path of the given archival record.</summary>
        /// <param name="originalName">The original file name to normalize</param>
        /// <returns>A normalized file name</returns>
        public string NormalizePathNameAccordingToStandard(string originalName);

        /// <summary>Normalize directory separators.</summary>
        /// <param name="filePath">Old path</param>
        /// <param name="trim">Trim from beginning and end</param>
        /// <returns>Directory path</returns>
        public string NormalizeDirectorySeparators(string filePath, bool trim = true);

        /// <summary>Normalize directories structure.</summary>
        /// <param name="directories">List of directories</param>
        /// <returns>Dictionary of normalized directories</returns>
        public Dictionary<string, string> NormalizeDirectories(List<string> directories);

        /// <summary>Normalize file name.</summary>
        /// <param name="fileName">File name</param>
        /// <param name="counter">Counter</param>
        /// <returns>Normalized file name</returns>
        public string NormalizeFileName(string fileName, int counter);
    }
}