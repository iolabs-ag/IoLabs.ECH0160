﻿# eCH-0160 Archival Standard Implementation

## Overview
The README provides a comprehensive overview of a project that implements the eCH-0160 standard for digital archival. The project is designed to generate Submission Information Packages (SIPs) from a directory of files, adhering strictly to the eCH-0160 standard. This ensures the generated SIPs are compliant with the requirements of digital preservation and are compatible with archival systems that support the eCH-0160 standard. 

## Features
- **eCH-0160 Standard Compliance**: The project strictly adheres to the eCH-0160 standard, a Swiss standard for the archival interface. This ensures that the Submission Information Packages (SIPs) generated by the project are compliant with the requirements of digital preservation. The standard outlines a methodology for the archival interface, including the structure of the SIPs, the metadata to be included, and the file formats to be used. By complying with this standard, the project ensures that the SIPs it generates are suitable for long-term preservation and are compatible with archival systems that support the eCH-0160 standard.
- **SIP Generation**: The core feature of this project is the generation of Submission Information Packages (SIPs) from a directory of files. This is done in accordance with the eCH-0160 standard, ensuring that the generated SIPs are compliant and can be used for digital preservation purposes.  
- **XML Metadata Support**: The project supports the inclusion of XML metadata in the SIPs. This allows for rich, structured information about the archived files to be included in the package, enhancing the utility and accessibility of the archived data.  
- **Extensibility**: The project is designed with extensibility in mind. It uses interfaces to allow for custom implementations of key components. This means that developers can extend and customize the functionality to suit their specific needs.  
- **Custom Converters**: The project includes the ability to write custom converters. These converters can be used to transform files during the archival process, for example, converting image files to a standard format, or extracting text from PDFs.  
- **Dependency Injection Configuration**: The project provides extension methods for easy configuration using dependency injection. This simplifies the setup process and makes it easier to integrate the project into existing applications.  
- **Manual Configuration**: For developers who prefer to have more control over the configuration, the project also supports manual configuration. This allows for more fine-grained control over the setup and operation of the project.  
- **Comprehensive Documentation**: The project includes comprehensive documentation, including a detailed README and inline comments in the code. This makes it easier for developers to understand how to use and extend the project.

## Getting Started
Clone or download this repository and add into your project.

## Integration
Main class which is responsible for the package generation is the `AdvancedSIPGenerator` class. 

### Configuring using Dependency Injection
To simplify the usage of the library, you can register the required services using the `AddEch0160` and optionally `AddEch0160BasicConverters` extension methods.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddEch0160()
        .AddEch0160BasicConverters();
}
```
Then inject the `ISipGenerator` interface into your class:

```csharp
public class YourClass
{
    private readonly ISipGenerator sipGenerator;

    public YourClass(ISipGenerator sipGenerator)
    {
        this.sipGenerator = sipGenerator;
    }
}
```

### Manual Configuration
Without the extension methods, you can register the services manually:

```csharp
// create list of converter instances
IList<IFileConverter> fileConverters = new List<IFileConverter>
{
    new SimpleCopyConverter()
};

// create instance of the FilesConversionManager
IFilesConversionManager filesConversionManager = new FilesConversionManager(fileConverters);

// create instance of the PathNormalizer
IPathNormalizer pathNormalizer = new PathNormalizer();

// create instance of the HashCalculator    
IHashCalculator hashCalculator = new SHA256HashCalculator();

// create instance of the MetadataGenerator    
IMetadataGenerator metadataGenerator = new MetadataGenerator(hashCalculator);

// create instance of the SipGenerator
IAdvancedSIPGenerator sipGenerator = new AdvancedSIPGenerator(filesConversionManager, pathNormalizer, hashCalculator, metadataGenerator);
``` 

## Usage 

### Key Components
| Class | Description |
--------|-------------|
| `AdvancedSipGenerator` | This is the main class responsible for generating Submission Information Packages (SIPs) according to the eCH-0160 standard. It provides a method CreateSIPPackage which takes in details of the archive delivery, a unique process ID, system information, base directory, and directory for converted files. It also accepts optional parameters for actions to be executed after the conversion of a record is done and after the name normalization of a record is done.|  
| `ArchivalRecordInput` | This class represents a file that has been downloaded as part of the archivation process. It contains properties such as the original name of the file, the full path to the source file, the hash of the file, and additional data for the file. It also provides methods to clone the instance and to check if the file has a specific extension.|  
| `OrdnungSystem` | This class represents the basic SIP structure. It provides methods to add an OrdnungSystemPosition and a Dossier to the system. The OrdnungSystemPosition represents a position in the system, while a Dossier represents a folder in the system that contains files.|  
| `ArchiveDeliveryDetails` | This class represents the details of the archive delivery. It contains properties such as the delivering agency, reference, protection category, protection period, record creator name, system name, registry, form appearance, and others.|

SIP generation is done by calling the `CreateSIPPackage` method on the `IAdvancedSipGenerator` interface.

```csharp
/// <summary>Generates a SIP package structure as per the eCH-0160 standard.</summary>
public interface IAdvancedSIPGenerator
{
    /// <summary>Creates a SIP package for the specified archive delivery.</summary>
    /// <param name="archivalDetails">Details of the archive delivery</param>
    /// <param name="temporaryProcessID">Project specific Id</param>
    /// <param name="sysInformation">System position for XML Nodes and it's depth related to it</param>
    /// <param name="baseDirectory">Base directory for the SIP package</param>
    /// <param name="convertedFilesDirectory">Directory for the converted files</param>
    /// <param name="onRecordConversionPassed">Action to be executed after the conversion of a record is done</param>
    /// <param name="onRecordNameNormalizationDone">Action to be executed after the name normalization of a record is done</param>
    /// <returns>Sip Package</returns>
    public Task<SipPackage> CreateSIPPackage(ArchiveDeliveryDetails archivalDetails,
                                                Guid temporaryProcessID,
                                                OrdnungSystem sysInformation,
                                                string baseDirectory,
                                                string convertedFilesDirectory,
                                                Func<ArchivalRecordDocument, ArchivalRecordInput?, Task>? onRecordConversionPassed = null,
                                                Func<ArchivalRecordDocument, Task>? onRecordNameNormalizationDone = null
                                                );
}
```
### Example usage:
```csharp
// create processID
Guid processID = Guid.NewGuid();    
    
// prepare input file list eg by reading the files from a directory
// simple sample    
IList<ArchivalRecordInput> archFilesResult = new List<ArchivalRecordInput>
{
    new ArchivalRecordInput()
    {
        OriginalName = "test.txt",
        CreationDate = new DateTime(2024, 06, 17, 11, 03, 46),
        LastUpdateDate = new DateTime(2024, 06, 17, 11, 07, 24),
        FullPathWithFileName = $"/SIP_generator/{processID}/downloaded_files/test.txt",
        DirectoryRelativePath = "",
        AdditionalData = new Dictionary<string, string> { { "account", "159753" }, { "projectName", "My Project" }, }
    },
    new ArchivalRecordInput()
    {
        OriginalName = "my document.docx",
        CreationDate = new DateTime(2024, 06, 18, 12, 33, 02),
        LastUpdateDate = new DateTime(2024, 06, 18, 12, 37, 27),
        FullPathWithFileName = $"/SIP_generator/{processID}/downloaded_files/My Folder/my document.docx",
        DirectoryRelativePath = "My Folder/",
        AdditionalData = new Dictionary<string, string> { { "account", "159753" }, { "projectName", "My Project" }, }
    }
};

// create basic SIP structure
OrdnungSystem ordnungSystem = OrdnungSystem.Create("Ordnungssystem Sharepoint");
ordnungSystem
    .AddOrdnungSystemPosition("Ordnungsystem title, "01")
    .AddDossier("Folder name", archFilesResult);

// prepare delivery details
ArchiveDeliveryDetails deliveryDetails = new ArchiveDeliveryDetails
{
    DeliveringAgency = "Delivery Agency",
    DeliveringAgencyShort = "DA",
    Reference = "SIP Reference",
    ProtectionCategory = "20 Protection Category",
    ProtectionPeriod = 20,
    RecordCreatorName = "Record Creator",
    SystemName = "System Name",
    Registry = "2024",
    FormAppearance = "digital"
};

// Create a SIP package
SipPackage sipPackage = await sipGenerator.CreateSIPPackage(
    deliveryDetails,
    processID, // process instance unique ID
    ordnungSystem,
    "/temp/path/to/working/folder", // absolute path to the working folder
    "converted_files", // relative path to the converted files inside project temp directory
    async (ArchivalRecordDocument archivalRecord, ArchivalRecordInput? input) =>
    {
        // handle onConversionDone event
        // eg log process
    }, async (ArchivalRecordDocument archivalRecord) =>
    {
        // handle onPathsNormalized
        // eg log process
    });
```

### Result
The generated SIP package is prepared in the temp folder and can be accessed using 
```csharp
string packageName = sipPackage.SipPackageName;
string packagePath = sipPackage.SipPackagePath;
```

## Converters
Converters in this context are classes that transform an `ArchivalRecordInput` into an `ArchivalRecordDocument``. They play a crucial role in the archival process, as they handle the conversion of files during the archival process. Here's a brief description:  

- **Input**: The converter accepts an instance of ArchivalRecordInput as input. This class represents a file that has been downloaded as part of the archivation process. It contains properties such as the original name of the file and the full path to the source file.  
- **Output**: The converter returns an instance of ArchivalRecordDocument or null. If the file should not be archived, the converter returns null. Otherwise, it returns an ArchivalRecordDocument which represents the converted file.  
- **ArchivalRecordDocumentFile**: The ArchivalRecordDocument can contain one or more instances of ArchivalRecordDocumentFile. This class represents a file in the document.  

In summary, converters are responsible for transforming an ArchivalRecordInput into an ArchivalRecordDocument which can contain one or more ArchivalRecordDocumentFile instances. They play a crucial role in the archival process, ensuring that files are correctly converted and prepared for archival.

### Bundled Converters
The only bundled converter is the `SimpleCopyConverter`, which copies the files to the output directory without any modifications.

### Creating a Custom Converter
Additional converters can be created by implementing the `IFileConverter` interface, or the abstract `ExtensionBasedFileConverter` class. 

Registering using the `AddEch0160Converter<T>()` extension method:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // ...
    services.AddEch0160Converter<ImageConverter>();
    // ...
}
```

See the ImageConverter example below:
```csharp
using System.IO;
using ImageMagick;
using ImageMagick.Formats;
using IoLabs.eCH0160Standard.Conversions;
using IoLabs.eCH0160Standard.Models;

namespace IoLabs.eCH0160Standard.Converters
{
    
    public class ImageConverter : ExtensionBasedFileConverter
    {
        /// <summary>The list of supported extensions.</summary>
        protected override List<string> SupportedExtensions
        {
            get
            {
                return new List<string> { "bmp", "ico", "gif" };
            }
        }
    
        /// <summary>Converter's identifier.</summary>
        public override string Identifier
        {
            get
            {
                return "Images";
            }
        }
    
        /// <summary>ImageConverter Constructor.</summary>
        /// <exception cref="ArgumentNullException">Exception</exception>
        public ImageFilesConverter()
        {
        }
    
        /// <summary>Gets the priority of the image file converter.</summary>
        public override int Priority
        {
            get
            {
                return 100;
            }
        }
    
        public async Task<ArchivalRecordDocument> ConvertAsync(ArchivalRecordInput input, string outputPath)
        {
            archivalRecordDocument doc = ArchivalRecordDocument.FromArchivalRecordInput(
                recordInput
            );
    
            // Perform the conversion
            string newName = Path.GetFileNameWithoutExtension(recordInput.FullPathWithFileName) + ".tiff";
            string newFilePath = Path.Combine(recordInput.DirectoryConversionPath, newName);
    
            MagickReadSettings settings = new MagickReadSettings(new BmpReadDefines
            {
                IgnoreFileSize = true,
            });
    
            // Load the image from file
            using (MagickImage image = new MagickImage(recordInput.FullPathWithFileName, settings))
            {
                // Set the output format
                image.Format = MagickFormat.Tiff;
    
                // Save the image to a file
                await image.WriteAsync(newFilePath);
            }
    
            ArchivalRecordDocumentFile file = new ArchivalRecordDocumentFile()
            {
                TempFileName = newName,
                DirectoryConversionPath = recordInput.DirectoryConversionPath,
                IsConverted = true,
            };
            doc.DocumentFiles.Add(file);
    
            return doc;
        }
    }
}    
```


## Development
Contributing to this project and developing new features involves a series of steps that ensure code quality and consistency across the project. Here's a general guide on how you can contribute:  

1. Fork the Repository: The first step to contributing is to fork the repository. This creates a copy of the project in your own GitHub account, allowing you to make changes without affecting the original project.  
2. Clone the Repository: After forking, you need to clone the repository to your local machine. This allows you to work on the project locally. You can do this by running the command git clone https://github.com/<your-username>/<repository-name>.git.  
3. Create a New Branch: It's a good practice to create a new branch for each new feature or bug fix you're working on. This keeps your changes organized and separated from the main project. You can create a new branch with git checkout -b <branch-name>.  
4. Make Your Changes: Now you can start making changes to the project. Make sure to follow the project's coding standards and conventions.  
5. Commit Your Changes: After making your changes, you need to commit them. This involves telling Git which changes you want to save. You can do this with git add . to stage all changes, and then git commit -m "commit message" to commit them.  
6. Push Your Changes: After committing your changes, you need to push them to your forked repository on GitHub. You can do this with git push origin <branch-name>.  
7. Open a Pull Request: Once your changes are pushed, you can open a pull request in the original repository. This lets the project maintainers know that you have changes you want them to review.  
8. Address Review Comments: If the project maintainers have any feedback or requests, you'll need to address these in your branch and update your pull request.  
9. Merge: Once your changes have been reviewed and approved, they can be merged into the main project.  

## Known Issues and Limitations
- only covers FILES generation and 
- only covers single specific usecase, feel free to extend it

## TODOs
- release as a nuget package
- add more documentation
- add more examples
- simplify the usage
- simplify paths handling
- cover whole eCH-0160 standard
- add more converters
- add more tests

## Disclaimer

This library is provided "as is", without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose and noninfringement. In no event shall the authors or copyright holders be liable for any claim, damages or other liability, whether in an action of contract, tort or otherwise, arising from, out of or in connection with the library or the use or other dealings in the library.

## License
TBD