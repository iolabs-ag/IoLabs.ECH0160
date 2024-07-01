using IoLabs.ECH0160.Conversions;
using IoLabs.ECH0160.Models;

namespace IoLabs.ECH0160.Exceptions
{
    /// <summary>This class represents exceptions that occur during the conversion of files. It inherits from the ArchivingException class.</summary>
    public class ConversionException : ArchivingException
    {
        /// <summary>Gets or sets the converter used for the conversion.</summary>
        public IFileConverter Converter { get; set; }

        /// <summary>Gets or sets the archival record input that failed to convert.</summary>
        public ArchivalRecordInput ArchivingRecordInput { get; set; }

        /// <summary>Initializes a new instance of the ConversionException class.</summary>
        /// <param name="archivingRecord">The archival record input that failed to convert</param>
        /// <param name="converter">The converter used for the conversion</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified</param>
        /// <param name="message">The error message that explains the reason for the exception</param>
        public ConversionException(ArchivalRecordInput archivingRecord, IFileConverter converter, Exception innerException = null, string message = "") : base(message, innerException)
        {
            ArchivingRecordInput = archivingRecord;
            Converter = converter;
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// The message includes the file path of the input file that failed to convert and the identifier of the converter used.
        /// If a base message is provided, it is appended to the end of the message.
        /// </summary>
        public override string Message
        {
            get
            {
                return $"Failed to convert input file [{ArchivingRecordInput.FullPathWithFileName}] using converter [{Converter.Identifier}].{(string.IsNullOrEmpty(base.Message) ? string.Empty : $"Details: {base.Message}")}";
            }
        }
    }
}