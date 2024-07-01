namespace IoLabs.ECH0160
{
    /// <summary>Extension methods for string manipulation.</summary>
    public static class StringExtensions
    {
        /// <summary>Normalize path separators.</summary>
        /// <param name="path">Path to normalize</param>
        /// <returns>Normalized path</returns>
        public static string NormalizePathSeparators(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            string normalizedPath = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            return normalizedPath;
        }

        /// <summary>Forward path separators.</summary>
        /// <param name="path">Path to forward</param>
        /// <returns>Path with forward separators</returns>
        public static string ForwardPathSeparators(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            string normalizedPath = path.Replace('\\', '/');

            return normalizedPath;
        }
    }
}