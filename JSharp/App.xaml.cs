using JSharp.Properties;
using JSharp.Utility;
using JSharp.ViewModels;
using Prism;
using Prism.Ioc;
using Prism.Regions;
using Prism.Unity;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace JSharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
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
        }
        #region Prism
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<MainWindowViewModel>();
            containerRegistry.RegisterScoped<NewImageWindowViewModel>();
            containerRegistry.RegisterInstance(StaticStoreSingleton.Instance);
        }
        #endregion
        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            // Configure region adapters if necessary
        }

        public void Restart()
        {
            var currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(currentExecutablePath);
            Application.Current.Shutdown();
        }
    }

}
