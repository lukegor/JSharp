using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
