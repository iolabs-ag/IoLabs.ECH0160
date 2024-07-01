namespace IoLabs.ECH0160.Metadata.Checksums
{
    /// <summary>Defines the contract for hash calculation operations.</summary>
    public interface IHashCalculator
    {
        /// <summary>Asynchronously calculates SHA-256 hash of a file stream.</summary>
        /// <param name="filePath">The file to calculate hash for</param>
        /// <returns>Hexadecimal string representation of the hash</returns>
        public Task<string> CalculateHash(string filePath);

        /// <summary>Returns the code of the hashing algorithm used.</summary>
        /// <returns>The algorithm code as a string</returns>
        public string GetAlgorithmCode();
    }
}