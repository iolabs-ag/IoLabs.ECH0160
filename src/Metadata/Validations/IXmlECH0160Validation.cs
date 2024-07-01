using Microsoft.Extensions.Logging;

namespace IoLabs.ECH0160.Metadata.Validations
{
    /// <summary>Implements IXMLeCH0160Validation for validating XML against eCH-0160 schemas, logging validation errors and determining XML validity.</summary>
    public interface IXmlECH0160Validation
    {
        /// <summary>Validates the generated metadata.xml file against the provided XSD schema using a ValidationEventHandler.</summary>
        /// <param name="xmlFilePath">Path to the metadata.xml file</param>
        /// <param name="logger">Optional logger for capturing log messages</param>
        /// <returns>True if the XML is valid, False otherwise</returns>
        public bool ValidateMetadataXml(string xmlFilePath, ILogger logger = null);
    }
}