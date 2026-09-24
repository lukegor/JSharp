using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Domain.Recipes;
using JSharp.Services;
using JSharp.Utility.Utility;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Windows;

namespace JSharp.ViewModels
{
    internal sealed class ScalarField(string label, string text) : ObservableObject
    {
        public string Label { get; } = label;

        public string Text
        {
            get => field;
            set => SetProperty(ref field, value);
        } = text;

        public string? Error
        {
            get => field;
            set => SetProperty(ref field, value);
        }
    }

    internal sealed class RecipeStepRow(RecipeStep step) : ObservableObject
    {
        public RecipeStep Step
        {
            get => field;
            internal set => SetProperty(ref field, value);
        } = step;

        public bool Enabled
        {
            get => Step.Enabled;
            set { Step = Step with { Enabled = value }; OnPropertyChanged(); }
        }

        public ObservableCollection<ScalarField> Fields { get; } = new();

        public string ParamsSummary => Step.Params switch
        {
            NoneParamsDto => "no parameters",
            ConvertColorParamsDto p => $"target={p.Target}",
            PosterizeParamsDto p => $"levels={p.Levels}",
            StretchContrastParamsDto p => $"p1={p.P1} p2={p.P2} q3={p.Q3} q4={p.Q4}",
            EqualizeParamsDto => "recomputed from image",
            MedianParamsDto p => $"size={p.KernelSize}",
            BlurParamsDto p => $"size={p.KernelSize} border={p.Border}",
            GaussianBlurParamsDto p => $"size={p.KernelSize} sx={p.SigmaX} sy={p.SigmaY}",
            CustomKernelParamsDto p => $"kernel={p.Kernel.Length}x{p.Kernel[0].Length}",
            DoubleConvolutionParamsDto p => $"kernels={p.Kernel1.Length}x{p.Kernel1[0].Length}",
            EdgeDetectionParamsDto p => $"kernel={p.KernelName}",
            MorphologyParamsDto p => $"{p.Shape} size={p.ElementSize}",
            SimpleThresholdParamsDto p => $"t={p.Threshold} {p.Method}",
            DualThresholdParamsDto p => $"min={p.MinThreshold} max={p.MaxThreshold} {p.Mode}",
            BinaryImageParamsDto p => $"second={p.Second.Slot}",
            BlendParamsDto p => $"second={p.Second.Slot} w={p.Weight1}",
            InpaintParamsDto p => $"mask={p.Mask.Slot} r={p.Radius}",
            GrabCutParamsDto p => $"rect={p.X},{p.Y},{p.Width}x{p.Height}",
            _ => Step.Params.GetType().Name,
        };
    }

    /// <summary>
    /// Viewmodel for <see cref="JSharp.UI.Views.RecipeEditorWindow"/>.
    /// Works on DTOs only (no Mats): reorder, enable/disable, delete, scalar
    /// edits with validation, aux rebind, save. Kernel/morphology/aux steps
    /// expose reorder/enable/delete/rebind; changing their values means re-recording.
    /// </summary>
    internal sealed partial class RecipeEditorWindowViewModel : ObservableObject
    {
        private RecipeDocument _doc;
        private readonly string _recipePath;
        private readonly RecipeStore _store;
        private readonly IMessageService _messages;

        public ObservableCollection<RecipeStepRow> Rows { get; } = new();

        public RecipeStepRow? SelectedRow
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        public string RecipeName => _doc.Name;

        public RecipeEditorWindowViewModel(
            RecipeDocument doc, string recipePath, RecipeStore store, IMessageService messages)
        {
            _doc = doc;
            _recipePath = recipePath;
            _store = store;
            _messages = messages;

            foreach (RecipeStep step in doc.Steps)
            {
                RecipeStepRow row = new(step);
                BuildFields(row);
                Rows.Add(row);
            }
        }

        internal void BuildFields(RecipeStepRow row)
        {
            row.Fields.Clear();
            switch (row.Step.Params)
            {
                case PosterizeParamsDto p: row.Fields.Add(new("Levels", p.Levels.ToString())); break;
                case StretchContrastParamsDto p:
                    row.Fields.Add(new("P1", p.P1.ToString()));
                    row.Fields.Add(new("P2", p.P2.ToString()));
                    row.Fields.Add(new("Q3", p.Q3.ToString()));
                    row.Fields.Add(new("Q4", p.Q4.ToString()));
                    break;
                case MedianParamsDto p: row.Fields.Add(new("KernelSize", p.KernelSize.ToString())); break;
                case BlurParamsDto p: row.Fields.Add(new("KernelSize", p.KernelSize.ToString())); break;
                case GaussianBlurParamsDto p:
                    row.Fields.Add(new("KernelSize", p.KernelSize.ToString()));
                    row.Fields.Add(new("SigmaX", p.SigmaX.ToString(CultureInfo.InvariantCulture)));
                    row.Fields.Add(new("SigmaY", p.SigmaY.ToString(CultureInfo.InvariantCulture)));
                    break;
                case SimpleThresholdParamsDto p: row.Fields.Add(new("Threshold", p.Threshold.ToString())); break;
                case DualThresholdParamsDto p:
                    row.Fields.Add(new("Min", p.MinThreshold.ToString()));
                    row.Fields.Add(new("Max", p.MaxThreshold.ToString()));
                    break;
                case BlendParamsDto p:
                    row.Fields.Add(new("Weight", p.Weight1.ToString(CultureInfo.InvariantCulture)));
                    break;
                case InpaintParamsDto p: row.Fields.Add(new("Radius", p.Radius.ToString())); break;
            }

            row.Fields.Add(new("Repeat", row.Step.Repeat.ToString()));
        }

        internal bool TryApplyFields(RecipeStepRow row)
        {
            bool ok = true;
            int Int(string label, int min, int max, Func<int, bool>? extra = null)
            {
                ScalarField f = row.Fields.First(x => x.Label == label);
                if (!int.TryParse(f.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v)
                    || v < min || v > max || (extra is not null && !extra(v)))
                {
                    f.Error = extra is not null
                        ? $"{label} must be an odd integer {min}..{max}."
                        : $"{label} must be an integer {min}..{max}.";
                    ok = false;
                }
                else
                {
                    f.Error = null;
                }

                return v;
            }

            double Double(string label, double min, double max)
            {
                ScalarField f = row.Fields.First(x => x.Label == label);
                if (!double.TryParse(f.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double v)
                    || v < min || v > max)
                {
                    f.Error = $"{label} must be a number {min}..{max}.";
                    ok = false;
                }
                else
                {
                    f.Error = null;
                }

                return v;
            }

            RecipeParamsDto p = row.Step.Params;
            int repeat = Int("Repeat", 1, 9);
            p = p switch
            {
                PosterizeParamsDto => new PosterizeParamsDto(Int("Levels", 2, 255)),
                StretchContrastParamsDto => new StretchContrastParamsDto(
                    Int("P1", 0, 255), Int("P2", 0, 255), Int("Q3", 0, 255), Int("Q4", 0, 255)),
                MedianParamsDto => new MedianParamsDto(Int("KernelSize", 3, 255, v => v % 2 == 1)),
                BlurParamsDto b => new BlurParamsDto(Int("KernelSize", 1, 255, v => v % 2 == 1), b.Border),
                GaussianBlurParamsDto g => new GaussianBlurParamsDto(
                    Int("KernelSize", 1, 255, v => v % 2 == 1),
                    Double("SigmaX", 0.01, 100), Double("SigmaY", 0.01, 100), g.Border),
                SimpleThresholdParamsDto s => new SimpleThresholdParamsDto(Int("Threshold", 0, 255), s.Method),
                DualThresholdParamsDto d => new DualThresholdParamsDto(
                    Int("Min", 0, 255), Int("Max", 0, 255), d.Mode, d.EnableContrastMode),
                BlendParamsDto b => new BlendParamsDto(b.Second, Double("Weight", 0, 1)),
                InpaintParamsDto i => new InpaintParamsDto(i.Mask, Int("Radius", 1, 20)),
                _ => p,
            };

            if (ok)
            {
                row.Step = row.Step with { Params = p, Repeat = repeat };
            }

            return ok;
        }

        internal void MoveRowUp(RecipeStepRow row) => Move(row, -1);

        internal void MoveRowDown(RecipeStepRow row) => Move(row, 1);

        private void Move(RecipeStepRow row, int delta)
        {
            int i = Rows.IndexOf(row);
            int j = i + delta;
            if (i < 0 || j < 0 || j >= Rows.Count)
            {
                return;
            }

            Rows.Move(i, j);
        }

        internal void DeleteRow(RecipeStepRow row)
        {
            Rows.Remove(row);
            if (ReferenceEquals(SelectedRow, row))
            {
                SelectedRow = null;
            }
        }

        [RelayCommand]
        private void MoveUp(RecipeStepRow? row)
        {
            if (row is not null)
            {
                MoveRowUp(row);
            }
        }

        [RelayCommand]
        private void MoveDown(RecipeStepRow? row)
        {
            if (row is not null)
            {
                MoveRowDown(row);
            }
        }

        [RelayCommand]
        private void Delete(RecipeStepRow? row)
        {
            if (row is not null)
            {
                DeleteRow(row);
            }
        }

        [RelayCommand]
        private void ApplyFields(RecipeStepRow? row)
        {
            if (row is not null)
            {
                TryApplyFields(row);
            }
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                RecipeDocument snapshot = _doc with { Steps = Rows.Select(r => r.Step).ToList() };
                _store.SaveOverwrite(snapshot, _recipePath);
                _doc = snapshot;
            }
            catch (Exception ex)
            {
                _messages.ShowMessage(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void RebindAux(RecipeStepRow? row)
        {
            if (row is null)
            {
                return;
            }

            IReadOnlyList<ImageRef> refs = RecipeParamConverter.GetAuxRefs(row.Step.Params);
            if (refs.Count == 0)
            {
                return;
            }

            ImageRef current = refs[0];

            OpenFileDialog dlg = new()
            {
                Filter = Constants.ImageFilterString,
                Title = $"Rebind '{current.Slot}'...",
            };
            if (dlg.ShowDialog() != true)
            {
                return;
            }

            try
            {
                byte[] bytes = File.ReadAllBytes(dlg.FileName);
                string sha = AuxFiles.Sha256Hex(bytes);
                string? recipeDir = Path.GetDirectoryName(_recipePath);
                string auxDir = Path.Combine(recipeDir!, "aux");
                Directory.CreateDirectory(auxDir);
                File.Copy(dlg.FileName, Path.Combine(auxDir, $"{sha}.png"), overwrite: true);

                ImageRef rebound = new(current.Slot, $"aux/{sha}.png", sha, null);
                row.Step = row.Step.Params switch
                {
                    BinaryImageParamsDto => row.Step with { Params = new BinaryImageParamsDto(rebound) },
                    BlendParamsDto b => row.Step with { Params = new BlendParamsDto(rebound, b.Weight1) },
                    InpaintParamsDto i => row.Step with { Params = new InpaintParamsDto(rebound, i.Radius) },
                    _ => row.Step,
                };

                List<AuxEntry> aux = _doc.Aux.Where(a => a.Slot != current.Slot).ToList();
                aux.Add(new AuxEntry(current.Slot, $"aux/{sha}.png", sha,
                    current.Slot == "mask" ? "mask" : "secondImage"));
                _doc = _doc with { Aux = aux };
            }
            catch (Exception ex)
            {
                _messages.ShowMessage(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
