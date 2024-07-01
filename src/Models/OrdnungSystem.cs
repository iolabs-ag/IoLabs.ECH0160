namespace IoLabs.ECH0160.Models
{
    /// <summary>Represents a system in the archival record.</summary>
    public class OrdnungSystem
    {
        /// <summary>Name of the system.</summary>
        public string Name { get; set; }

        /// <summary>System positions for the system.</summary>
        public IList<OrdnungSystemPosition> SystemPositions { get; set; } = new List<OrdnungSystemPosition>();

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

        /// <summary>Creates a new OrdnungSystem.</summary>
        /// <param name="name">Name of the system</param>
        /// <returns>The new OrdnungSystem</returns>
        public static OrdnungSystem Create(string name)
        {
            return new OrdnungSystem
            {
                Name = name,
            };
        }

        /// <summary>Get dossiers recursively.</summary>
        /// <param name="topLevelOnly">Flag indicating whether to get only top level dossiers</param>
        /// <returns>List of dossiers</returns>
        public List<Dossier> GetDossiersRecursive(bool topLevelOnly = true)
        {
            List<Dossier> dossiers = new List<Dossier>();

            foreach (OrdnungSystemPosition position in SystemPositions)
            {
                dossiers.AddRange(position.GetDossiersRecursive(topLevelOnly));
            }

            return dossiers;
        }
    }
}