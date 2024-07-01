using IoLabs.ECH0160.Models;

namespace IoLabs.ECH0160.Conversions
{
    /// <summary>Abstract directory converter.</summary>
    public abstract class AbstractDirectoryConverter : IDirectoryConverter
    {
        /// <summary>Mark the directory as converted.</summary>
        /// <param name="currentDirectory">Directory which represents dossier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task MarkAsConverted(Dossier currentDirectory)
        {
            foreach (ArchivalRecordInput input in currentDirectory.ArchivalRecordsInputs)
            {
                input.StopProcessingRecord = true;
            }

            foreach (Dossier dossier in currentDirectory.Subfolders)
            {
                await MarkAsConverted(dossier);
            }
        }

        /// <summary>Determines if a directory is supported.</summary>
        /// <param name="directory">Directory which represents dossier</param>
        /// <returns>Boolean where the directory is supported by the converter, otherwise false</returns>
        public abstract bool IsDirectorySupported(Dossier directory);

        /// <summary>Gets the priority of the file converter.</summary>
        public abstract int Priority { get; }

        /// <summary>Identifier of the Converter.</summary>
        public abstract string Identifier { get; }

        /// <summary>Converts the directory associated with the provided archival directory.</summary>
        /// <param name="directory">Directory which represents dossier</param>
        /// <returns>A task that represents the asynchronous operation, with a result of the converted ArchivalRecordDocument</returns>
        public abstract Task<ArchivalRecordDocument> ConvertDirectory(Dossier directory);
    }
}