using System.IO;
using System.IO.Compression;
using System.Text;
using JSharp.Domain.Operations;
using JSharp.Domain.Recipes;

namespace JSharp.Services
{
    /// <summary>
    /// Persists recipe drafts (JSON + sidecar aux folder), imports/exports
    /// portable bundles, and lists/deletes stored recipes.
    /// </summary>
    public sealed class RecipeStore(IOperationCatalog catalog, string baseDirectory)
    {
        public IReadOnlyList<string> List() =>
            Directory.Exists(baseDirectory)
                ? Directory.GetFiles(baseDirectory, $"*{RecipePaths.DefaultExtension}", SearchOption.AllDirectories)
                : Array.Empty<string>();

        public RecipeDocument Load(string path) => RecipeJson.Read(File.ReadAllText(path));

        public string Save(RecipeDocument doc, string name, string stagingDirectory)
        {
            Directory.CreateDirectory(baseDirectory);
            StringBuilder safeName = new(name.Length);
            foreach (char c in name)
            {
                safeName.Append(char.IsLetterOrDigit(c) || c is '-' or '_' ? c : '_');
            }

            string dir = Path.Combine(baseDirectory, safeName.ToString());
            Directory.CreateDirectory(dir);
            string jsonPath = Path.Combine(dir, safeName.ToString() + RecipePaths.DefaultExtension);

            RecipeDocument stamped = doc with
            {
                Name = name,
                CreatedUtc = DateTime.UtcNow,
                AppVersion = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0.0.0",
                CatalogVersion = RecipeCatalogVersion.For(catalog.Descriptors.Select(d => d.Id)),
            };
            File.WriteAllText(jsonPath, RecipeJson.Write(stamped));

            string auxDir = Path.Combine(dir, "aux");
            Directory.CreateDirectory(auxDir);
            foreach (string staged in Directory.GetFiles(stagingDirectory, "*.png"))
            {
                File.Copy(staged, Path.Combine(auxDir, Path.GetFileName(staged)), overwrite: true);
            }

            return jsonPath;
        }

        public string SaveOverwrite(RecipeDocument doc, string recipeJsonPath)
        {
            File.WriteAllText(recipeJsonPath, RecipeJson.Write(doc));
            return recipeJsonPath;
        }

        public string ExportBundle(string recipeJsonPath, string zipPath)
        {
            string? dir = Path.GetDirectoryName(recipeJsonPath)
                ?? throw new RecipeFormatException("Recipe path has no directory.");
            using (ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(recipeJsonPath, "recipe.json");
                string auxDir = Path.Combine(dir, "aux");
                if (Directory.Exists(auxDir))
                {
                    foreach (string png in Directory.GetFiles(auxDir, "*.png"))
                    {
                        zip.CreateEntryFromFile(png, $"aux/{Path.GetFileName(png)}");
                    }
                }
            }

            return zipPath;
        }

        public string ImportBundle(string zipPath, string destinationName)
        {
            string temp = Path.Combine(Path.GetTempPath(), "jsharp-bundle-" + Guid.NewGuid());
            Directory.CreateDirectory(temp);
            string root = BundleAuxResolver.ExtractWithCaps(zipPath, temp);
            RecipeDocument doc = RecipeJson.Read(File.ReadAllText(root));

            string dir = Path.Combine(baseDirectory, destinationName);
            string finalPath = Path.Combine(dir, destinationName + RecipePaths.DefaultExtension);
            if (File.Exists(finalPath))
            {
                throw new RecipeFormatException($"A recipe named '{destinationName}' already exists.");
            }

            Directory.CreateDirectory(dir);
            string dstAux = Path.Combine(dir, "aux");
            Directory.CreateDirectory(dstAux);
            string srcAux = Path.Combine(temp, "aux");
            if (Directory.Exists(srcAux))
            {
                foreach (string png in Directory.GetFiles(srcAux, "*.png"))
                {
                    File.Copy(png, Path.Combine(dstAux, Path.GetFileName(png)), overwrite: true);
                }
            }

            RecipeDocument normalized = NormalizeImported(doc, dstAux) with { Name = destinationName };
            File.WriteAllText(finalPath, RecipeJson.Write(normalized));
            return finalPath;
        }

        private static RecipeDocument NormalizeImported(RecipeDocument doc, string auxDir)
        {
            List<RecipeStep> steps = new();
            List<AuxEntry> aux = new();
            foreach (RecipeStep step in doc.Steps)
            {
                RecipeParamsDto p = step.Params;
                foreach (ImageRef current in RecipeParamConverter.GetAuxRefs(p))
                {
                    string fileName = current.EmbeddedId is not null
                        ? $"{current.EmbeddedId}.png"
                        : Path.GetFileName(current.File ?? throw new RecipeFormatException(
                            $"Step '{step.OperationId}' has an empty aux reference for slot '{current.Slot}'."));
                    string full = Path.Combine(auxDir, fileName);
                    if (!File.Exists(full))
                    {
                        throw new RecipeFormatException($"Bundle aux file is missing: aux/{fileName}.");
                    }

                    string sha = AuxFiles.Sha256HexOfFile(full);
                    ImageRef normalized = new(current.Slot, $"aux/{fileName}", sha, null);
                    p = (p, current.Slot) switch
                    {
                        (BinaryImageParamsDto, _) => new BinaryImageParamsDto(normalized),
                        (BlendParamsDto b, _) => new BlendParamsDto(normalized, b.Weight1),
                        (InpaintParamsDto inp, _) => new InpaintParamsDto(normalized, inp.Radius),
                        _ => p,
                    };
                    if (aux.All(a => a.Slot != current.Slot))
                    {
                        aux.Add(new AuxEntry(current.Slot, $"aux/{fileName}", sha,
                            current.Slot == "mask" ? "mask" : "secondImage"));
                    }
                }

                steps.Add(step with { Params = p });
            }

            return doc with { Steps = steps, Aux = aux };
        }

        public void Delete(string recipeJsonPath)
        {
            string? dir = Path.GetDirectoryName(recipeJsonPath);
            if (dir is not null && Directory.Exists(dir))
            {
                Directory.Delete(dir, recursive: true);
            }
        }

        public string Rename(string recipeJsonPath, string newName)
        {
            string? dir = Path.GetDirectoryName(recipeJsonPath)
                ?? throw new RecipeFormatException("Recipe path has no directory.");
            StringBuilder safeName = new(newName.Length);
            foreach (char c in newName)
            {
                safeName.Append(char.IsLetterOrDigit(c) || c is '-' or '_' ? c : '_');
            }

            string destDir = Path.Combine(
                Path.GetDirectoryName(dir) ?? baseDirectory, safeName.ToString());
            if (Directory.Exists(destDir))
            {
                throw new RecipeFormatException($"A recipe named '{newName}' already exists.");
            }

            Directory.Move(dir, destDir);
            string destPath = Path.Combine(destDir, safeName + RecipePaths.DefaultExtension);
            RecipeDocument renamed = Load(Path.Combine(destDir,
                Path.GetFileName(recipeJsonPath))) with { Name = newName };
            File.WriteAllText(destPath, RecipeJson.Write(renamed));
            foreach (string stale in Directory.GetFiles(destDir, $"*{RecipePaths.DefaultExtension}"))
            {
                if (!stale.Equals(destPath, StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(stale);
                }
            }

            return destPath;
        }
    }
}
