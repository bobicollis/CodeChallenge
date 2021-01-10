using System.Text.RegularExpressions;

namespace CodeChallenge.Utils
{
    /// <summary>
    /// Accepts a six or eight byte hex value, expecting alpha channel to be last byte.
    /// An empty value is converted to transparent.
    /// </summary>
    public static class ColourParser
    {
        private const string TRANSPARENT = @"00000000";
        private static readonly Regex hexRegex;

        static ColourParser()
        {
            //hexRegex = new Regex("^#?(([0-9a-fA-F]{2}){3,4}|([0-9a-fA-F]){3,4})$");
            hexRegex = new Regex("^#?(([0-9a-fA-F]{2}){3,4})$");
        }

        public static string Parse(string colour)
        {
            if (string.IsNullOrEmpty(colour)) return TRANSPARENT;
            if (hexRegex.IsMatch(colour)) return colour.ToUpper();
            if (colour.Length == 6) return $"{colour}00";
            return TRANSPARENT;
        }
    }
}
