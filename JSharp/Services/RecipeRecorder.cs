using System.IO;
using JSharp.Domain.Operations;
using JSharp.Domain.Recipes;
using JSharp.Operations;
using OpenCvSharp;

namespace JSharp.Services
{
    /// <summary>
    /// Accumulates committed operations into a staged recipe draft while recording.
    /// Only successful commits are appended (previews and cancellations never reach
    /// it); aux images are content-addressed into staging at commit time so later
    /// window closes cannot invalidate the recipe.
    /// </summary>
    public sealed class RecipeRecorder
    {
        private readonly IOperationCatalog _catalog;
        private readonly string _stagingDirectory;

        public RecipeRecorder(IOperationCatalog catalog, string stagingDirectory)
        {
            _catalog = catalog;
            _stagingDirectory = stagingDirectory;
        }

        public bool IsRecording { get; private set; }

        public RecipeDocument? Draft { get; private set; }

        internal string StagingDirectory => _stagingDirectory;

        public void Start(string name)
        {
            Directory.CreateDirectory(_stagingDirectory);
            Draft = new RecipeDocument(1, name, DateTime.UtcNow, "app", "catalog",
                new List<RecipeStep>(), new List<AuxEntry>());
            IsRecording = true;
        }

        public void Stop() => IsRecording = false;

        public void Clear()
        {
            Draft = null;
            IsRecording = false;
        }

        public bool TryAppend(string operationId, string displayName, OperationExecutionPlan plan)
        {
            // Tier-1 rule: the recorder captures resolved concrete values only.
            // The inputs block is an authoring concern (editor/CLI generation, later slice)
            // and is never auto-created here.
            if (!IsRecording || Draft is null || plan.Parameters is null)
            {
                return false;
            }

            if (!_catalog.TryGetDescriptor(operationId, out _))
            {
                return false;
            }

            RecipeParamsDto dto = RecipeParamConverter.ToDto(plan.Parameters, plan.Morphology);
            if (dto is BinaryImageParamsDto || dto is BlendParamsDto || dto is InpaintParamsDto)
            {
                dto = StageAux(operationId, dto, plan.Parameters);
            }

            Draft.Steps.Add(new RecipeStep(Guid.NewGuid().ToString(), operationId, displayName,
                dto, Math.Max(1, plan.Repeat), plan.NewColorSpace?.ToString(), plan.ThenSplitChannels, true));
            return true;
        }

        private RecipeParamsDto StageAux(string operationId, RecipeParamsDto dto, OperationParams parameters)
        {
            IReadOnlyList<string> slots = RecipeParamConverter.GetAuxSlots(operationId);
            IReadOnlyList<Mat> mats = RecipeParamConverter.ExtractAuxMats(parameters);
            for (int i = 0; i < slots.Count && i < mats.Count; i++)
            {
                ImageRef staged = StageMat(slots[i], mats[i]);
                dto = (dto, staged.Slot) switch
                {
                    (BinaryImageParamsDto, _) => new BinaryImageParamsDto(staged),
                    (BlendParamsDto b, _) => new BlendParamsDto(staged, b.Weight1),
                    (InpaintParamsDto inp, _) => new InpaintParamsDto(staged, inp.Radius),
                    _ => dto,
                };
            }

            return dto;
        }

        private ImageRef StageMat(string slot, Mat mat)
        {
            Directory.CreateDirectory(_stagingDirectory);
            string tmp = Path.Combine(_stagingDirectory, Guid.NewGuid() + ".tmp.png");
            AuxFiles.WritePng(tmp, mat);
            string sha = AuxFiles.Sha256HexOfFile(tmp);
            string final = Path.Combine(_stagingDirectory, sha + ".png");
            if (File.Exists(final))
            {
                File.Delete(tmp);
            }
            else
            {
                File.Move(tmp, final);
            }

            string slotName = slot;
            int n = 2;
            while (Draft!.Aux.Any(a => a.Slot == slotName && a.Sha256 != sha))
            {
                slotName = $"{slot}-{n++}";
            }

            if (Draft.Aux.All(a => a.Slot != slotName))
            {
                Draft.Aux.Add(new AuxEntry(slotName, $"aux/{sha}.png", sha,
                    slot == "mask" ? "mask" : "secondImage"));
            }

            return new ImageRef(slotName, $"aux/{sha}.png", sha, null);
        }
    }
}
