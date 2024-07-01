namespace IoLabs.ECH0160.Normalizations
{
    /// <summary>Provides utilities for path normalization and validation.</summary>
    public class PathNormalizer : IPathNormalizer
    {
        /// <summary>Gets the maximum length of a path.</summary>
        public int PadLength = 6;

        /// <summary>Normalizes the path of the given archival record with respect to all records.</summary>
        /// <param name="originalDirectoryPath">The archival record to normalize</param>
        /// <returns>The archival record with normalized path</returns>
        public string NormalizePathStructureAccordingToStandard(string originalDirectoryPath)
        {
            if (!string.IsNullOrEmpty(originalDirectoryPath))
            {
                // Split the original path by backslashes (or whatever separator you use)
                string[] segments = originalDirectoryPath.Trim('/').Split('/');

                // Transform each segment into the new format
                IEnumerable<string> transformedSegments = segments.Select((segment, index) => $"/d{index + 1:000}"); // This formats the index as three digits, starting from 001

                // Join the transformed segments back into a single path string
                string normalizedPath = string.Join(string.Empty, transformedSegments).TrimStart('/').TrimEnd('/');

                return normalizedPath;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public Dictionary<string, string> NormalizeDirectories(List<string> directories)
        {
            Dictionary<string, string> mappedDirs = new Dictionary<string, string>();

            int dirCounter = 1;

            foreach (string directory in directories)
            {
                if (directory == string.Empty)
                {
                    mappedDirs.Add(directory, string.Empty);
                }
                else
                {
                    string[] dirs = NormalizeDirectorySeparators(directory).Split(Path.DirectorySeparatorChar);
                    string dirPath = string.Empty;
                    string newDirName = string.Empty;

                    foreach (string dir in dirs)
                    {
                        dirPath = Path.Combine(dirPath, dir);
                        mappedDirs.TryGetValue(dirPath, out string mappedDirPath);

                        if (mappedDirPath == null)
                        {
                            newDirName = Path.Combine(newDirName, $"d{dirCounter.ToString().PadLeft(PadLength, '0')}"); // TODO: padlength dynamic
                            mappedDirs.Add(dirPath, newDirName);

                            dirCounter++;
                        }
                        else
                        {
                            newDirName = mappedDirPath;
                        }
                    }
                }
            }

            return mappedDirs;
        }

        /// <inheritdoc />
        public string NormalizeFileName(string fileName, int counter)
        {
            string extension = Path.GetExtension(fileName).ToLower();

            return $"p{counter.ToString().PadLeft(PadLength, '0')}{extension}";
        }

        /// <summary>Checks if a character is allowed in a file name.</summary>
        /// <param name="c">The character to check</param>
        /// <returns>True if the character is allowed; otherwise, false</returns>
        private bool IsAllowedCharacter(char c)
        {
            return char.IsLetterOrDigit(c) || " !#$%()+-=./@[]{}_~".Contains(c);
        }

        /// <summary>Generates a shortened path name by appending a zero-padded subfolder count to a prefix.</summary>
        /// <param name="name">The original name of the path, not used in this implementation but may be useful for future extensions</param>
        /// <param name="subfolderCount">The count of the subfolder which is used to generate the suffix</param>
        /// <returns>A shortened path string consisting of a 'd' prefix followed by a zero-padded subfolder count</returns>
        private string ShortenPath(string name, int subfolderCount)
        {
            return "d" + subfolderCount.ToString("D3");
        }

        /// <summary>Normalizes the path of the given archival record.</summary>
        /// <param name="originalName">The original file name to normalize</param>
        /// <returns>A normalized file name</returns>
        public string NormalizePathNameAccordingToStandard(string originalName)
        {
            if (!string.IsNullOrEmpty(originalName))
            {
                string normalizedBase = new string(originalName.SelectMany(c => CharConversionsTables.ReplacementMap1252.ContainsKey(c) ? CharConversionsTables.ReplacementMap1252[c] : new string(c, 1)).ToArray());

                normalizedBase = new string(normalizedBase.SelectMany(c => CharConversionsTables.ReplacementMap8859.ContainsKey(c) ? CharConversionsTables.ReplacementMap8859[c] : new string(c, 1)).ToArray());
                normalizedBase = new string(normalizedBase.SelectMany(c => CharConversionsTables.ReplacementMapUsAscii.ContainsKey(c) ? CharConversionsTables.ReplacementMapUsAscii[c] : new string(c, 1)).ToArray());

                return normalizedBase;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public string NormalizeDirectorySeparators(string filePath, bool trim = true)
        {
            string normalizedPath = filePath.NormalizePathSeparators();

            if (trim)
            {
                normalizedPath = normalizedPath.Trim(Path.DirectorySeparatorChar);
            }

            return normalizedPath;
        }
    }
}