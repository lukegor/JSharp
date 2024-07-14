using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp
{
    [Obsolete("Not in use")]
    internal class StaticStoreSingleton
    {
        private static StaticStoreSingleton _instance;

        private StaticStoreSingleton() { }

        public static StaticStoreSingleton Instance
        {
            get
            {
                // If the instance doesn't exist, create it
                if (_instance == null)
                {
                    _instance = new StaticStoreSingleton();
                }
                return _instance;
            }
        }

        public static NewImageWindow FocusedImage { get; set; }
    }
}
