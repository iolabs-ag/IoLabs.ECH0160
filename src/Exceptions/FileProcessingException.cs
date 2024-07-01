using IoLabs.ECH0160.Models;

namespace IoLabs.ECH0160.Exceptions
{
    /// <summary>This class represents exceptions that occur during the processing of files. It inherits from the ArchivingException class.</summary>
    public class FileProcessingException : ArchivingException
    {
        /// <summary>Gets or sets the archival record document that failed to process.</summary>
        public ArchivalRecordDocument Document { get; set; }

        /// <summary>
        /// Gets or sets the archival record document file that failed to process.
        /// This property can be null if the exception occurred while processing the document itself, not a specific file.
        /// </summary>
        public ArchivalRecordDocumentFile File { get; set; }

        /// <summary>Initializes a new instance of the FileProcessingException class. This constructor is used when the exception occurred while processing the document itself.</summary>
        /// <param name="document">The archival record document that failed to process</param>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified</param>
        public FileProcessingException(ArchivalRecordDocument document, string message = "", Exception innerException = null) : base(message, innerException)
        {
            Document = document;
        }

        /// <summary>
        /// Initializes a new instance of the FileProcessingException class.
        /// This constructor is used when the exception occurred while processing a specific file in the document.
        /// </summary>
        /// <param name="document">The archival record document that contains the file</param>
        /// <param name="file">The archival record document file that failed to process</param>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified</param>
        public FileProcessingException(ArchivalRecordDocument document, ArchivalRecordDocumentFile file, string message = "", Exception innerException = null) : base(message, innerException)
        {
            Document = document;
            File = file;
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// The message includes the identifier of the document and the file (if any) that failed to process.
        /// If a base message is provided, it is appended to the end of the message.
        /// </summary>
        public override string Message
        {
            get
            {
                return File != null ?
                    $"Failed to process file [{(string.IsNullOrEmpty(File.TempFileName) ? File.Id : File.TempFileName)}] of document [{(string.IsNullOrEmpty(Document.Title) ? Document.Id : Document.DirectoryRelative + "/" + Document.Title)}].{(string.IsNullOrEmpty(base.Message) ? string.Empty : $" Details: {base.Message}")}" :
                    $"Failed to process document [{Document.Id}:{Document.Title}].{(string.IsNullOrEmpty(base.Message) ? string.Empty : $" Details: {base.Message}")}";
            }
        }
    }
}