using System.Xml.Linq;
using IoLabs.ECH0160.Metadata.Checksums;
using IoLabs.ECH0160.Models;
using Microsoft.Extensions.Logging;

namespace IoLabs.ECH0160.Metadata
{
    /// <summary>Generates metadata for SIP packages.</summary>
    public class MetadataGenerator : IMetadataGenerator
    {
        private readonly IHashCalculator hashCalculator;
        private ArchiveDeliveryDetails archivalDetails;

        /// <summary>Initializes a new instance of the MetadataGenerator class.</summary>
        /// <param name="hashCalculator">The hash calculator used for generating file hashes</param>
        public MetadataGenerator(IHashCalculator hashCalculator)
        {
            this.hashCalculator = hashCalculator;
        }

        /// <summary>Creates the SIP package directory structure and initial setup.</summary>
        /// <param name="processID">Project specific Id</param>
        /// <param name="baseDirectory">Base directory for the SIP package</param>
        /// <param name="deliveryType">Type of delivery for the SIP package</param>
        /// <returns>Dictionary with key paths for base, header, xsd, and content directories</returns>
        public Dictionary<string, string> GetSIPPackageMetadataXMLStructure(Guid processID, string baseDirectory, string deliveryType)
        {
            // TODO: refactor not to use string dictionary

            Dictionary<string, string> xmlValues = new Dictionary<string, string>();

            Directory.CreateDirectory(baseDirectory);

            xmlValues.Add("base", baseDirectory);
            xmlValues.Add("temp-dir", Path.Combine(baseDirectory, processID.ToString(), "temp"));

            Directory.CreateDirectory(xmlValues["temp-dir"]);

            string headerDirectory = Path.Combine(xmlValues["temp-dir"], "header");

            Directory.CreateDirectory(headerDirectory);

            xmlValues.Add("header", headerDirectory);
            xmlValues.Add("header-section", "header");
            xmlValues.Add("xsd-section", "xsd");

            string metadataPath = Path.Combine(xmlValues["header"], "metadata.xml");
            xmlValues.Add("metadata", metadataPath);

            string xsdDir = Path.Combine(headerDirectory, "xsd");

            Directory.CreateDirectory(xsdDir);

            xmlValues.Add("xsd-dir", xsdDir);

            string contentDir = Path.Combine(xmlValues["temp-dir"], "content");

            Directory.CreateDirectory(contentDir);

            xmlValues.Add("content", contentDir);
            xmlValues.Add("content-section", "content");
            xmlValues.Add("delivery", deliveryType);

            return xmlValues;
        }

        /// <summary>Generates metadata XML for files in the SIP package.</summary>
        /// <param name="headerArchivalRecords">List of header files for metadata generation</param>
        /// <param name="archivalDetails">Archival records from the institutional entity</param>
        /// <param name="structureValues">Paths for saving the metadata XML</param>
        /// <param name="sysInformation">System position for XML Nodes and it's depth related to it</param>
        public void CreateXmlMetadataFile(IList<ArchivalRecordInput> headerArchivalRecords, ArchiveDeliveryDetails archivalDetails, Dictionary<string, string> structureValues, OrdnungSystem sysInformation)
        {
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace areldaNS = "http://bar.admin.ch/arelda/v4";

            this.archivalDetails = archivalDetails;

            XDocument metadataXml = new XDocument
            (
                new XDeclaration("1.0", "UTF-8", null),
                new XElement
                (
                    areldaNS + "paket",
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                    new XAttribute(xsi + "type", "paketSIP"),
                    new XElement(areldaNS + "paketTyp", "SIP"),
                    new XAttribute("schemaVersion", "5.0"),
                    GenerateTableOfContents(sysInformation, headerArchivalRecords, areldaNS, structureValues),
                    GetDeliveryElement(sysInformation, areldaNS, xsi, structureValues)
                )
            );

            metadataXml.Save(structureValues["metadata"]); // Saves the XML document to the specified path in structureValues dictionary
        }

        /// <summary>Copies XSD files to a target directory asynchronously.</summary>
        /// <param name="targetDirectory">Target directory for the XSD files</param>
        /// <param name="logger">Logger for capturing log messages (optional)</param>
        /// <returns>A Task representing the asynchronous operation</returns>
        public async Task AddXsdSwissStandardFilesAsync(string targetDirectory, ILogger logger = null)
        {
            string projectRoot = AppDomain.CurrentDomain.BaseDirectory;
            string sourceDirectory = Path.Combine(projectRoot, "Resources", "xsd");
            string[] xsdFiles = Directory.GetFiles(sourceDirectory, "*.xsd", SearchOption.TopDirectoryOnly);

            foreach (string file in xsdFiles)
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetDirectory, fileName);

                using (FileStream sourceStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                {
                    using (FileStream destStream = new FileStream(destFile, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                    {
                        await sourceStream.CopyToAsync(destStream);
                    }
                }
            }

            logger?.LogInformation("All XSD files have been copied to: {Directory}", targetDirectory);
        }

        /// <summary>Retrieves header files for the SIP Package.</summary>
        /// <param name="xmlStructure">XML structure for the header files</param>
        /// <returns>List of archival records representing header files</returns>
        public async Task<IList<ArchivalRecordInput>> GetArchivalRecordFilesHeaderAsync(Dictionary<string, string> xmlStructure)
        {
            string projectRoot = AppDomain.CurrentDomain.BaseDirectory;
            string sourceDirectory = Path.Combine(projectRoot, "Resources", xmlStructure["xsd-section"]);
            string[] xsdFiles = Directory.GetFiles(sourceDirectory, "*.xsd");

            IEnumerable<Task<ArchivalRecordInput>> tasks = xsdFiles.Select(async file =>
            {
                FileInfo fileInfo = new FileInfo(file);

                long size = fileInfo.Length;
                string name = Path.GetFileName(file);
                string hash = await hashCalculator.CalculateHash(file);

                return new ArchivalRecordInput
                {
                    OriginalName = name,
                    Id = Guid.NewGuid(),
                    Size = size,
                    Hash = hash,
                    HashAlgorithm = hashCalculator.GetAlgorithmCode()
                };
            });

            ArchivalRecordInput[] results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        #region Private methods

        private XElement GetDossierElement(XNamespace areldaNS, Dossier dossier)
        {
            XElement dossierElement = new XElement(areldaNS + "dossier", new XAttribute("id", FormatCustomId(Guid.NewGuid())));
            XElement titelElement = new XElement(areldaNS + "titel", CleanUrlParameters(dossier.Title));

            dossierElement.Add(titelElement);

            // TODO: precalculate calculate date range for whole tree opposite way to speed up iterations
            DateRange dateRange = dossier.GetDateRange();

            XElement time = new XElement(areldaNS + "entstehungszeitraum",
                new XElement(areldaNS + "von", new XElement(areldaNS + "datum", dateRange.MinDateString)),
                new XElement(areldaNS + "bis", new XElement(areldaNS + "datum", dateRange.MaxDateString)));

            dossierElement.Add(time);

            // add subfolders
            foreach (Dossier subfolder in dossier.Subfolders.Included())
            {
                dossierElement.Add(GetDossierElement(areldaNS, subfolder));
            }

            // add documents and files
            foreach (ArchivalRecordDocument archivalRecordDocument in dossier.ArchivalRecordsDocuments.Where(d => d.DocumentFiles.Any())) // input -> document
            {
                XElement dokumentElement = new XElement(areldaNS + "dokument", new XAttribute("id", FormatCustomId(Guid.NewGuid())));
                XElement titel = new XElement(areldaNS + "titel", archivalRecordDocument.Title);
                XElement form = new XElement(areldaNS + "erscheinungsform", archivalDetails.FormAppearance);

                dokumentElement.Add(titel);
                dokumentElement.Add(form);

                if (archivalRecordDocument.AdditionalData != null)
                {
                    XElement zusatzDaten = new XElement(areldaNS + "zusatzDaten");

                    foreach (KeyValuePair<string, string> attribute in archivalRecordDocument.AdditionalData)
                    {
                        XElement merkmal = new XElement(areldaNS + "merkmal", new XAttribute("name", attribute.Key), attribute.Value);

                        zusatzDaten.Add(merkmal);
                    }

                    dokumentElement.Add(zusatzDaten);
                }

                foreach (ArchivalRecordDocumentFile archivalRecordDocumentFile in archivalRecordDocument.DocumentFiles)
                {
                    XElement dateiElement = new XElement(areldaNS + "dateiRef", FormatCustomId(archivalRecordDocumentFile.Id));

                    dokumentElement.Add(dateiElement);
                }

                dossierElement.Add(dokumentElement);
            }

            return dossierElement;
        }

        private XElement GetOrdnungSystemPositionElement(XNamespace areldaNS, OrdnungSystemPosition position)
        {
            XElement xElement = new XElement
            (
                areldaNS + "ordnungssystemposition",
                new XAttribute("id", FormatCustomId(Guid.NewGuid())), // TODO: do not genereate if set
                new XElement(areldaNS + "nummer", position.Number), // TODO: genereate if not set
                new XElement(areldaNS + "titel", position.Title)
            );

            position.Dossiers?.Included().ForEach(d => xElement.Add(GetDossierElement(areldaNS, d)));
            position.SystemPositions?.ToList().ForEach(position => xElement.Add(GetOrdnungSystemPositionElement(areldaNS, position)));

            return xElement;
        }

        private XElement GetDeliveryElement(OrdnungSystem sysInformation, XNamespace areldaNS, XNamespace xsi, Dictionary<string, string> structureValues)
        {
            XElement ablieferungElement = new XElement
            (
                areldaNS + "ablieferung",
                new XAttribute(xsi + "type", "ablieferungFilesSIP"),
                new XElement(areldaNS + "ablieferungstyp", structureValues["delivery"]),
                new XElement(areldaNS + "ablieferndeStelle", archivalDetails.DeliveringAgency),
                new XElement(areldaNS + "schutzfristenkategorie", archivalDetails.ProtectionCategory),
                new XElement(areldaNS + "schutzfrist", archivalDetails.ProtectionPeriod),
                new XElement
                (
                    areldaNS + "provenienz",
                    new XElement(areldaNS + "aktenbildnerName", archivalDetails.RecordCreatorName),
                    new XElement(areldaNS + "systemName", archivalDetails.SystemName),
                    new XElement(areldaNS + "registratur", archivalDetails.Registry)
                )
            );

            XElement ordnungssystemElement = new XElement(areldaNS + "ordnungssystem", new XElement(areldaNS + "name", sysInformation.Name));

            sysInformation.SystemPositions.ToList().ForEach(p => ordnungssystemElement.Add(GetOrdnungSystemPositionElement(areldaNS, p)));

            ablieferungElement.Add(ordnungssystemElement);

            return ablieferungElement;
        }

        private string CleanUrlParameters(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                return path.Replace("%20", " ");
            }
            else
            {
                return string.Empty;
            }
        }

        private void GenerateFolderFromDossier(Dossier dossier, XNamespace areldaNS, XElement parentElement, bool flatten = false)
        {
            XElement folderEl;

            if (!flatten)
            {
                folderEl = new XElement(areldaNS + "ordner", new XElement(areldaNS + "name", dossier.TitleAccordingToStandard), new XElement(areldaNS + "originalName", dossier.Title));

                parentElement.Add(folderEl);
            }
            else
            {
                folderEl = parentElement;
            }

            foreach (Dossier subfolder in dossier.Subfolders.Included())
            {
                GenerateFolderFromDossier(subfolder, areldaNS, folderEl);
            }

            foreach (ArchivalRecordDocument document in dossier.ArchivalRecordsDocuments)
            {
                foreach (ArchivalRecordDocumentFile documentFile in document.DocumentFiles)
                {
                    folderEl.Add
                    (
                        new XElement
                        (
                            areldaNS + "datei",
                            new XAttribute("id", FormatCustomId(documentFile.Id)),
                            new XElement(areldaNS + "name", documentFile.NameAccordingToStandard),
                            new XElement(areldaNS + "originalName", document.Title),
                            new XElement(areldaNS + "pruefalgorithmus", documentFile.HashAlgorithm),
                            new XElement(areldaNS + "pruefsumme", documentFile.Hash)
                        )
                    );
                }
            }
        }

        private XElement GenerateTableOfContents(OrdnungSystem sysInformation, IList<ArchivalRecordInput> archivalHeaderFiles, XNamespace areldaNS, Dictionary<string, string> structureValues)
        {
            // Create the root <inhaltsverzeichnis> element
            XElement inhaltsverzeichnis = new XElement(areldaNS + "inhaltsverzeichnis");

            // Add the <header> section
            XElement headerElement = new XElement(areldaNS + "ordner", new XElement(areldaNS + "name", structureValues["header-section"]), new XElement(areldaNS + "originalName", structureValues["header-section"]));
            XElement headerXsdOrdner = new XElement(areldaNS + "ordner", new XElement(areldaNS + "name", "xsd"), new XElement(areldaNS + "originalName", "xsd"));

            foreach (ArchivalRecordInput record in archivalHeaderFiles)
            {
                XElement headerFileElement = new XElement
                (
                    areldaNS + "datei",
                    new XAttribute("id", FormatCustomId(record.Id)),
                    new XElement(areldaNS + "name", record.OriginalName),
                    new XElement(areldaNS + "originalName", record.OriginalName),
                    new XElement(areldaNS + "pruefalgorithmus", record.HashAlgorithm),
                    new XElement(areldaNS + "pruefsumme", record.Hash)
                );

                headerXsdOrdner.Add(headerFileElement);
            }

            headerElement.Add(headerXsdOrdner);
            inhaltsverzeichnis.Add(headerElement);

            // Create the <content> section that will contain all other directories and files
            XElement contentElement = new XElement(areldaNS + "ordner", new XElement(areldaNS + "name", structureValues["content-section"]), new XElement(areldaNS + "originalName", structureValues["content-section"]));

            foreach (Dossier dossier in sysInformation.GetDossiersRecursive(true).Included())
            {
                GenerateFolderFromDossier(dossier, areldaNS, contentElement, false);
            }

            inhaltsverzeichnis.Add(contentElement); // Add the <content> section to the <inhaltsverzeichnis>

            return inhaltsverzeichnis;
        }

        private string FormatCustomId(Guid id)
        {
            byte[] bytes = id.ToByteArray();
            string encodedId = Convert.ToBase64String(bytes).Trim('=').Replace('+', '-').Replace('/', '_');

            return "_" + encodedId;
        }

        #endregion Private methods
    }
}