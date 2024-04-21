using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Models
{
    public class AnalysisSettings
    {
        public int SizeFrom { get; set; }
        // null means infinity
        public int? SizeTo { get; set; }
        public AnalysisSettings()
        {
            SizeFrom = 0;
            SizeTo = null;
        }

        public AnalysisSettings(int sizeFrom, int? sizeTo)
        {
            SizeFrom = sizeFrom;
            SizeTo = sizeTo;
        }
    }
}
