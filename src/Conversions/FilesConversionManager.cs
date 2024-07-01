using IoLabs.ECH0160.Models;
using Microsoft.Extensions.Logging;

namespace IoLabs.ECH0160.Conversions
{
    /// <summary>Manages file conversions using a list of IFileConverter implementations.</summary>
    public class FilesConversionManager : IFilesConversionManager
    {
        private readonly IEnumerable<IFileConverter> converters;
        private readonly IEnumerable<IDirectoryConverter> directoryConverters;
        private readonly ILogger<FilesConversionManager> logger;

        /// <summary>Initializes a new instance of the FilesConversionManager with a list of converters.</summary>
        /// <param name="converters">List of file converters</param>
        /// <param name="directoryConverters">List of directory converters</param>
        /// <param name="logger">Logger</param>
        public FilesConversionManager(IEnumerable<IFileConverter> converters, IEnumerable<IDirectoryConverter> directoryConverters, ILogger<FilesConversionManager> logger)
        {
            this.converters = converters.OrderByDescending(c => c.Priority); // ensure order by priority
            this.directoryConverters = directoryConverters;
            this.logger = logger;
        }

        /// <summary>Attempts to convert a file using the appropriate converter based on priority and support.</summary>
        /// <param name="recordInput">The archival record to convert</param>
        /// <returns>The archival record document</returns>
        public async Task<ArchivalRecordDocument> ConvertFile(ArchivalRecordInput recordInput)
        {
            IFileConverter converter = converters.FirstOrDefault(c => c.IsFileSupported(recordInput));

            if (converter != null)
            {
                int runIndex = 0;
                int maxRetries = 5; // TODO: move to configuration, add delay between retries

                logger.LogDebug("Converting file [{FilePath}] using converter [{Converter}]", recordInput.FullPathWithFileName, converter.Identifier);

                recordInput.ConverterIdentifier = converter.Identifier;

                while (true)
                {
                    runIndex++;

                    try
                    {
                        ArchivalRecordDocument archivalRecordDocument = await converter.ConvertFile(recordInput);

                        recordInput.IsConvertedOrSkipped = true;

                        logger.LogTrace("Converting file [{FilePath}] using converter [{Converter}] DONE", recordInput.FullPathWithFileName, converter.Identifier);

                        return archivalRecordDocument;
                    }
                    catch (Exception ex)
                    {
                        if (runIndex >= maxRetries)
                        {
                            recordInput.ConversionFailed = true;
                            recordInput.ConversionErrorMessage = ex.Message;
                            recordInput.ConversionErrorStackTrace = ex.StackTrace;

                            return null;
                        }

                        logger.LogWarning(ex, "Error converting file [{FilePath}] using converter [{Converter}], rescheduled", recordInput.FullPathWithFileName, converter.Identifier);

                        Thread.Sleep(runIndex * 3000); // TODO: move to configuration
                    }
                }
            }
            else
            {
                recordInput.IsConvertedOrSkipped = true;

                return ArchivalRecordDocument.FromArchivalRecordInput(recordInput);
            }
        }

        /// <summary>Attempts to convert a directory using the appropriate converter based on priority and support.</summary>
        /// <param name="directory">The archival record directory to convert</param>
        /// <returns>The archival record document</returns>
        public async Task<ArchivalRecordDocument> ConvertDirectory(Dossier directory)
        {
            IDirectoryConverter converter = directoryConverters.OrderByDescending(c => c.Priority).FirstOrDefault(c => c.IsDirectorySupported(directory));

            if (converter != null)
            {
                ArchivalRecordDocument archivalRecordInput = await converter.ConvertDirectory(directory);

                directory.IsExcluded = true;

                return archivalRecordInput;
            }

            return null;
        }
    }
}