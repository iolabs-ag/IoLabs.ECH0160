using System.Xml;
using System.Xml.Schema;
using Microsoft.Extensions.Logging;

namespace IoLabs.ECH0160.Metadata.Validations
{
    /// <summary>Provides XML validation against an eCH-0160 XSD schema.</summary>
    public class XmlECH0160Validation : IXmlECH0160Validation
    {
        private const string XsdFilePath = "header/xsd";

        /// <summary>Validates the generated metadata.xml file against the provided XSD schema using a ValidationEventHandler.</summary>
        /// <param name="xmlFilePath">Path to the metadata.xml file</param>
        /// <param name="logger">Optional logger for capturing log messages</param>
        /// <returns>True if the XML is valid, False otherwise</returns>
        public bool ValidateMetadataXml(string xmlFilePath, ILogger logger = null)
        {
            if (!File.Exists(xmlFilePath) || !File.Exists(XsdFilePath)) // Validation of file existence
            {
                logger?.LogInformation("One or more files do not exist.");

                return false;
            }

            XmlSchemaSet schemas = new XmlSchemaSet();

            schemas.Add(null, XmlReader.Create(XsdFilePath)); // Directly add the single XSD file

            bool isValid = true;

            XmlReaderSettings settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = schemas
            };

            settings.ValidationEventHandler += (sender, e) => HandleValidationError(e, ref isValid, logger);

            try
            {
                using (XmlReader reader = XmlReader.Create(xmlFilePath, settings)) // Validate XML file
                {
                    while (reader.Read()) ; // Perform validation
                }
            }
            catch (Exception ex) when (ex is XmlSchemaValidationException or XmlException)
            {
                logger?.LogInformation("Error: {Message}", ex.Message);

                isValid = false;
            }

            return isValid;
        }

        private void HandleValidationError(ValidationEventArgs e, ref bool isValid, ILogger logger)
        {
            logger?.LogInformation("Validation Error: {Message}", e.Message);

            isValid = false;
        }
    }
}