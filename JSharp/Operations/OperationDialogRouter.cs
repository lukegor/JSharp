using JSharp.Domain.Models.SimpleDataModels;
using JSharp.Domain.Operations;
using JSharp.Domain.Services;
using JSharp.Services;
using JSharp.Shared.Imaging;
using JSharp.Shared.Resources;
using JSharp.UI.Views;
using JSharp.Utility.Utility;
using JSharp.ViewModels;
using JSharp.ViewModels.Abstractions;
using OpenCvSharp;
using System.Windows;

namespace JSharp.Operations
{
    /// <summary>
    /// Centralizes every parameter dialog. Value dialogs run modally: the VM's selection
    /// event captures the plan and closes the window. The two live-preview thresholder
    /// flows self-apply on confirm and report HandledByCollector.
    /// </summary>
    internal sealed class OperationDialogRouter : IOperationDialogRouter
    {
        private readonly Func<MainWindowViewModel> _mainVm;
        private readonly IDialogService _dialogs;
        private readonly IMessageService _messages;

        public OperationDialogRouter(Func<MainWindowViewModel> mainVm, IDialogService dialogs, IMessageService messages)
        {
            _mainVm = mainVm;
            _dialogs = dialogs;
            _messages = messages;
        }

        public Task<OperationExecutionPlan?> TryCollectAsync(string operationId)
        {
            OperationExecutionPlan? plan = operationId switch
            {
                "point.negate" or "point.stretch-histogram" or "threshold.adaptive"
                    or "segment.skeletonize" or "segment.hough" or "segment.watershed"
                    or "geometry.rotate-90" or "geometry.flip-180" =>
                    new OperationExecutionPlan(NoParams.Default),

                "color.convert-grayscale" =>
                    new OperationExecutionPlan(new ConvertColorParams(ColorSpaceType.Grayscale), NewColorSpace: ColorSpaceType.Grayscale, OperationId: "color.convert"),
                "color.convert-hsv" =>
                    new OperationExecutionPlan(new ConvertColorParams(ColorSpaceType.HSV), NewColorSpace: ColorSpaceType.HSV, ThenSplitChannels: true, OperationId: "color.convert"),
                "color.convert-lab" =>
                    new OperationExecutionPlan(new ConvertColorParams(ColorSpaceType.LAB), NewColorSpace: ColorSpaceType.LAB, ThenSplitChannels: true, OperationId: "color.convert"),

                "point.equalize-histogram" => CollectEqualizeHistogram(),
                "point.posterize" => CollectPosterize(),
                "point.stretch-contrast" => CollectStretchContrast(),
                "filter.median" => CollectMedian(),
                "filter.convolve" => CollectConvolver(),
                "filter.double-convolution" => CollectDoubleConvolution(),
                "morph.erode" => CollectMorphology(),
                "morph.dilate" => CollectMorphology(),
                "morph.open" => CollectMorphology(),
                "morph.close" => CollectMorphology(),
                "geometry.pyramid-up" => CollectPyramid(),
                "geometry.pyramid-down" => CollectPyramid(),
                "calc.add" or "calc.subtract" or "calc.and" or "calc.or" or "calc.xor" or "calc.not" or "calc.blend" or "calc.dialog" => CollectCalculator(),
                "segment.inpaint" => CollectInpaint(),
                "segment.grabcut" => CollectGrabCut(),

                "threshold.simple" => StartLivePreviewOneThreshold(),
                "threshold.dual" => StartLivePreviewTwoThresholds(),

                _ => throw new InvalidOperationException($"No parameter collector registered for '{operationId}'."),
            };

            return Task.FromResult<OperationExecutionPlan?>(plan);
        }

        private OperationExecutionPlan CollectEqualizeHistogram()
        {
            var focused = _mainVm().FocusedImageVm!;
            return new OperationExecutionPlan(HistogramParamsFactory.FromImage(focused.MatImage));
        }

        private OperationExecutionPlan? CollectPosterize()
        {
            PosterizeWindowViewModel vm = new();

            (bool confirmed, int levels) = _dialogs.ShowDialog<PosterizeWindowViewModel, int>(vm);

            return confirmed ? new OperationExecutionPlan(new PosterizeParams(levels)) : null;
        }

        private OperationExecutionPlan? CollectStretchContrast()
        {
            StretchContrastWindowViewModel vm = new();

            (bool confirmed, StretchContrastParams parameters) = _dialogs.ShowDialog<StretchContrastWindowViewModel, StretchContrastParams>(vm);

            return confirmed ? new OperationExecutionPlan(parameters) : null;
        }

        private OperationExecutionPlan? CollectMedian()
        {
            MedianWindowViewModel vm = new();

            (bool confirmed, int matrixSize) = _dialogs.ShowDialog<MedianWindowViewModel, int>(vm);

            return confirmed ? new OperationExecutionPlan(new MedianFilterParams(matrixSize)) : null;
        }

        private OperationExecutionPlan? CollectConvolver()
        {
            ConvolverWindowViewModel vm = new(_dialogs);

            (bool confirmed, ConvolutionInfo info) = _dialogs.ShowDialog<ConvolverWindowViewModel, ConvolutionInfo>(vm);
            if (!confirmed)
            {
                return null;
            }

            BorderMode borderType = info.BorderPixelsOption;
            string executorId;
            OperationParams parameters;
            if (vm.CurrentKernel == Kernels.BoxBlur)
            {
                executorId = "filter.blur";
                parameters = new BlurParams(3, borderType);
            }
            else if (vm.CurrentKernel == Kernels.GaussianBlur)
            {
                executorId = "filter.gaussian-blur";
                parameters = new GaussianBlurParams(3, 1.5, 1.5, borderType);
            }
            else if (vm.CurrentKernel == Kernels.SobelEW || vm.CurrentKernel == Kernels.SobelNS || vm.CurrentKernel == Kernels.Canny || vm.CurrentKernel == Kernels.Laplacian)
            {
                executorId = "filter.edge-detection";
                string kernelName = EdgeKernelNames.TryToInvariant(vm.CurrentKernel, out string invariantKernel)
                    ? invariantKernel
                    : vm.CurrentKernel;
                parameters = new EdgeDetectionParams(kernelName, borderType, info.Min, info.Max);
            }
            else
            {
                float[,] kernel = new float[3, 3];
                int index = 0;
                foreach (int value in vm.TextBoxValues)
                {
                    kernel[index / 3, index % 3] = value;
                    index++;
                }

                executorId = "filter.custom-kernel";
                parameters = new CustomKernelParams(kernel, borderType, 0);
            }

            return new OperationExecutionPlan(parameters, OperationId: executorId);
        }

        private OperationExecutionPlan? CollectDoubleConvolution()
        {
            DoubleConvolverWindowViewModel vm = new();

            (bool confirmed, DoubleConvolutionParams parameters) = _dialogs.ShowDialog<DoubleConvolverWindowViewModel, DoubleConvolutionParams>(vm);

            return confirmed ? new OperationExecutionPlan(parameters) : null;
        }

        private OperationExecutionPlan? CollectMorphology()
        {
            StandardMorphologicalWindowViewModel vm = new();

            (bool confirmed, MorphologySelection selection) = _dialogs.ShowDialog<StandardMorphologicalWindowViewModel, MorphologySelection>(vm);
            if (!confirmed)
            {
                return null;
            }

            using Mat element = selection.Shape switch
            {
                ShapeType.Rectangle => Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(selection.ElementSize, selection.ElementSize), new OpenCvSharp.Point(-1, -1)),
                ShapeType.Rhombus => StructuringElements.Diamond(selection.ElementSize),
                _ => throw new InvalidOperationException("Unknown shape requested"),
            };

            return new OperationExecutionPlan(
                new MorphologyParams(element.Clone(), selection.Border, new Scalar(selection.BorderValue)),
                Morphology: new MorphologySpec(selection.Shape, selection.ElementSize, selection.Border, selection.BorderValue));
        }

        private OperationExecutionPlan? CollectPyramid()
        {
            PyramidWindowViewModel vm = new();

            (bool confirmed, int effectSize) = _dialogs.ShowDialog<PyramidWindowViewModel, int>(vm);
            if (!confirmed)
            {
                return null;
            }

            int repeat = effectSize switch
            {
                2 => 1,
                4 => 2,
                _ => throw new InvalidOperationException("Unknown operation requested"),
            };

            return new OperationExecutionPlan(
                NoParams.Default,
                Repeat: repeat);
        }

        private OperationExecutionPlan? CollectCalculator()
        {
            MainWindowViewModel main = _mainVm();
            if (main.OpenImageWindows.Count == 0)
            {
                return null;
            }

            IEnumerable<ImageInfo> imageInfoList = main.OpenImageWindows
                .Select(vm => new ImageInfo(vm.MatImage, vm.FileName))
                .ToArray();

            ImageCalculatorWindowViewModel vm = new(imageInfoList);

            (bool confirmed, ImageCalculatorInfo? info) = _dialogs.ShowDialog<ImageCalculatorWindowViewModel, ImageCalculatorInfo>(vm);
            if (!confirmed || info is null)
            {
                return null;
            }

            Mat img1 = info.Image1;
            Mat img2 = info.Image2;

            if (img1.Channels() != Constants.Grayscale_ChannelCount || img2.Channels() != Constants.Grayscale_ChannelCount)
            {
                main.ShowCalculatorGrayscaleError();
                return null;
            }

            if (img1.Width != img2.Width || img1.Height != img2.Height)
            {
                _messages.ShowMessage(
                    Errors.ImagesMustBeOfSameSize, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            OperationParams parameters;
            string executorId;
            switch (info.OperationData.Operation)
            {
                case OperationType.ADD:
                    executorId = "calc.add";
                    parameters = new BinaryImageParams(img2);
                    break;
                case OperationType.SUB:
                    executorId = "calc.subtract";
                    parameters = new BinaryImageParams(img2);
                    break;
                case OperationType.AND:
                    executorId = "calc.and";
                    parameters = new BinaryImageParams(img2);
                    break;
                case OperationType.OR:
                    executorId = "calc.or";
                    parameters = new BinaryImageParams(img2);
                    break;
                case OperationType.XOR:
                    executorId = "calc.xor";
                    parameters = new BinaryImageParams(img2);
                    break;
                case OperationType.BLEND:
                    executorId = "calc.blend";
                    parameters = new BlendParams(img2, info.OperationData.BlendFactor1 ?? 0.5);
                    break;
                case OperationType.NOT:
                    executorId = "calc.not";
                    parameters = NoParams.Default;
                    break;
                default: throw new InvalidOperationException("Unknown operation requested");
            }

            return new OperationExecutionPlan(parameters, OperationId: executorId, CreateNewWindow: info.ShouldCreateNewWindow);
        }

        private OperationExecutionPlan? CollectInpaint()
        {
            MainWindowViewModel main = _mainVm();
            if (main.OpenImageWindows.Count < 2)
            {
                main.ShowTwoImagesRequiredError();
                return null;
            }

            List<ImageInfo> imageInfoList = main.OpenImageWindows
                .Select(vm => new ImageInfo(vm.MatImage, vm.FileName))
                .ToList();

            InpaintWindowViewModel vm = new(_messages, imageInfoList);

            (bool confirmed, InpaintSelection? selection) = _dialogs.ShowDialog<InpaintWindowViewModel, InpaintSelection>(vm);
            if (!confirmed || selection is null)
            {
                return null;
            }

            return new OperationExecutionPlan(
                new InpaintParams(selection.Mask, selection.Radius),
                OperationId: "segment.inpaint");
        }

        private OperationExecutionPlan? CollectGrabCut()
        {
            var points = _mainVm().FocusedImageVm!.Points.ToArray();
            if (points[0] == null || points[1] == null)
            {
                _mainVm().ShowTwoPointsRequiredError();
                return null;
            }

            System.Windows.Point start = points[0]!.Value;
            System.Windows.Point end = points[1]!.Value;
            int x = (int)Math.Min(start.X, end.X);
            int y = (int)Math.Min(start.Y, end.Y);
            int width = (int)Math.Abs(end.X - start.X);
            int height = (int)Math.Abs(end.Y - start.Y);

            return new OperationExecutionPlan(
                new GrabCutParams(new OpenCvSharp.Rect(x, y, width, height)));
        }

        private OperationExecutionPlan StartLivePreviewOneThreshold()
        {
            var focused = _mainVm().FocusedImageVm!;
            Mat image = focused.MatImage.Clone();
            SimpleThresholderWindowViewModel vm = null!;
            vm = new SimpleThresholderWindowViewModel(
                image,
                preview: () => focused.PreviewSimpleThresholding(image, vm.FromValue, vm.Thresholding));

            _dialogs.Show<SimpleThresholderWindowViewModel, SimpleThresholdParams>(
                vm,
                result => focused.PerformSimpleThresholding(result.Threshold, result.Method),
                onDismissed: () => focused.DiscardPreview());

            return new OperationExecutionPlan(null, HandledByCollector: true);
        }

        private OperationExecutionPlan StartLivePreviewTwoThresholds()
        {
            var focused = _mainVm().FocusedImageVm!;
            Mat image = focused.MatImage.Clone();
            ThresholderWindowViewModel vm = null!;
            vm = new ThresholderWindowViewModel(
                image,
                preview: () => focused.PreviewThresholding(image, vm.FromValue, vm.ToValue, vm.Thresholding, vm.EnableContrastMode));

            _dialogs.Show<ThresholderWindowViewModel, DualThresholdParams>(
                vm,
                result => focused.PerformThresholding(result.MinThreshold, result.MaxThreshold, result.Mode, result.EnableContrastMode),
                onDismissed: () => focused.DiscardPreview());

            return new OperationExecutionPlan(null, HandledByCollector: true);
        }

        private static float[,] ToMatrix(IEnumerable<int> values, int size)
        {
            float[,] matrix = new float[size, size];
            int index = 0;
            foreach (int value in values)
            {
                matrix[index / size, index % size] = value;
                index++;
            }

            return matrix;
        }
    }
}
