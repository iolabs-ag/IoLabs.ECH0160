using System.Security.Cryptography;

namespace IoLabs.ECH0160.Metadata.Checksums
{
    /// <summary>Calculates SHA-256 hash for given file stream.</summary>
    public class SHA256HashCalculator : IHashCalculator
    {
        /// <summary>Asynchronously calculates SHA-256 hash of a file stream.</summary>
        /// <param name="filePath">The file to calculate hash for</param>
        /// <returns>Hexadecimal string representation of the hash</returns>
        public async Task<string> CalculateHash(string filePath)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    byte[] hash = await sha256.ComputeHashAsync(fileStream);

                    return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
                }
            }
        }

        /// <summary>Returns the code of the hashing algorithm used.</summary>
        /// <returns>Algorithm code as a string</returns>
        public string GetAlgorithmCode()
        {
            return "SHA-256";
        }
    }
}