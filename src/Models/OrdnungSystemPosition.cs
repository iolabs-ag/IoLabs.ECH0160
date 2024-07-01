namespace IoLabs.ECH0160.Models
{
    /// <summary>Represents a system position in the archival record.</summary>
    public class OrdnungSystemPosition
    {
        /// <summary>Number for the system position.</summary>
        public string Number { get; set; }

        /// <summary>Title for the system position.</summary>
        public string Title { get; set; }

        /// <summary>Subpositions for the system position.</summary>
        public IList<OrdnungSystemPosition> SystemPositions { get; set; } = new List<OrdnungSystemPosition>();

        /// <summary>Dossiers.</summary>
        public IList<Dossier> Dossiers { get; set; } = new List<Dossier>();

        /// <summary>Adds a new system position to the system.</summary>
        /// <param name="title">Title for the system position</param>
        /// <param name="number">Number for the system position</param>
        /// <returns>The new system position</returns>
        public OrdnungSystemPosition AddOrdnungSystemPosition(string title, string number)
        {
            OrdnungSystemPosition position = new OrdnungSystemPosition
            {
                Title = title,
                Number = number
            };

            SystemPositions.Add(position);

            return position;
        }

        /// <summary>Adds a new dossier to the system position.</summary>
        /// <param name="title">Title for the dossier</param>
        /// <param name="archivalRecords">Archival records for the dossier</param>
        /// <returns>The new dossier</returns>
        public Dossier AddDossier(string title, IEnumerable<ArchivalRecordInput> archivalRecords)
        {
            Dossier dossier = new Dossier
            {
                Title = title,
                ArchivalRecordsInputs = archivalRecords.ToList(),
                Path = title
            };

            Dossiers.Add(dossier);

            return dossier;
        }

        /// <summary>Get the first dossier in the system position.</summary>
        /// <returns>The first dossier in the system position</returns>
        public Dossier GetFirstDossier()
        {
            if (Dossiers?.Count > 0)
            {
                return Dossiers.First();
            }

            foreach (OrdnungSystemPosition subPosition in SystemPositions) // Iterate children, depth-first
            {
                Dossier subDossier = subPosition.GetFirstDossier();

                if (subDossier != null)
                {
                    return subDossier;
                }
            }

            return null;
        }

        /// <summary>Get dossiers recursively.</summary>
        /// <param name="topLevelOnly">Flag indicating whether to get only top level dossiers</param>
        /// <returns>List of dossiers</returns>
        public IEnumerable<Dossier> GetDossiersRecursive(bool topLevelOnly = true)
        {
            List<Dossier> dossiers = Dossiers.ToList();

            if (!topLevelOnly)
            {
                dossiers.AddRange(Dossiers.SelectMany(d => d.GetDossiersRecursive()));
            }

            foreach (OrdnungSystemPosition position in SystemPositions)
            {
                dossiers.AddRange(position.GetDossiersRecursive(topLevelOnly));
            }

            return dossiers;
        }
    }
}