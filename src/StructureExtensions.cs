using IoLabs.ECH0160.Models;

namespace IoLabs.ECH0160
{
    /// <summary>Extensions for the structure.</summary>
    public static class StructureExtensions
    {
        /// <summary>Filters the dossiers to include only those that are not excluded.</summary>
        /// <param name="dossiers">List of dossiers</param>
        /// <returns>List of included dossiers</returns>
        public static List<Dossier> Included(this IList<Dossier> dossiers)
        {
            return dossiers.Where(d => !d.IsExcluded && d.ContainsFiles()).ToList();
        }
    }
}