using IoLabs.ECH0160.Models;

namespace IoLabs.ECH0160.Conversions.Converters
{
    /// <summary>A file converter that do nothing.</summary>
    public class SimpleCopyConverter : ExtensionBasedFileConverter
    {
        /// <summary>The list of supported extensions.</summary>
        protected override List<string> SupportedExtensions { get { return new List<string> { "xml", "csv", "txt", "jpg", "jpeg", "jp2", "png", "tif", "tiff" }; } }

        /// <summary>Converter's identifier.</summary>
        public override string Identifier { get { return "CopyOnly"; } }

        /// <summary>Gets the priority of the image file converter.</summary>
        public override int Priority { get { return 100; } }

        /// <summary>EmailEmlConverter Constructor.</summary>
        /// <exception cref="ArgumentNullException">Exception</exception>
        public SimpleCopyConverter()
        {
        }

        /// <summary>Converts the given video file to .mp4 and packs it into a .zip.</summary>
        /// <param name="recordInput">The archival record containing the video file to be converted</param>
        /// <returns>A task representing the asynchronous operation, with a boolean result indicating the success of the conversion</returns>
        public override async Task<ArchivalRecordDocument> ConvertFile(ArchivalRecordInput recordInput)
        {
            string destinationPath = recordInput.GetConversionPath();

            using (FileStream sourceStream = new FileStream(recordInput.FullPathWithFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                using (FileStream destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }
            }

            ArchivalRecordDocument document = ArchivalRecordDocument.FromArchivalRecordInput(recordInput);

            ArchivalRecordDocumentFile file = new ArchivalRecordDocumentFile()
            {
                TempFileName = recordInput.OriginalName,
                DirectoryConversionPath = recordInput.DirectoryConversionPath,
                IsConverted = false,
            };

            document.DocumentFiles.Add(file);

            return document;
        }
    }
}