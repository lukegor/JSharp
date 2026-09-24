using JSharp.Domain.Abstractions;
using JSharp.Configuration;
using JSharp.Domain.Abstractions.Validation;
using JSharp.Domain.Operations;
using JSharp.Services;
using JSharp.Properties;
using JSharp.Shared.Pro;
using JSharp.Shared.Resources;
using JSharp.UI.Views;
using JSharp.Utility.Utility;
using JSharp.ViewModels;
using JSharp.ViewModels.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace JSharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// Main entry point of the program
    /// </summary>
    public partial class App : Application
    {
        public LanguageDictionary Languages { get; set; } = new LanguageDictionary();
        private IServiceProvider _serviceProvider = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            _serviceProvider = ConfigureServices();

            InstallGlobalExceptionHandlers(_serviceProvider.GetRequiredService<ILogger<App>>());

            InitializeCulture();

            base.OnStartup(e);

            LaunchGUI();
        }

        private void InstallGlobalExceptionHandlers(ILogger<App> logger)
        {
            DispatcherUnhandledException += (_, args) =>
            {
                logger.LogError(args.Exception, "Unhandled exception on the UI thread.");
                ShowFatalErrorDialog(args.Exception);
                args.Handled = true;
            };

            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            {
                Exception exception = args.ExceptionObject as Exception
                    ?? new InvalidOperationException($"Non-exception failure object: {args.ExceptionObject}");
                logger.LogCritical(exception, "Fatal exception off the UI thread.");
            };

            TaskScheduler.UnobservedTaskException += (_, args) =>
            {
                logger.LogError(args.Exception, "Unobserved task exception.");
                args.SetObserved();
            };
        }

        private static void ShowFatalErrorDialog(Exception exception)
        {
            try
            {
                string details = exception.ToString();

                Window dialog = new()
                {
                    Title = "JSharp",
                    Width = 640,
                    MinWidth = 480,
                    Height = 440,
                    MinHeight = 340,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                TextBox detailsBox = new()
                {
                    Text = details,
                    IsReadOnly = true,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    AcceptsReturn = true
                };

                Button copyButton = new() { Content = Strings.CopyDetails, Padding = new Thickness(12, 4, 12, 4) };
                copyButton.Click += (_, _) => Clipboard.SetText(details);

                Button openLogsButton = new() { Content = Strings.OpenLogsFolder, Padding = new Thickness(12, 4, 12, 4) };
                openLogsButton.Click += (_, _) => OpenLogsFolder();

                Button closeButton = new() { Content = Strings.Close, Padding = new Thickness(12, 4, 12, 4), IsCancel = true };
                closeButton.Click += (_, _) => dialog.Close();

                StackPanel buttonPanel = new()
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                buttonPanel.Children.Add(copyButton);
                buttonPanel.Children.Add(openLogsButton);
                buttonPanel.Children.Add(closeButton);

                Grid root = new() { Margin = new Thickness(12) };
                root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                TextBlock messageText = new()
                {
                    Text = Messages.FatalErrorMessage,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 8)
                };
                Grid.SetRow(messageText, 0);
                root.Children.Add(messageText);

                Grid.SetRow(detailsBox, 1);
                root.Children.Add(detailsBox);

                Grid.SetRow(buttonPanel, 2);
                buttonPanel.Margin = new Thickness(0, 8, 0, 0);
                root.Children.Add(buttonPanel);

                dialog.Content = root;
                dialog.ShowDialog();
            }
            catch (Exception handlerException)
            {
                Trace.TraceError($"Fatal error dialog failed: {handlerException}");
            }
        }

        private static void OpenLogsFolder()
        {
            if (!Directory.Exists(LogPaths.Directory))
            {
                return;
            }

            Process.Start(new ProcessStartInfo { FileName = LogPaths.Directory, UseShellExecute = true });
        }

        private void LaunchGUI()
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            var mainVm = _serviceProvider.GetRequiredService<MainWindowViewModel>();
#if !JSHARP_OPEN
            mainVm.Recorder = _serviceProvider.GetRequiredService<RecipeRecorder>();
#endif
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.CloseAndFlush();
            base.OnExit(e);
        }

        internal static void Restart()
        {
            var currentExecutablePath = Environment.ProcessPath;
            if (currentExecutablePath == null)
            {
                throw new InvalidOperationException("Could not determine the path of the current executable.");
            }
            Process.Start(currentExecutablePath);
            Application.Current.Shutdown();
        }

        private void InitializeCulture()
        {
            CultureInfo culture;
            //#if DEBUG
            //            culture = CultureInfo.InvariantCulture; // Force invariant culture for debugging
            //#else
            if (!string.IsNullOrEmpty(Settings.Default.LanguageVersion) && Languages.TryGetValue(Settings.Default.LanguageVersion, out CultureInfo? languageCulture))
            {
                culture = languageCulture;
            }
            else
            {
                culture = CultureInfo.CurrentCulture; // Use the current culture in release mode
            }
            //#endif
            // Set default culture for the application - seems unnecessary
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // Set current culture for the current thread - NECESSARY
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        }

        private ServiceProvider ConfigureServices()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    Path.Combine(LogPaths.Directory, "app-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 14,
                    shared: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            var services = new ServiceCollection();

            services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Information);
                logging.AddDebug();
                logging.AddSerilog(dispose: true);
            });

            services.AddSingleton<IAppSettings, SettingsService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IDialogService, WpfDialogService>();
            services.AddSingleton<IFocusedImageStore, FocusedImageStore>();

            services.AddImageOperations();

#if JSHARP_OPEN
            services.AddSingleton<IProGate, ProGateStub>();
#else
            services.AddSingleton<IProGate, ProGatePass>();
#endif

#if !JSHARP_OPEN
            services.AddSingleton<RecipeRecorder>(sp => new RecipeRecorder(
                sp.GetRequiredService<IOperationCatalog>(),
                Path.Combine(Path.GetTempPath(), "jsharp-recipe-staging")));
            services.AddSingleton<RecipeStore>(sp => new RecipeStore(
                sp.GetRequiredService<IOperationCatalog>(), RecipePaths.Directory));
#endif

            services.AddTransient<SettingsWindow>();

            services.AddTransient<SettingsWindowViewModel>();

            services.ConfigureMainWindowWithVM();

            return services.BuildServiceProvider();
        }
    }
}
