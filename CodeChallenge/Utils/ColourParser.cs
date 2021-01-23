using System.Text.RegularExpressions;

namespace CodeChallenge.Utils
{
    public static class ColourParser
    {
        private const string TRANSPARENT = @"00000000";
        private static readonly Regex hexRegex;

        static ColourParser()
        {
            hexRegex = new Regex("^#?(([0-9a-fA-F]{2}){3,4})$");
        }

        /// <summary>
        /// Accepts a six or eight character hex value, expecting alpha channel to be last two characters if present.
        /// An empty or invalid value is converted to transparent.
        /// </summary>
        public static string ParseHexColour(string colour)
        {
            if (string.IsNullOrEmpty(colour)) return TRANSPARENT;
            if (!hexRegex.IsMatch(colour)) return TRANSPARENT;
            if (colour.Length == 8) return colour.ToUpper();
            if (colour.Length == 6) return $"{colour.ToUpper()}FF";
            return TRANSPARENT; // Should be unreachable, but better to fail gracefully...
        }
    }
}
