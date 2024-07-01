using IoLabs.ECH0160.Conversions;
using IoLabs.ECH0160.Exceptions;
using IoLabs.ECH0160.Metadata;
using IoLabs.ECH0160.Metadata.Checksums;
using IoLabs.ECH0160.Models;
using IoLabs.ECH0160.Normalizations;
using Microsoft.Extensions.Logging;

namespace IoLabs.ECH0160
{
    /// <summary>Generates a SIP package structure as per the eCH-0160 standard.</summary>
    public class AdvancedSIPGenerator : IAdvancedSIPGenerator
    {
        private readonly IFilesConversionManager filesConversionManager;
        private readonly IPathNormalizer pathNormalizer;
        private readonly IHashCalculator hashCalculator;
        private readonly IMetadataGenerator generator;
        private readonly ILogger<AdvancedSIPGenerator> logger;

        /// <summary>Initializes a new instance of the AdvancedSIPGenerator class with specified services for SIP package generation.</summary>
        /// <param name="filesConversionManager">The service for converting files to the eCH-0160 standard</param>
        /// <param name="pathNormalizer">The service for normalizing file paths within the SIP package</param>
        /// <param name="hashCalculator">The service for calculating file hashes</param>
        /// <param name="generator">The metadata generator service</param>
        /// <param name="logger">The logger for capturing log messages throughout the process</param>
        public AdvancedSIPGenerator(IFilesConversionManager filesConversionManager, IPathNormalizer pathNormalizer, IHashCalculator hashCalculator, IMetadataGenerator generator, ILogger<AdvancedSIPGenerator> logger = null)
        {
            this.generator = generator;
            this.filesConversionManager = filesConversionManager;
            this.pathNormalizer = pathNormalizer;
            this.hashCalculator = hashCalculator;
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task<SipPackage> CreateSIPPackage(ArchiveDeliveryDetails archivalDetails, Guid temporaryProcessID, OrdnungSystem sysInformation, string baseDirectory, string convertedFilesDirectory,
                                                       Func<ArchivalRecordDocument, ArchivalRecordInput, Task> onRecordConversionPassed = null, Func<ArchivalRecordDocument, Task> onRecordNameNormalizationDone = null)
        {
            // TODO: only if needed
            BuildTree(sysInformation, onRecordConversionPassed, $"{baseDirectory}/{temporaryProcessID}/{convertedFilesDirectory}");

            await ConvertData(sysInformation, onRecordConversionPassed); // TODO consider moving this oustide of the method

            // TODO remap to new structure to avoid filtering of converted files, do not use string dictionary
            Dictionary<string, string> xmlMetaDataStructure = generator.GetSIPPackageMetadataXMLStructure(temporaryProcessID, baseDirectory, archivalDetails.DeliveryType);

            await generator.AddXsdSwissStandardFilesAsync(xmlMetaDataStructure["xsd-dir"]);

            IList<ArchivalRecordInput> headerFiles = await generator.GetArchivalRecordFilesHeaderAsync(xmlMetaDataStructure);

            await ChangeNamesAndDirectoriesAccordingToStandard(sysInformation, onRecordNameNormalizationDone);

            generator.CreateXmlMetadataFile(headerFiles, archivalDetails, xmlMetaDataStructure, sysInformation);

            MoveFilesToContentDirectory(sysInformation, xmlMetaDataStructure["content"]);

            return new SipPackage()
            {
                DeliveryDetails = archivalDetails,
                SipPackagePath = xmlMetaDataStructure["temp-dir"],
                BaseDirectory = baseDirectory,
                SipPackageName = GeneratePackageName(archivalDetails),
                XmlMetaDataStructure = xmlMetaDataStructure
            };
        }

        #region Private methods

        private void BuildTree(OrdnungSystem sysInformation, Func<ArchivalRecordDocument, ArchivalRecordInput, Task> onRecordConversionDone, string rootTempDir = null)
        {
            foreach (Dossier dossier in sysInformation.GetDossiersRecursive()) // Intentionally not mapping title
            {
                Dossier rootDirectory = GetTree(dossier.ArchivalRecordsInputs, rootTempDir, dossier.Path ?? string.Empty);
                dossier.Subfolders = rootDirectory.Subfolders;
                dossier.ArchivalRecordsInputs = rootDirectory.ArchivalRecordsInputs;
                dossier.Path = rootDirectory.Path;
                dossier.TempPath = rootDirectory.TempPath;
            }
        }

        private async Task ConvertData(OrdnungSystem sysInformation, Func<ArchivalRecordDocument, ArchivalRecordInput, Task> onRecordConversionDone)
        {
            foreach (Dossier dossier in sysInformation.GetDossiersRecursive())
            {
                ArchivalRecordDocument archivalRecordDocument = await ConvertDirectory(dossier, onRecordConversionDone);

                if (archivalRecordDocument != null) // adding to the same top level dossier, since we cannot go up in the tree   
                {
                    dossier.ArchivalRecordsDocuments.Add(archivalRecordDocument);
                }
            }
        }

        private async Task<ArchivalRecordDocument> ConvertDirectory(Dossier dossier, Func<ArchivalRecordDocument, ArchivalRecordInput, Task> onRecordConversionDone)
        {
            ArchivalRecordDocument archivalRecordDocument = await filesConversionManager.ConvertDirectory(dossier);

            if (archivalRecordDocument != null)
            {
                if (onRecordConversionDone != null)
                {
                    await onRecordConversionDone.Invoke(archivalRecordDocument, null); // TODO: consider passing the dossier or a list of files
                }

                return archivalRecordDocument;
            }
            else
            {
                foreach (Dossier recordSubDirectory in dossier.Subfolders) // convert content, dirs
                {
                    ArchivalRecordDocument convertDirectoryDocument = await ConvertDirectory(recordSubDirectory, onRecordConversionDone);

                    if (convertDirectoryDocument != null)
                    {
                        dossier.ArchivalRecordsDocuments.Add(convertDirectoryDocument);
                    }
                }

                foreach (ArchivalRecordInput recordInput in dossier.ArchivalRecordsInputs) // convert files
                {
                    if (string.IsNullOrEmpty(recordInput.DirectoryConversionPath)) // prepare path
                    {
                        recordInput.DirectoryConversionPath = Path.Combine(dossier.TempPath);
                    }

                    Directory.CreateDirectory(recordInput.DirectoryConversionPath); // create the directory if it does not exist

                    ArchivalRecordDocument convertedRecord = await filesConversionManager.ConvertFile(recordInput);

                    if (convertedRecord != null)
                    {
                        dossier.ArchivalRecordsDocuments.Add(convertedRecord);
                    }

                    if (onRecordConversionDone != null)
                    {
                        await onRecordConversionDone.Invoke(convertedRecord, recordInput);
                    }
                }

                return null;
            }
        }

        private async Task<IList<ArchivalRecordDocument>> ConvertDirectories(IList<Dossier> archivalRecordDirectories, Func<ArchivalRecordDocument, Task> onRecordConversionDone)
        {
            List<ArchivalRecordDocument> dirsConvertedToFiles = new List<ArchivalRecordDocument>();

            foreach (Dossier archivalRecordDirectory in archivalRecordDirectories)
            {
                ArchivalRecordDocument archivalRecordDocument = await filesConversionManager.ConvertDirectory(archivalRecordDirectory);

                if (archivalRecordDocument == null)
                {
                    IList<ArchivalRecordDocument> archivalRecordDocuments = await ConvertDirectories(archivalRecordDirectory.Subfolders, onRecordConversionDone);

                    dirsConvertedToFiles.AddRange(archivalRecordDocuments);
                }
                else
                {
                    if (onRecordConversionDone != null)
                    {
                        await onRecordConversionDone.Invoke(archivalRecordDocument);
                    }

                    dirsConvertedToFiles.Add(archivalRecordDocument); // TODO: trigger storing of the file to journal (DB)
                }
            }

            return dirsConvertedToFiles;
        }

        private Dossier GetTree(IList<ArchivalRecordInput> archivalRecords, string rootTempDir, string dirPrefix)
        {
            Dictionary<string, Dossier> dirsMap = new Dictionary<string, Dossier>();
            Dossier root = new Dossier() { TempPath = rootTempDir ?? string.Empty, Path = dirPrefix};

            dirsMap.Add(root.Path, root);

            var orderedEnumerable = archivalRecords.OrderBy(p => p.DirectoryRelativePath)
                                                   .GroupBy(p => new
                                                   {
                                                       path = Path.Combine(dirPrefix, Path.GetDirectoryName(p.DirectoryRelativePath) ?? string.Empty),
                                                       tempPath = Path.GetDirectoryName(p.FullPathWithFileName) ?? string.Empty
                                                   }, q => q)
                                                   .OrderBy(p => p.Key.path);

            foreach (var group in orderedEnumerable) // assuming ordered ascending by path, so the subfolders will be added after and to the parent folder
            {
                Dossier directory;

                if (group.Key.path != root.Path) // root
                {
                    directory = new Dossier();
                    dirsMap.Add(group.Key.path, directory);
                }
                else
                {
                    directory = root;
                }

                directory.Path = group.Key.path;
                directory.Title = directory.Path.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }).Last();
                directory.TempPath = Path.Combine(rootTempDir, group.Key.path).NormalizePathSeparators();
                directory.ArchivalRecordsInputs = group.ToList();

                int lastIndexOfSlash = directory.Path.Trim(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }).LastIndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });

                if (lastIndexOfSlash > 0)
                {
                    string parentDirPath = directory.Path.Substring(0, lastIndexOfSlash);

                    dirsMap.TryGetValue(parentDirPath, out Dossier parentDir);

                    if (parentDir == null)
                    {
                        // build non existing parent directories                        
                        string[] pathParts = parentDirPath.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }); // TODO: consider creating grouping entry by path only and let this algorythm to handle it automatically
                        string currentPath = string.Empty;

                        Dossier pDir = root;
                        Dossier newParentDir = null;

                        foreach (string pathPart in pathParts)
                        {
                            currentPath = Path.Combine(currentPath, pathPart);

                            if (!dirsMap.ContainsKey(currentPath))
                            {
                                newParentDir = new Dossier()
                                {
                                    Path = currentPath,
                                    Title = pathPart,
                                    TempPath = Path.Combine(root.TempPath, currentPath)
                                };

                                pDir?.Subfolders.Add(newParentDir);
                                dirsMap.Add(currentPath, newParentDir);
                                pDir = newParentDir;
                            }
                            else
                            {
                                pDir = dirsMap[currentPath];
                            }
                        }

                        parentDir = newParentDir;
                    }

                    parentDir.Subfolders.Add(directory);
                }
                else
                {
                    if (root != directory) // TODO: better
                    {
                        root.Subfolders.Add(directory);
                    }
                }
            }

            return root;
        }

        /*
            In a SIP, the name of the top-level folder always begins with the character sequence SIP_ followed by specific identifying information [identifiers].
            Recommendation : The name of the top-level folder is to be constructed in accordance with the following pattern:
            SIP_
            [submission date]
            _
            [name of submitting authority]
            _
            [reference]
            [submission date]: date in the format: YYYYMMDD
            [name of the submitting authority]: The name of the submitting authority is given in the form of the official abbreviation (e.g. EPA, SDC, FSO). If no official abbreviation is available, a short but meaningful designation of the submitting authority should be chosen.
            [reference]: Use of the reference by the submitting authority is optional. It can consist, for example, of the abbreviation for the name of the contact or database or the submission number.
         */

        private string GeneratePackageName(ArchiveDeliveryDetails archivalDetails)
        {
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time")); // TODO: configurable timezone

            string name = $"SIP_{now:yyyyMMdd}_{archivalDetails.DeliveringAgencyShort}";

            if (!string.IsNullOrEmpty(archivalDetails.Reference))
            {
                name += $"_{archivalDetails.Reference}";
            }

            name += $"_{now:HHmmss}"; // TODO: check id this is needed

            return name;
        }

        private void MoveFilesToContentDirectory(OrdnungSystem sysInformation, string sipContentDir)
        {
            foreach (Dossier dossier in sysInformation.GetDossiersRecursive(false).Included())
            {
                string targetDir = Path.Combine(sipContentDir, dossier.PathAccordingToStandard ?? string.Empty);

                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                foreach (ArchivalRecordDocument archivalRecordDocument in dossier.ArchivalRecordsDocuments)
                {
                    foreach (ArchivalRecordDocumentFile archivalRecordDocumentFile in archivalRecordDocument.DocumentFiles)
                    {
                        if (!string.IsNullOrEmpty(archivalRecordDocumentFile.NameAccordingToStandard) && !string.IsNullOrEmpty(archivalRecordDocumentFile.TempFileName) && !string.IsNullOrEmpty(archivalRecordDocumentFile.DirectoryConversionPath))
                        {
                            string convertedFile = Path.Combine(archivalRecordDocumentFile.DirectoryConversionPath, archivalRecordDocumentFile.TempFileName);
                            string destinationPath = Path.Combine(targetDir, archivalRecordDocumentFile.NameAccordingToStandard);

                            if (File.Exists(convertedFile))
                            {
                                File.Copy(convertedFile, destinationPath, overwrite: true);
                            }
                        }
                        else
                        {
                            throw new FileProcessingException(archivalRecordDocument, archivalRecordDocumentFile, "Unable to move file to SIP, file name cannot be empty!");
                        }
                    }
                }
            }
        }

        private async Task ChangeNamesAndDirectoriesAccordingToStandard(OrdnungSystem sysInformation, Func<ArchivalRecordDocument, Task> onRecordNameNormalizationDone = null)
        {
            int fileCounter = 1;

            List<Dossier> dossiers = sysInformation.GetDossiersRecursive(false).Included();
            List<string> paths = dossiers.Select(d => d.Path).ToList();
            Dictionary<string, string> mappedDirs = pathNormalizer.NormalizeDirectories(paths);

            foreach (Dossier dossier in dossiers)
            {
                dossier.PathAccordingToStandard = mappedDirs[dossier.Path ?? string.Empty]; // TODO: normalize directly

                string newPath = dossier.PathAccordingToStandard.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }).Last();

                dossier.TitleAccordingToStandard = string.IsNullOrEmpty(newPath) ? dossier.Title : newPath;

                foreach (ArchivalRecordDocument recordDocument in dossier.ArchivalRecordsDocuments)
                {
                    foreach (ArchivalRecordDocumentFile archivalRecordDocumentFile in recordDocument.DocumentFiles)
                    {
                        if (!string.IsNullOrEmpty(archivalRecordDocumentFile.TempFileName))
                        {
                            string normalizedFolder;

                            if (dossier.Path != null && dossier.Path != string.Empty)
                            {
                                mappedDirs.TryGetValue(pathNormalizer.NormalizeDirectorySeparators(dossier.Path), out normalizedFolder);

                                if (normalizedFolder == null)
                                {
                                    logger?.LogWarning("Directory {Directory} not found in the mapping.", dossier.Path);

                                    normalizedFolder = string.Empty;
                                }
                            }
                            else
                            {
                                normalizedFolder = string.Empty;
                            }

                            archivalRecordDocumentFile.NameAccordingToStandard = pathNormalizer.NormalizeFileName(archivalRecordDocumentFile.TempFileName, fileCounter);
                            archivalRecordDocumentFile.DirectoryPathAccordingToStandard = normalizedFolder;
                            archivalRecordDocumentFile.Hash = await hashCalculator.CalculateHash(Path.Combine(archivalRecordDocumentFile.DirectoryConversionPath, archivalRecordDocumentFile.TempFileName));
                            archivalRecordDocumentFile.HashAlgorithm = hashCalculator.GetAlgorithmCode();

                            fileCounter++;
                        }
                        else
                        {
                            throw new FileProcessingException(recordDocument, archivalRecordDocumentFile, "Unable to rename file, file name to rename cannot be empty!");
                        }
                    }

                    if (onRecordNameNormalizationDone != null)
                    {
                        await onRecordNameNormalizationDone.Invoke(recordDocument);
                    }
                }
            }
        }

        #endregion
    }
}