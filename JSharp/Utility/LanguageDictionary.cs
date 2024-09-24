using System.Globalization;

namespace JSharp.Utility
{
    public class LanguageDictionary : Dictionary<string, CultureInfo>
    {
        public static IEnumerable<string> KeysList => new LanguageDictionary().Keys;
        public LanguageDictionary()
        {
            Add("English", new CultureInfo("en"));
            Add("Deutsch", new CultureInfo("de"));
            Add("polski", new CultureInfo("pl"));
        }
    }
}
