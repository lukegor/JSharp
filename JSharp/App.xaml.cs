using System.Diagnostics;
using System.Globalization;
using System.Windows;
using JSharp.UI.Views;
using JSharp.Utility;
using JSharp.ViewModels;
using JSharp.Views;
using JSharp.Views.Properties;

namespace JSharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public LanguageDictionary Languages = new LanguageDictionary();
        protected override void OnStartup(StartupEventArgs e)
        {
            CultureInfo culture;
//#if DEBUG
//            culture = CultureInfo.InvariantCulture; // Force invariant culture for debugging
//#else
            if (!string.IsNullOrEmpty(Settings.Default.LanguageVersion) && Languages.ContainsKey(Settings.Default.LanguageVersion))
            {
                culture = Languages[key: Settings.Default.LanguageVersion];
            }
            else culture = CultureInfo.CurrentCulture; // Use the current culture in release mode
//#endif
            // Set default culture for the application - seems unnecessary
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // Set current culture for the current thread - NECESSARY
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

            base.OnStartup(e);

            LaunchGUI();
        }

        private void LaunchGUI()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        internal void Restart()
        {
            var currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(currentExecutablePath);
            Application.Current.Shutdown();
        }
    }

}
