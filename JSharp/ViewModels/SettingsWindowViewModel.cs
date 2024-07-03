using JSharp.Utility;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.ViewModels
{
    public class SettingsWindowViewModel : BindableBase
    {
        public SettingsWindowViewModel() { }

        public IEnumerable<string> GetFileExtensionTypes()
        {
            IEnumerable<string> types = new string[] { ".bmp", ".jpg", ".jpeg", ".tiff", ".png" };
            return types;
        }
    }
}
