using JSharp.Domain.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using JSharp.Domain.Documents;
using JSharp.Domain.Models.SimpleDataModels;
using JSharp.Domain.Operations;
using JSharp.Services;
using JSharp.Operations;
using JSharp.Shared.Pro;
using JSharp.UI.Views;
using JSharp.Utility.Utility;
using JSharp.ViewModels.Abstractions;
using Microsoft.Win32;
using OpenCvSharp;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="JSharp.UI.Views.NewImageWindow"/>. Composes an immutable
    /// <see cref="ImageDocument"/> (pixels, color space, undo history) and routes every
    /// algorithm invocation through the operation executor.
    /// </summary>
    internal class NewImageWindowViewModel : ObservableObject, IDisposable
    {
        private const int widthAdjustmentConst = 16;
        private const int heightAdjustmentConst = 39;

        private readonly MainWindowViewModel _owner;
        private readonly IOperationExecutor _executor;
        private readonly SemaphoreSlim _operationGate = new(1, 1);

        // Live token for the currently executing per-image operation, if any.
        // Guarded by _operationGate; cancelled on Dispose so window close stops work early.
        private CancellationTokenSource? _operationCts;
        private ImageDocument _document;

        public event EventHandler<string>? FocusChanged;
        public event EventHandler<Mat>? ImageChanged;
        public event EventHandler? Closing;

        public System.Windows.Point MousePosition
        {
            get => field;
            set
            {
                SetProperty(ref field, value);
                _owner.UpdateDescriptor();
            }
        }

        #region dual fields/properties
        public Mat MatImage => _document.CurrentMat;

        public bool CanUndo => _document.CanUndo;

        public double ZoomScale
        {
            get => field;
            set => SetProperty(ref field, value);
        } = 1.0;

        /// <summary>
        /// basic name + extension
        /// for counting duplicates
        /// </summary>
        internal string CoreName { get; private set; } = string.Empty;

        public string FileName
        {
            get => field;
            private set => SetProperty(ref field, value);
        } = string.Empty;

        public BitmapSource? Source
        {
            get => field;
            set
            {
                SetProperty(ref field, value);
                UpdateWindowSize();
            }
        }

        public ColorSpaceType ColorSpaceType => _document.ColorSpaceType;

        public string Title
        {
            get => field;
            set => SetProperty(ref field, value);
        } = string.Empty;

        public double Height
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        public double Width
        {
            get => field;
            set => SetProperty(ref field, value);
        }
        #endregion

        internal ObservableCollection<System.Windows.Point?> Points { get; set; } = new ObservableCollection<System.Windows.Point?> { null, null };
        private int DuplicateCount { get; set; }
        internal string? filePath;

#if !JSHARP_OPEN
        internal HistogramWindowViewModel? histogramWindowViewModel;
        private HistogramWindow? histogramWindow;
#endif

        private readonly IAppSettings _appSettings;
        private readonly IMessageService _messageService;
        private readonly IProGate? _pro;

        public NewImageWindowViewModel(
            MainWindowViewModel owner,
            IOperationExecutor executor,
            IAppSettings appSettings,
            IMessageService messageService,
            Mat matImage,
            string fileName,
            int duplicateCount,
            IProGate? pro = null)
        {
            _owner = owner;
            _executor = executor;
            _appSettings = appSettings;
            _messageService = messageService;
            _pro = pro;

            _document = ImageDocument.Create(matImage.Clone());

            string saveFileName = Path.GetFileName(fileName);

            if (saveFileName != fileName)
            {
                filePath = fileName;
            }

            HandleNaming(saveFileName, duplicateCount);

            ZoomScale = 1.0;

            UpdateTitle();

            UpdateWindowSize();
        }

        #region Title/name management
        private void UpdateTitle()
        {
            Title = $"{FileName} ({ColorSpaceType.GetName()}) ({Math.Round(ZoomScale * 100, 2)}%)";
        }

        private string GetDuplicateString(int duplicateCount)
        {
            if (duplicateCount == 0)
            {
                return string.Empty;
            }
            else
            {
                return $"-{duplicateCount}";
            }
        }

        private void HandleNaming(string fileName, int duplicateCount)
        {
            string coreNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
            string fileExtension = System.IO.Path.GetExtension(fileName);
            this.CoreName = coreNameWithoutExtension + fileExtension;
            this.DuplicateCount = duplicateCount;
            this.FileName = coreNameWithoutExtension + GetDuplicateString(duplicateCount) + fileExtension;
        }
        #endregion

        #region Other internal UI updates
        internal void ScaleZoom(bool isZoomIn)
        {
            if (isZoomIn)
            {
                ZoomScale *= _appSettings.CumulativeZoomFactor;
            }
            else
            {
                ZoomScale /= _appSettings.CumulativeZoomFactor;
            }

            UpdateWindowSize();
            UpdateTitle();
        }

        public void ApplyOperationOutput(Mat newImage, ColorSpaceType? newColorSpace = null)
        {
            _document = _document.Apply(newImage, newColorSpace);
            OnPropertyChanged(nameof(ColorSpaceType));
            OnPropertyChanged(nameof(MatImage));
            OnPropertyChanged(nameof(CanUndo));
            Source = MatToBitmapSource.Convert(MatImage);
            UpdateTitle();

            // Trigger the ImageChanged event for histogram to update reactively
            ImageChanged?.Invoke(this, MatImage);
        }

        public void Undo()
        {
            if (!_document.CanUndo)
            {
                return;
            }

            _document = _document.Undo();
            OnPropertyChanged(nameof(MatImage));
            OnPropertyChanged(nameof(CanUndo));
            Source = MatToBitmapSource.Convert(MatImage);
            UpdateTitle();
            ImageChanged?.Invoke(this, MatImage);
        }

        private void UpdateWindowSize()
        {
            if (Source == null)
            {
                return;
            }

            this.Width = (Source.Width * ZoomScale) + widthAdjustmentConst;
            this.Height = (Source.Height * ZoomScale) + heightAdjustmentConst;
        }
        #endregion

        #region External-related
        public void SaveChanges()
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                ImageFileWriter.TryWrite(MatImage, filePath, _messageService);
            }
            else
            {
                SaveAs();
            }
        }

        public void SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = Constants.ImageFilterString;
            saveFileDialog.Title = "Save Image As...";
            saveFileDialog.FileName = FileName;

            if (saveFileDialog.ShowDialog() == true)
            {
                if (ImageFileWriter.TryWrite(MatImage, saveFileDialog.FileName, _messageService))
                {
                    filePath = saveFileDialog.FileName;
                }
            }
        }

        /// <summary>
        /// True once the window started closing. Live-preview dialogs are modeless and can
        /// outlive their image; their callbacks must not touch the disposed document.
        /// </summary>
        internal bool IsClosed { get; private set; }

        public void Window_Activated()
        {
            _owner.SetFocusedImage(this);
            string titleForMainWindowLabel = $" ({ColorSpaceType.GetName()}) {FileName}";
            FocusChanged?.Invoke(this, titleForMainWindowLabel);
        }

        public void NewImageWindow_Closing()
        {
            IsClosed = true;

            if (ReferenceEquals(_owner.FocusedImageVm, this))
            {
                FocusChanged?.Invoke(this, "null");
            }

            this.MatImage.Dispose();
#if !JSHARP_OPEN
            histogramWindow?.Close();
#endif
            Closing?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            // Stop queued/cooperative work when the image window closes. Deliberately does NOT
            // dispose _operationGate here: an in-flight operation's finally block may still
            // release it, and SemaphoreSlim holds no unmanaged resources worth the race.
            //
            // Residual risk (documented in the spec §2.3): a native OpenCV call already in
            // flight cannot be interrupted; fully closing that gap requires an async close
            // flow (drain-wait on the UI thread would deadlock on continuations).
            _operationCts?.Cancel();
        }
        #endregion

        #region Actions
        public void MakeHistogram()
        {
            try
            {
                _pro?.ThrowIfOpen("measurements");
            }
            catch (NotSupportedException ex)
            {
                _messageService.ShowMessage(_pro?.ProMessage ?? ex.Message, JSharp.Shared.Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

#if !JSHARP_OPEN
            if (histogramWindow != null)
            {
                return;
            }

            histogramWindowViewModel = new HistogramWindowViewModel();
            histogramWindow = new HistogramWindow { DataContext = histogramWindowViewModel };
            histogramWindowViewModel.UpdateHistogram(MatImage);
            histogramWindow.Closed += (_, _) =>
            {
                histogramWindow = null;
                histogramWindowViewModel = null;
            };
            histogramWindow.Show();
#endif
        }
        #endregion

        /// <summary>Live preview: updates the view only - never touches the document or undo history.</summary>
        public void PreviewThresholding(Mat img, int minThreshold, int maxThreshold, ThresholdingType thresholdingType, bool enableContrastMode)
        {
            QueuePreview("threshold.dual", img,
                new DualThresholdParams(minThreshold, maxThreshold, thresholdingType, enableContrastMode));
        }

        public async Task PerformThresholding(int minThreshold, int maxThreshold, ThresholdingType thresholdingType, bool enableContrastMode)
        {
            if (IsClosed)
            {
                return;
            }

            _previewCts?.Cancel();
            var parameters = new DualThresholdParams(minThreshold, maxThreshold, thresholdingType, enableContrastMode);
            if (await RunToCurrentAsync("threshold.dual", parameters).ConfigureAwait(false))
            {
                _owner.NotifyOperationCommitted("threshold.dual", new OperationExecutionPlan(parameters));
            }
        }

        /// <summary>Live preview: updates the view only - never touches the document or undo history.</summary>
        public void PreviewSimpleThresholding(Mat img, int threshold, SimpleThresholdingMethod thresholdingMethod)
        {
            QueuePreview("threshold.simple", img, new SimpleThresholdParams(threshold, thresholdingMethod));
        }

        public async Task PerformSimpleThresholding(int threshold, SimpleThresholdingMethod thresholdingMethod)
        {
            if (IsClosed)
            {
                return;
            }

            _previewCts?.Cancel();
            var parameters = new SimpleThresholdParams(threshold, thresholdingMethod);
            if (await RunToCurrentAsync("threshold.simple", parameters).ConfigureAwait(false))
            {
                _owner.NotifyOperationCommitted("threshold.simple", new OperationExecutionPlan(parameters));
            }
        }

        private void RefreshViewOnly(Mat preview)
        {
            Source = MatToBitmapSource.Convert(preview);
            ImageChanged?.Invoke(this, preview);
            preview.Dispose();
        }

        /// <summary>
        /// Drops a live preview: stops queued preview work and repaints the view from
        /// the document. The document itself is never touched by previews.
        /// </summary>
        internal void DiscardPreview()
        {
            if (IsClosed)
            {
                return;
            }

            _previewCts?.Cancel();
            Source = MatToBitmapSource.Convert(MatImage);
            ImageChanged?.Invoke(this, MatImage);
        }

        private CancellationTokenSource? _previewCts;

        internal TimeSpan PreviewDebounceDelay { get; set; } = TimeSpan.FromMilliseconds(150);

        internal void QueuePreview(string operationId, Mat source, OperationParams parameters)
        {
            if (IsClosed)
            {
                return;
            }

            _previewCts?.Cancel();
            CancellationTokenSource cts = new();
            _previewCts = cts;

            _ = RunPreviewAsync(operationId, source, parameters, cts);
        }

        private async Task RunPreviewAsync(
            string operationId, Mat source, OperationParams parameters, CancellationTokenSource cts)
        {
            try
            {
                await Task.Delay(PreviewDebounceDelay, cts.Token);

                OperationResult result = await _executor.ExecuteAsync(
                    new OperationRequest(operationId, source, parameters), cts.Token);

                if (cts.Token.IsCancellationRequested)
                {
                    result.Output?.Dispose();
                    return;
                }

                if (!result.Success || result.Output is null)
                {
                    ShowOperationFailure(operationId, result);
                    return;
                }

                RefreshViewOnly(result.Output);
            }
            catch (OperationCanceledException)
            {
                // A newer preview superseded this one - by design.
            }
        }

        internal MainWindowViewModel Owner => _owner;

        internal void ResetPoints()
        {
            Points[0] = null;
            Points[1] = null;
        }

        internal async Task<bool> RunToCurrentAsync(string operationId, OperationParams parameters)
        {
            await _operationGate.WaitAsync();
            CancellationTokenSource cts = new CancellationTokenSource();
            _operationCts = cts;
            try
            {
                OperationResult result = await _executor.ExecuteAsync(
                    new OperationRequest(operationId, MatImage, parameters), cts.Token);

                if (!result.Success || result.Output is null)
                {
                    ShowOperationFailure(operationId, result);
                    return false;
                }

                ApplyOperationOutput(result.Output);
                return true;
            }
            finally
            {
                _operationCts = null;
                cts.Dispose();
                _operationGate.Release();
            }
        }

        private void ShowOperationFailure(string operationId, OperationResult result)
        {
            if (result.ErrorCode == JSharp.Domain.Operations.ErrorCodes.Cancelled)
            {
                return; // silent by design
            }

            string message = OperationResultPresenter.ToLocalizedMessage(result, _owner.SafeOperationName(operationId));
            _owner.MessageServiceForDialogs.ShowMessage(message, JSharp.Shared.Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
