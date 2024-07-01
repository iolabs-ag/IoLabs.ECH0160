namespace IoLabs.ECH0160.Exceptions
{
    /// <summary>Exception thrown when an error occurs during archiving.</summary>
    public class ArchivingException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="ArchivingException"/> class.</summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
        public ArchivingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}