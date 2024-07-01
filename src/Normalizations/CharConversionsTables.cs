namespace IoLabs.ECH0160.Normalizations
{
    /// <summary>Provides mappings for character conversions from Codepage-1252 and ISO-8859 to ASCII.</summary>
    public static class CharConversionsTables
    {
        /*
           Normalization Codepage-1252, ISO-8859 and Unicode in the range 0x80-0x9F (U+0080 - U+009F)

           Codepage-1252 is called Western European. It differs from ISO-8859-1 in the range 0x80-
           9F, whose 32 positions here contain 27 displayable characters, including those added in ISO
           8859-15 and some necessary for better typography. For the non-representable characters,
           the Symbol field is left empty in the table below. The differences between all these
           encodings, and the general lack of consistency in supporting different character sets, is a
           common interoperability problem.

           The characters (0x80..0x9F or U+0080..U+009F) are control lines in ISO-8859 and in
           Unicode. They are illegal in file names and are not considered for normalization. If they occur
           anyway, they have to be eliminated and an error message has to be generated.

           The following table contains the conversion of the affected characters into visually similar US ASCII characters
         */

        /// <summary>Contains the mapping for characters from Codepage-1252 (0x80-0x9F) to their ASCII equivalents.</summary>
        public static Dictionary<char, string> ReplacementMap1252 = new Dictionary<char, string>
        {
            {'\u0080', "E="},
            {'\u0081', string.Empty}, // Assuming this symbol should be removed as there's no ASCII equivalent in the documentation
            {'\u0082', "'"},
            {'\u0083', "f"},
            {'\u0084', "'"},
            {'\u0085', "..."},
            {'\u0086', "-"},
            {'\u0087', "-"},
            {'\u0088', "-"},
            {'\u0089', "%0"},
            {'\u008A', "S"},
            {'\u008B', "'"},
            {'\u008C', "OE"},
            {'\u008D', "-"},
            {'\u008E', "Z"},
            {'\u008F', "-"},
            {'\u0090', "-"},
            {'\u0091', "'"},
            {'\u0092', "'"},
            {'\u0093', "'"},
            {'\u0094', "'"},
            {'\u0095', "-"},
            {'\u0096', "-"},
            {'\u0097', "--"},
            {'\u0098', "~"},
            {'\u0099', "TM"},
            {'\u009A', "s"},
            {'\u009B', "'"},
            {'\u009C', "oe"},
            {'\u009D', "-"},
            {'\u009E', "Z"},
            {'\u009F', "Y"}
        };

        /*
           Normalization Codepage-1252, ISO-8859 and Unicode in the range 0xA0-0xFF (U+00A0 - U+00FF)

           This and the following mappings map characters from UTF-8 (subset) and ISO-8859 that are not in the US ASCII range to visually similar US ASCII characters.
           The relevant and allowed code pages of the ISO-8859 standard are: Code Page 1 Latin-1, Western European, Code Page 15 Latin-9, Western European
           The following table contains the mapping of ISO-8859-1 characters and Unicode characters outside the US-ASCII range (0xA0 - 0xFF) into visually similar US-ASCII characters or strings, taking into account the restrictions of section E.2.2.1 above.
           This excerpt discusses the normalization of character sets from ISO-8859-1 and Unicode to their nearest ASCII representations, within a specified range
        */

        /// <summary>Contains the mapping for characters from ISO-8859 (0xA0-0xFF) to their ASCII equivalents.</summary>
        public static Dictionary<char, string> ReplacementMap8859 = new Dictionary<char, string>
        {
            { '\u00A0', "SP"},
            { '\u00A1', "-"},
            { '\u00A2', "c"},
            { '\u00A3', "L="},
            { '\u00A4', "I="},
            { '\u00A5', "Y="},
            { '\u00A6', "-"},
            { '\u00A7', "SS"},
            { '\u00A8', "-"},
            { '\u00A9', "(c)"},
            { '\u00AA', "a"},
            { '\u00AB', "-"},
            { '\u00AC', "-"},
            { '\u00AD', "-"},
            { '\u00AE', "(r)"},
            { '\u00AF', "-"},
            { '\u00B0', "deg"},
            { '\u00B1', "+-"},
            { '\u00B2', "2"},
            { '\u00B3', "3"},
            { '\u00B4', "'"},
            { '\u00B5', "u"},
            { '\u00B6', "P"},
            { '\u00B7', "."},
            { '\u00B8', ","},
            { '\u00B9', "1"},
            { '\u00BA', "o"},
            { '\u00BB', "-"},
            { '\u00BC', "_"},
            { '\u00BE', "_"},
            { '\u00BF', "_"},
            { '\u00C0', "A"},
            { '\u00C1', "A"},
            { '\u00C2', "A"},
            { '\u00C3', "A"},
            { '\u00C4', "Ae"},
            { '\u00C5', "A"},
            { '\u00C6', "AE"},
            { '\u00C7', "C"},
            { '\u00C8', "E"},
            { '\u00C9', "E"},
            { '\u00CA', "E"},
            { '\u00CB', "E"},
            { '\u00CC', "I"},
            { '\u00CD', "I"},
            { '\u00CE', "I"},
            { '\u00CF', "I"},
            { '\u00D0', "D"},
            { '\u00D1', "N"},
            { '\u00D2', "O"},
            { '\u00D3', "O"},
            { '\u00D4', "O"},
            { '\u00D5', "O"},
            { '\u00D6', "Oe"},
            { '\u00D7', "x"},
            { '\u00D8', "O"},
            { '\u00D9', "U"},
            { '\u00DA', "U"},
            { '\u00DB', "U"},
            { '\u00DC', "Ue"},
            { '\u00DD', "Y"},
            { '\u00DE', "Th"},
            { '\u00DF', "ss"},
            { '\u00E0', "a"},
            { '\u00E1', "a"},
            { '\u00E2', "a"},
            { '\u00E3', "a"},
            { '\u00E4', "ae"},
            { '\u00E5', "a"},
            { '\u00E6', "ae"},
            { '\u00E7', "c"},
            { '\u00E8', "e"},
            { '\u00E9', "e"},
            { '\u00EA', "e"},
            { '\u00EB', "e"},
            { '\u00EC', "i"},
            { '\u00ED', "i"},
            { '\u00EE', "i"},
            { '\u00EF', "i"},
            { '\u00F0', "d"},
            { '\u00F1', "n"},
            { '\u00F2', "o"},
            { '\u00F3', "o"},
            { '\u00F4', "o"},
            { '\u00F5', "o"},
            { '\u00F6', "oe"},
            { '\u00F7', "/"},
            { '\u00F8', "o"},
            { '\u00FC', "ue"},
            { '\u00FD', "y"},
            { '\u00FE', "th"},
            { '\u00FF', "y"},
            { '\u00F9', "u"},
            { '\u00FA', "u"},
            { '\u00FB', "u"},
            { '\u20AC', "E="},
            { '\u2020', "-"},
            { '\u2030', "-"},
        };

        /*
            E.2.3 Normalization US-ASCII in the range 0x20-0x7E
            The control characters (0x00..0x1F, 0x7F) are illegal in file names and are not considered further for standardization.
            If they occur nevertheless, they are to be eliminated and an error message is to be generated. Characters not allowed in file names
            (see chapter E.2.2.2) are mapped to "_" during normalization, allowed characters remain the same:
        */

        /// <summary>Contains the mapping for characters from ISO-8859 (0xA0-0xFF) to their ASCII equivalents.</summary>
        public static Dictionary<char, string> ReplacementMapUsAscii = new Dictionary<char, string>
        {
            {'"', "_"},
            {'*', "_"},
            {'/', "_"},
            {':', "_"},
            {'<', "_"},
            {'>', "_"},
            {'?', "_"},
            {'\\', "_"},
            {'|', "_"}
        };
    }
}