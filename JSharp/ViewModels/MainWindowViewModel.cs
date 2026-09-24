using JSharp.Domain.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Domain.Abstractions.Validation;
using JSharp.Domain.Models.SimpleDataModels;
using JSharp.Domain.Operations;
#if !JSHARP_OPEN
using JSharp.Domain.Recipes;
#endif
using JSharp.Services;
using JSharp.Services.Validation.Validators;
using JSharp.Domain.Services;
using JSharp.Operations;
using JSharp.Shared.Pro;
using JSharp.Shared.Resources;
using JSharp.UI.Views;
using JSharp.Utility.Utility;
using JSharp.ViewModels.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using OpenCvSharp;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="MainWindow"/>: document lifecycle, analysis windows,
    /// settings, and ONE generic operation dispatch path (collect -> execute -> apply).
    /// </summary>
    internal class MainWindowViewModel : ObservableObject
    {
        public ObservableCollection<NewImageWindowViewModel> OpenImageWindows
        {
            get => field;
            set => SetProperty(ref field, value);
        } = new ObservableCollection<NewImageWindowViewModel>();

        public NewImageWindowViewModel? FocusedImageVm { get; private set; }

        internal event EventHandler? ExitRequested;

        internal void SetFocusedImage(NewImageWindowViewModel vm)
        {
            FocusedImageVm = vm;
            _focusedImageStore.Set(vm);
        }

        public string LblFocusedImageContent
        {
            get => field;
            set => SetProperty(ref field, value);
        } = string.Empty;

        public string Descriptor
        {
            get => field;
            set => SetProperty(ref field, value);
        } = string.Empty;

        public string SelectedButtonTag
        {
            get => field;
            set => SetProperty(ref field, value);
        } = string.Empty;

        public bool IsBusy
        {
            get => field;
            set
            {
                if (SetProperty(ref field, value))
                {
                    OnPropertyChanged(nameof(IsBusyStatus));
                    RunOperationCommand.NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>User-facing status line for the busy overlay, e.g. "Running Watershed…".</summary>
        public string IsBusyStatus => IsBusy ? $"Running {_busyOperationName}…" : string.Empty;

        private string _busyOperationName = string.Empty;

        #region Commands
        public RelayCommand OpenRgb_ClickCommand { get; }
        public RelayCommand OpenGray_ClickCommand { get; }
        public RelayCommand Duplicate_CommandClick { get; }
        public RelayCommand Save_ClickCommand { get; }
        public RelayCommand SaveAll_ClickCommand { get; }
        public RelayCommand SaveAs_ClickCommand { get; }
        public RelayCommand Exit_ClickCommand { get; }
        public RelayCommand ShowHistogram_ClickCommand { get; }
        public RelayCommand SplitChannels_ClickCommand { get; }
        public RelayCommand SimpleAnalyze_ClickCommand { get; }
        public RelayCommand Analyze_ClickCommand { get; }
        public RelayCommand PlotProfile_ClickCommand { get; }
        public RelayCommand OpenSettings_ClickCommand { get; }
        public RelayCommand CopyToSystem_ClickCommand { get; }
        public RelayCommand CompressRLE_ClickCommand { get; }
        public RelayCommand OpenRleImageCommand { get; }
        public RelayCommand Undo_CommandCommand { get; }
        public RelayCommand<string> RunOperationCommand { get; }
        public RelayCommand CancelOperationCommand { get; }
#if !JSHARP_OPEN
        public RelayCommand StartRecordingCommand { get; }
        public RelayCommand StopRecordingCommand { get; }
        public RelayCommand OpenManagerCommand { get; }
#endif
        #endregion

#if !JSHARP_OPEN
        public bool IsRecording => Recorder?.IsRecording == true;

        public string RecordingStatus =>
            Recorder is { IsRecording: true, Draft: not null } recorder
                ? $"● Recording '{recorder.Draft.Name}' ({recorder.Draft.Steps.Count} step(s))"
                : string.Empty;
#endif

        private readonly IServiceProvider _services;
        private readonly IMessageService _messageService;
        private readonly IOperationExecutor _executor;
        private readonly IOperationDialogRouter _router;
        private readonly IOperationCatalog _catalog;
        private readonly RleImageCodec _rleImageCodec;
        private readonly IDialogService _dialogs;
        private readonly IFocusedImageStore _focusedImageStore;
        private readonly IProGate? _pro;
        private CancellationTokenSource? _activeCts;

        public MainWindowViewModel(
            IServiceProvider services,
            IMessageService messageService,
            IOperationExecutor executor,
            IOperationDialogRouter router,
            IOperationCatalog catalog,
            RleImageCodec rleImageCodec,
            IDialogService dialogs,
            IFocusedImageStore focusedImageStore,
            IProGate? pro = null)
        {
            _services = services;
            _messageService = messageService;
            _executor = executor;
            _router = router;
            _catalog = catalog;
            _rleImageCodec = rleImageCodec;
            _dialogs = dialogs;
            _focusedImageStore = focusedImageStore;
            _pro = pro;

            OpenRgb_ClickCommand = new RelayCommand(OpenRgb_Click);
            OpenGray_ClickCommand = new RelayCommand(OpenGray_Click);
            Duplicate_CommandClick = new RelayCommand(Duplicate_Click);
            Save_ClickCommand = new RelayCommand(Save);
            SaveAll_ClickCommand = new RelayCommand(SaveAll_Click);
            SaveAs_ClickCommand = new RelayCommand(SaveAs_Click);
            Exit_ClickCommand = new RelayCommand(Exit_Click);
            ShowHistogram_ClickCommand = new RelayCommand(ShowHistogram_Click);
            SplitChannels_ClickCommand = new RelayCommand(SplitChannels_Click);
            SimpleAnalyze_ClickCommand = new RelayCommand(SimpleAnalyze_Click);
            Analyze_ClickCommand = new RelayCommand(Analyze_Click);
            PlotProfile_ClickCommand = new RelayCommand(PlotProfile_Click);
            OpenSettings_ClickCommand = new RelayCommand(OpenSettings_Click);
            CopyToSystem_ClickCommand = new RelayCommand(CopyToSystem_Click);
            CompressRLE_ClickCommand = new RelayCommand(CompressRLE_Click);
            OpenRleImageCommand = new RelayCommand(OpenRleImage_Click);
            Undo_CommandCommand = new RelayCommand(Undo_Click);
            CancelOperationCommand = new RelayCommand(() => _activeCts?.Cancel());
            RunOperationCommand = new RelayCommand<string>(
                id => _ = RunOperationAsync(id!),
                _ => !IsBusy);
#if !JSHARP_OPEN
            StartRecordingCommand = new RelayCommand(
                StartRecording,
                () => Recorder is not { IsRecording: true });
            StopRecordingCommand = new RelayCommand(
                StopRecording,
                () => Recorder is { IsRecording: true });
            OpenManagerCommand = new RelayCommand(OpenManager);
#endif
        }

        internal void UpdateCheckedRadioButton(object sender)
        {
            var tag = ((System.Windows.Controls.RadioButton)sender).Tag;
            SelectedButtonTag = tag.ToString()!;
        }

        internal void UpdateDescriptor()
        {
            string? descriptor = StatusBarDescriptorBuilder.Build(
                SelectedButtonTag,
                FocusedImageVm?.MousePosition ?? default,
                FocusedImageVm?.Points);

            if (descriptor != null)
            {
                Descriptor = descriptor;
            }
        }

        private void OpenRgb_Click()
        {
            (List<Mat> matImages, List<string> fileNames) = ImageFileLoader.LoadColor(_messageService);
            for (int i = 0; i < matImages.Count; ++i)
            {
                DisplayImage(matImages[i], fileNames[i]);
            }
        }

        private void OpenGray_Click()
        {
            (List<Mat> matImages, List<string> fileNames) = ImageFileLoader.LoadGrayscale(_messageService);
            for (int i = 0; i < matImages.Count; ++i)
            {
                DisplayImage(matImages[i], fileNames[i]);
            }
        }

        private void Duplicate_Click()
        {
            if (FocusedImageVm == null)
            {
                ShowNoImageFocused();
                return;
            }

            DisplayImage(FocusedImageVm.MatImage.Clone(), FocusedImageVm.FileName);
        }

        internal void DisplayImage(Mat matImage, string fileName)
        {
            int duplicateCount = OpenImageWindows.Count(x => x.CoreName == Path.GetFileName(fileName));

            var appSettings = _services.GetRequiredService<IAppSettings>();
            var messageService = _services.GetRequiredService<IMessageService>();
            var executor = _services.GetRequiredService<IOperationExecutor>();
            NewImageWindowViewModel imageWindowViewModel = new(this, executor, appSettings, messageService, matImage, fileName, duplicateCount, _pro);
            imageWindowViewModel.Source = MatToBitmapSource.Convert(matImage);
            NewImageWindow newImageWindow = new NewImageWindow();

            OpenImageWindows.Add(imageWindowViewModel);
            imageWindowViewModel.FocusChanged += HandleFocusChanged;
            imageWindowViewModel.ImageChanged += NewImageWindow_ImageChanged;
            imageWindowViewModel.Closing += OnNewImageWindowClosing;

            newImageWindow.DataContext = imageWindowViewModel;
            newImageWindow.Show();
        }

        #region event handler methods
        private void OnNewImageWindowClosing(object? sender, EventArgs e)
        {
            if (sender is not NewImageWindowViewModel closingViewModel)
            {
                return;
            }

            if (ReferenceEquals(FocusedImageVm, closingViewModel))
            {
                FocusedImageVm = null;
                _focusedImageStore.Set(null);
                UpdateDescriptor();
            }

            OpenImageWindows.Remove(closingViewModel);

            closingViewModel.FocusChanged -= HandleFocusChanged!;
            closingViewModel.ImageChanged -= NewImageWindow_ImageChanged!;
            closingViewModel.Closing -= OnNewImageWindowClosing!;
            closingViewModel.Dispose();
        }

        public void HandleFocusChanged(object? sender, string newContent)
        {
            UpdateLabelContent(newContent);
        }

        private void NewImageWindow_ImageChanged(object? sender, Mat newImage)
        {
            if (sender is NewImageWindowViewModel newImageViewModel)
            {
#if !JSHARP_OPEN
                newImageViewModel.histogramWindowViewModel?.UpdateHistogram(newImage);
#endif
            }
        }
        #endregion

        private void UpdateLabelContent(string newContent)
        {
            LblFocusedImageContent = newContent;
        }

        #region Saves
        internal void Save()
        {
            if (FocusedImageVm == null)
            {
                ShowNoImageFocused();
                return;
            }

            FocusedImageVm.SaveChanges();
        }

        private void SaveAs_Click()
        {
            if (FocusedImageVm == null)
            {
                ShowNoImageFocused();
                return;
            }

            FocusedImageVm.SaveAs();
        }

        private void SaveAll_Click()
        {
            if (OpenImageWindows == null || OpenImageWindows.Count == 0)
            {
                _messageService.ShowMessage(Errors.NoImageOpen, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (var window in OpenImageWindows)
            {
                window.SaveChanges();
            }
        }
        #endregion

        private void Undo_Click()
        {
            if (FocusedImageVm == null)
            {
                ShowNoImageFocused();
                return;
            }

            FocusedImageVm.Undo();
        }

        private void Exit_Click()
        {
            ExitRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ShowHistogram_Click()
        {
            if (FocusedImageVm == null)
            {
                ShowNoImageFocused();
                return;
            }

            try
            {
                _pro?.ThrowIfOpen("measurements");
            }
            catch (NotSupportedException ex)
            {
                _messageService.ShowMessage(_pro?.ProMessage ?? ex.Message, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

#if !JSHARP_OPEN
            if (FocusedImageVm.MatImage.Channels() != Constants.Grayscale_ChannelCount)
            {
                _messageService.ShowMessage(Errors.ImageNotGrayscale, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (FocusedImageVm.histogramWindowViewModel != null)
            {
                _messageService.ShowMessage(Errors.HistogramAlreadyOpen, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FocusedImageVm.MakeHistogram();
#endif
        }

        private void SplitChannels_Click()
        {
            if (FocusedImageVm == null)
            {
                ShowNoImageFocused();
                return;
            }

            if (FocusedImageVm.MatImage.Channels() != Constants.Xyz_ChannelCount)
            {
                _messageService.ShowMessage(Errors.ImageNotColor, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DisplaySplitChannels(FocusedImageVm.ColorSpaceType);
        }

        private void DisplaySplitChannels(ColorSpaceType colorSpace)
        {
            if (FocusedImageVm == null)
            {
                return;
            }

            Mat[] channels = ImageAnalysis.SplitChannels(FocusedImageVm.MatImage);
            string originalName = FocusedImageVm.FileName;

            for (int i = 0; i < channels.Length; ++i)
            {
                Mat channel = channels[i];

                if (channel != null)
                {
                    string channelName = ImageProcessingUtility.ChooseChannelName(colorSpace, i);
                    string title = $"{channelName} - {originalName}";
                    DisplayImage(channel, title);
                }
            }
        }

        private void SimpleAnalyze_Click()
        {
            if (FocusedImageVm == null)
            {
                ShowNoImageFocused();
                return;
            }

            try
            {
                _pro?.ThrowIfOpen("measurements");
            }
            catch (NotSupportedException ex)
            {
                _messageService.ShowMessage(_pro?.ProMessage ?? ex.Message, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

#if !JSHARP_OPEN
            SummaryWindowViewModel summaryWindowViewModel = new SummaryWindowViewModel(FocusedImageVm);
            SummaryWindow summaryWindow = new SummaryWindow();
            summaryWindow.DataContext = summaryWindowViewModel;

            summaryWindow.Show();
#endif
        }

        private void Analyze_Click()
        {
            if (FocusedImageVm == null)
            {
                ShowNoImageFocused();
                return;
            }

            try
            {
                _pro?.ThrowIfOpen("measurements");
            }
            catch (NotSupportedException ex)
            {
                _messageService.ShowMessage(_pro?.ProMessage ?? ex.Message, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

#if !JSHARP_OPEN
            AnalyzeParticlesWindowViewModel analyzeParticlesWindowViewModel = new(_messageService);

            (bool confirmed, AnalysisSettings settings) = _dialogs.ShowDialog<AnalyzeParticlesWindowViewModel, AnalysisSettings>(analyzeParticlesWindowViewModel);
            if (!confirmed)
            {
                return;
            }

            SummaryWindowViewModel summaryWindowViewModel = new SummaryWindowViewModel(settings, FocusedImageVm);
            SummaryWindow summaryWindow = new SummaryWindow();
            summaryWindow.DataContext = summaryWindowViewModel;

            summaryWindow.Show();
#endif
        }

        private void PlotProfile_Click()
        {
            if (FocusedImageVm == null)
            {
                ShowNoImageFocused();
                return;
            }

            try
            {
                _pro?.ThrowIfOpen("measurements");
            }
            catch (NotSupportedException ex)
            {
                _messageService.ShowMessage(_pro?.ProMessage ?? ex.Message, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

#if !JSHARP_OPEN
            var points = FocusedImageVm.Points.Select(p => p != null && p.HasValue ? p.Value : (System.Windows.Point?)null).ToArray();

            if (points[0] == null || points[1] == null)
            {
                _messageService.ShowMessage(Errors._2PointsMustBeSelected, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (FocusedImageVm.MatImage.Channels() != Constants.Grayscale_ChannelCount)
            {
                _messageService.ShowMessage(Errors.ImageNotGrayscale, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            System.Windows.Point[] validPoints = points.Where(p => p.HasValue).Select(p => p!.Value).ToArray();

            PlotlineGraphWindowViewModel plotlineGraphWindowViewModel = new PlotlineGraphWindowViewModel(validPoints, FocusedImageVm.MatImage);
            PlotlineGraphWindow plotlineGraphWindow = new PlotlineGraphWindow();
            plotlineGraphWindow.DataContext = plotlineGraphWindowViewModel;

            plotlineGraphWindow.Show();
#endif
        }

        private void OpenSettings_Click()
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            SettingsWindowViewModel settingsWindowViewModel = _services.GetRequiredService<SettingsWindowViewModel>();
            settingsWindow.DataContext = settingsWindowViewModel;
            settingsWindow.ShowDialog();
        }

        private void CopyToSystem_Click()
        {
            if (FocusedImageVm == null)
            {
                ShowNoImageFocused();
                return;
            }

            BitmapSource bitmapSource = MatToBitmapSource.Convert(FocusedImageVm.MatImage);
            Clipboard.SetImage(bitmapSource);
        }

        private void CompressRLE_Click()
        {
            if (FocusedImageVm == null)
            {
                ShowNoImageFocused();
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                FileName = FocusedImageVm.CoreName + ".rle",
                DefaultExt = ".rle",
                Filter = Constants.RleFilterString
            };
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                _rleImageCodec.Save(FocusedImageVm.MatImage, dialog.FileName);

                int originalFileSize = (int)(FocusedImageVm.MatImage.Total() * FocusedImageVm.MatImage.ElemSize());
                int compressedFileSize = (int)new FileInfo(dialog.FileName).Length;
                double compressionRatio = CompressionCore.CalculateCompressionRatio(compressedFileSize, originalFileSize);
                _messageService.ShowMessage($"{Messages.OriginalFileSize}: {originalFileSize}{Environment.NewLine}{Environment.NewLine}" +
                    $"{Messages.CompressedFileSize}: {compressedFileSize}{Environment.NewLine}{Environment.NewLine}" +
                    $"{Messages.CompressionRatio}: {compressionRatio.ToString()}", Strings.Compression, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                _messageService.ShowMessage(Errors.LoadingFailedRle, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenRleImage_Click()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Multiselect = false,
                DefaultExt = ".rle",
                Filter = Constants.RleFilterString
            };
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                Mat restored = _rleImageCodec.Load(dialog.FileName);
                DisplayImage(restored, dialog.FileName);
            }
            catch (InvalidDataException)
            {
                _messageService.ShowMessage(Errors.InvalidRleFile, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                _messageService.ShowMessage(Errors.LoadingFailedRle, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ShowAbout()
        {
            string informationalVersion = Assembly.GetEntryAssembly()!
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
                .InformationalVersion;

            _messageService.ShowMessage($"JSharp{Environment.NewLine}{Strings.Image_Processing_Program}{Environment.NewLine}{Environment.NewLine}" +
                $"{Strings.Version}:{Environment.NewLine}{informationalVersion}{Environment.NewLine}{Environment.NewLine}" +
                $"{Strings.Author}:{Environment.NewLine}Łukasz Górski{Environment.NewLine}{Environment.NewLine}Copyright © 2024 Łukasz Górski",
                $"{UIStrings.About}", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #region Generic operation dispatch
        internal async Task RunOperationAsync(string operationId)
        {
            if (FocusedImageVm == null)
            {
                ShowNoImageFocused();
                return;
            }

            OperationExecutionPlan? plan = await _router.TryCollectAsync(operationId);
            if (plan is null || plan.HandledByCollector)
            {
                return;
            }

            string executorId = plan.OperationId ?? operationId;

            _busyOperationName = _catalog.GetDescriptor(executorId).DisplayNameKey;
            OnPropertyChanged(nameof(IsBusyStatus));

            IsBusy = true;
            try
            {
                using CancellationTokenSource cts = new CancellationTokenSource();
                _activeCts = cts;

                OperationResult result = await _executor.ExecuteAsync(
                    new OperationRequest(executorId, FocusedImageVm.MatImage, plan.Parameters),
                    cts.Token);

                if (!result.Success)
                {
                    ShowOperationError(executorId, result);
                    return;
                }

                if (plan.CreateNewWindow)
                {
                    Mat output = result.Output!;
                    DisplayImage(output, "Image Calculator Result Window");
                    NotifyOperationCommitted(operationId, plan);
                    output.Dispose();
                    return;
                }

                ColorSpaceType? appliedColorspace = OutputColorSpace.ApplyStep(
                    FocusedImageVm.ColorSpaceType, plan.NewColorSpace?.ToString(), result.Output!.Channels());

                int repeats = Math.Max(1, plan.Repeat);
                for (int i = 0; i < repeats; i++)
                {
                    Mat output = i == repeats - 1 ? result.Output! : result.Output!.Clone();
                    FocusedImageVm.ApplyOperationOutput(output, appliedColorspace);
                }

                if (appliedColorspace is { } cs)
                {
                    UpdateLabelContent($"({cs.GetName()}) {FocusedImageVm.FileName}");
                }

                if (plan.ThenSplitChannels)
                {
                    DisplaySplitChannels(FocusedImageVm.ColorSpaceType);
                }

                NotifyOperationCommitted(operationId, plan);
            }
            finally
            {
                IsBusy = false;
                _activeCts = null;
            }
        }

        private void ShowOperationError(string operationId, OperationResult result)
        {
            if (result.ErrorCode == JSharp.Domain.Operations.ErrorCodes.Cancelled)
            {
                return; // silent by design
            }

            // error-clarity: name the operation and give a recovery path, never raw ids.
            string operationName = SafeOperationName(operationId);
            string message = OperationResultPresenter.ToLocalizedMessage(result, operationName);
            _messageService.ShowMessage(message, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        internal string SafeOperationName(string operationId) =>
            _catalog.TryGetDescriptor(operationId, out OperationDescriptor? descriptor)
                ? descriptor.DisplayNameKey
                : operationId;

#if !JSHARP_OPEN
        internal RecipeRecorder? Recorder { get; set; }
#endif

        internal void NotifyOperationCommitted(string menuOperationId, OperationExecutionPlan plan)
        {
#if !JSHARP_OPEN
            if (Recorder is not { IsRecording: true })
            {
                return;
            }

            string effectiveId = plan.OperationId ?? menuOperationId;
            Recorder.TryAppend(effectiveId, SafeOperationName(effectiveId), plan);
            UpdateRecordingState();
#endif
        }
        #endregion

        #region Recipe recording

        internal void StartRecording()
        {
            try
            {
                _pro?.ThrowIfOpen("recipes");
            }
            catch (NotSupportedException ex)
            {
                _messageService.ShowMessage(_pro?.ProMessage ?? ex.Message, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

#if !JSHARP_OPEN
            Recorder?.Start("Untitled recipe");
            UpdateRecordingState();
#endif
        }

        internal void StopRecording()
        {
            try
            {
                _pro?.ThrowIfOpen("recipes");
            }
            catch (NotSupportedException ex)
            {
                _messageService.ShowMessage(_pro?.ProMessage ?? ex.Message, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

#if !JSHARP_OPEN
            Recorder?.Stop();
            try
            {
                if (Recorder?.Draft is not { } draft || draft.Steps.Count == 0)
                {
                    _messageService.ShowMessage(
                        "No operations were recorded.", "Recipes",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                RecipeStore store = _services.GetRequiredService<RecipeStore>();
                string prefill = $"Recipe {DateTime.Now:yyyy-MM-dd HH-mm-ss}";
                string name = PromptRecipeName(prefill) ?? prefill;
                string path = store.Save(draft, name, Recorder.StagingDirectory);
                _messageService.ShowMessage(
                    $"Recipe saved ({draft.Steps.Count} step(s)):{Environment.NewLine}{path}" +
                    $"{Environment.NewLine}Manage it via Recipes > Recipes...",
                    "Recipes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                Recorder?.Clear();
                UpdateRecordingState();
            }
#endif
        }

#if !JSHARP_OPEN
        private void UpdateRecordingState()
        {
            OnPropertyChanged(nameof(IsRecording));
            OnPropertyChanged(nameof(RecordingStatus));
            StartRecordingCommand.NotifyCanExecuteChanged();
            StopRecordingCommand.NotifyCanExecuteChanged();
        }
#endif
        #endregion

        #region Recipe run
        internal async Task RunRecipeAsync(string? recipePath)
        {
            if (FocusedImageVm == null)
            {
                ShowNoImageFocused();
                return;
            }

            try
            {
                _pro?.ThrowIfOpen("recipes");
            }
            catch (NotSupportedException ex)
            {
                _messageService.ShowMessage(_pro?.ProMessage ?? ex.Message, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

#if !JSHARP_OPEN
            string? selected = recipePath;
            if (selected is null)
            {
                OpenFileDialog dlg = new()
                {
                    Filter = $"JSharp Recipe (*{RecipePaths.DefaultExtension})|*{RecipePaths.DefaultExtension}|Recipe Bundle (*{RecipePaths.BundleExtension})|*{RecipePaths.BundleExtension}",
                    Title = "Run Recipe...",
                };
                if (dlg.ShowDialog() != true)
                {
                    return;
                }

                selected = dlg.FileName;
            }

            RawRecipeDocument raw;
            IAuxResolver resolver;
            if (selected.EndsWith(RecipePaths.BundleExtension, StringComparison.OrdinalIgnoreCase))
            {
                string temp = Path.Combine(Path.GetTempPath(), "jsharp-run-" + Guid.NewGuid());
                Directory.CreateDirectory(temp);
                string root = BundleAuxResolver.ExtractWithCaps(selected, temp);
                raw = RecipeRaw.ReadRaw(File.ReadAllText(root));
                resolver = new BundleAuxResolver(temp);
            }
            else
            {
                raw = RecipeRaw.ReadRaw(File.ReadAllText(selected));
                resolver = new FileAuxResolver(Path.GetDirectoryName(selected)!);
            }

            // Matrixed inputs run the full cartesian product; only the
            // non-matrixed inputs are prompted. Cancel aborts silently.
            IReadOnlyDictionary<string, string> overrides =
                new Dictionary<string, string>(StringComparer.Ordinal);
            if (raw.Inputs is { Count: > 0 })
            {
                HashSet<string> matrixed = MatrixedInputNames(raw);
                Dictionary<string, InputDeclaration> promptable = raw.Inputs
                    .Where(kv => !matrixed.Contains(kv.Key))
                    .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.Ordinal);
                if (promptable.Count > 0)
                {
                    IReadOnlyDictionary<string, string>? collected =
                        (RecipeInputsPrompter ?? RecipeInputsWindow.Prompt)(promptable);
                    if (collected is null)
                    {
                        return;
                    }

                    overrides = BuildOverrides(
                        promptable,
                        (name, _) => collected.TryGetValue(name, out string? text) ? text : null);
                }
            }

            List<RecipeIssue> errors = RecipePreflight
                .CheckRaw(raw, _catalog, FocusedImageVm.MatImage, resolver, overrides)
                .Where(i => !i.IsWarning)
                .ToList();
            if (errors.Count > 0)
            {
                string list = string.Join(Environment.NewLine,
                    errors.Take(5).Select(e => $"Step {e.StepIndex + 1} ({e.OperationId}): {e.Message}"));
                if (errors.Count > 5)
                {
                    list += $"{Environment.NewLine}…and {errors.Count - 5} more.";
                }

                _messageService.ShowMessage(list, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RecipePlayer player = new(_catalog, _executor);
            IReadOnlyList<BoundRun> runs = RecipeBinding.Resolve(raw, overrides);
            _busyOperationName = raw.Name;
            OnPropertyChanged(nameof(IsBusyStatus));
            IsBusy = true;
            try
            {
                using CancellationTokenSource cts = new CancellationTokenSource();
                _activeCts = cts;
                // Each combo runs from the pre-run snapshot (independent pixels);
                // commits apply sequentially so history holds one entry per combo;
                // cancel leaves already-committed combos. Safe to share the
                // snapshot: RecipePlayer clones its input per RunAsync.
                using Mat baseSnapshot = FocusedImageVm.MatImage.Clone();
                ColorSpaceType baseCs = FocusedImageVm.ColorSpaceType;
                for (int c = 0; c < runs.Count; c++)
                {
                    BoundRun run = runs[c];
                    RecipeDocument doc = run.Document;
                    string label = run.BoundInputs.Count == 0
                        ? raw.Name
                        : $"{raw.Name} [{ComboTag(run.BoundInputs)}]";
                    int combo = c;
                    Progress<RecipeProgress> progress = new(p =>
                        _busyOperationName =
                            $"{label} — step {Math.Min(p.CompletedSteps + 1, p.TotalSteps)}/{p.TotalSteps}: " +
                            $"{SafeOperationName(p.CurrentOperationId)}");

                    RecipeRunResult result = await player.RunAsync(
                        baseSnapshot, baseCs, doc, resolver, progress, cts.Token);

                    if (!result.Success || result.Output is null)
                    {
                        result.Output?.Dispose();
                        if (result.StepResult?.ErrorCode == JSharp.Domain.Operations.ErrorCodes.Cancelled)
                        {
                            return; // silent by design
                        }

                        string opName = SafeOperationName(result.FailedOperationId ?? string.Empty);
                        string detail = result.StepResult is not null
                            ? OperationResultPresenter.ToLocalizedMessage(result.StepResult, opName)
                            : "Unknown error.";
                        string comboPrefix = runs.Count > 1 ? $"[combo {combo + 1}/{runs.Count}] " : string.Empty;
                        string where = result.FailedStepIndex >= 0
                            ? $"{comboPrefix}Step {result.FailedStepIndex + 1} ({opName}): "
                            : comboPrefix;
                        _messageService.ShowMessage(where + detail, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    FocusedImageVm.ApplyOperationOutput(result.Output, result.FinalColorSpace);
                    if (doc.Steps.Any(s => s.Enabled && s.ThenSplitChannels))
                    {
                        DisplaySplitChannels(FocusedImageVm.ColorSpaceType);
                    }
                }
            }
            finally
            {
                IsBusy = false;
                _activeCts = null;
            }
#else
            await Task.CompletedTask;
#endif
        }

#if !JSHARP_OPEN
        /// <summary>
        /// Test seam: stubbed by VM tests to avoid showing the WPF prompt window
        /// headlessly. Production default is <see cref="RecipeInputsWindow.Prompt"/>.
        /// Null return means the user cancelled.
        /// </summary>
        internal Func<IReadOnlyDictionary<string, InputDeclaration>, IReadOnlyDictionary<string, string>?>? RecipeInputsPrompter { get; set; }

        private static IReadOnlyDictionary<string, string> BuildOverrides(
            IReadOnlyDictionary<string, InputDeclaration> declared,
            Func<string, string?, string?> prompter) =>
            RecipeInputPrompt.Parse(declared, prompter);

        private static HashSet<string> MatrixedInputNames(RawRecipeDocument raw)
        {
            HashSet<string> names = new(StringComparer.Ordinal);
            if (raw.MatrixRaw is { ValueKind: System.Text.Json.JsonValueKind.Object } matrix)
            {
                foreach (System.Text.Json.JsonProperty prop in matrix.EnumerateObject())
                {
                    names.Add(prop.Name);
                }
            }

            return names;
        }

        private static string ComboTag(IReadOnlyDictionary<string, TypedValue> boundInputs) =>
            string.Join(", ", boundInputs.Select(kv => $"{kv.Key}={FormatTyped(kv.Value)}"));

        private static string FormatTyped(TypedValue value) => value switch
        {
            TypedValue.IntValue i => i.Value.ToString(CultureInfo.InvariantCulture),
            TypedValue.NumberValue n => n.Value.ToString(CultureInfo.InvariantCulture),
            TypedValue.StringValue s => s.Value,
            TypedValue.BoolValue b => b.Value ? "true" : "false",
            _ => value.ToString() ?? string.Empty,
        };

        private void OpenManager()
        {
            try
            {
                _pro?.ThrowIfOpen("recipes");
            }
            catch (NotSupportedException ex)
            {
                _messageService.ShowMessage(_pro?.ProMessage ?? ex.Message, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RecipeStore store = _services.GetRequiredService<RecipeStore>();
            RecipeManagerWindowViewModel vm = new(
                store,
                _messageService,
                path => RunRecipeAsync(path),
                OpenRecipeEditor,
                PromptRecipeName);
            RecipeManagerWindow window = new() { DataContext = vm };
            window.Show();
        }

        private void OpenRecipeEditor(string recipePath)
        {
            try
            {
                _pro?.ThrowIfOpen("recipes");
            }
            catch (NotSupportedException ex)
            {
                _messageService.ShowMessage(_pro?.ProMessage ?? ex.Message, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RecipeStore store = _services.GetRequiredService<RecipeStore>();
            RecipeDocument doc = store.Load(recipePath);
            RecipeEditorWindowViewModel vm = new(doc, recipePath, store, _messageService);
            RecipeEditorWindow window = new() { DataContext = vm };
            window.Show();
        }
#endif

        private string? PromptRecipeName(string prefill)
        {
            (bool confirmed, string? name) = _dialogs
                .ShowDialog<RecipeNamePromptWindowViewModel, string>(
                    new RecipeNamePromptWindowViewModel(prefill));
            return confirmed ? name : null;
        }
        #endregion

        #region Error helpers used by the dialog router
        internal IMessageService MessageServiceForDialogs => _messageService;

        internal void ShowCalculatorGrayscaleError() =>
            _messageService.ShowMessage(Errors.ImageNotGrayscale, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);

        internal void ShowTwoImagesRequiredError() =>
            _messageService.ShowMessage(Errors._2ImagesMustBeOpen, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);

        internal void ShowTwoPointsRequiredError() =>
            _messageService.ShowMessage(Errors._2PointsMustBeSelected, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        #endregion

        private void ShowNoImageFocused() =>
            _messageService.ShowMessage(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
