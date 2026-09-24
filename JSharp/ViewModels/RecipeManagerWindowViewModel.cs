using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Domain.Recipes;
using JSharp.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace JSharp.ViewModels
{
    internal sealed class RecipeListRow(string path, string name, int stepCount, DateTime modified, bool readable, int inputCount = 0)
    {
        public string Path { get; } = path;

        public string Name { get; } = name;

        public string Detail => readable
            ? inputCount > 0
                ? $"{stepCount} step(s) · {inputCount} input(s) · {modified:yyyy-MM-dd HH:mm}"
                : $"{stepCount} step(s) · {modified:yyyy-MM-dd HH:mm}"
            : "unreadable recipe file";

        public bool CanRun => readable;

        public bool CanEdit => readable;
    }

    /// <summary>
    /// Viewmodel for <see cref="JSharp.UI.Views.RecipeManagerWindow"/>.
    /// Catalog hub: lists stored recipes and delegates run/edit to the shell.
    /// Works on DTOs and file paths only; image execution stays in MainWindow.
    /// </summary>
    internal sealed partial class RecipeManagerWindowViewModel : ObservableObject
    {
        private readonly RecipeStore _store;
        private readonly IMessageService _messages;
        private readonly Func<string, Task> _runRecipe;
        private readonly Action<string> _openEditor;
        private readonly Func<string, string?> _promptName;

        public ObservableCollection<RecipeListRow> Rows { get; } = new();

        public RecipeListRow? SelectedRow
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        public RecipeManagerWindowViewModel(
            RecipeStore store,
            IMessageService messages,
            Func<string, Task> runRecipe,
            Action<string> openEditor,
            Func<string, string?> promptName)
        {
            _store = store;
            _messages = messages;
            _runRecipe = runRecipe;
            _openEditor = openEditor;
            _promptName = promptName;
            Refresh();
        }

        internal void Refresh()
        {
            Rows.Clear();
            foreach (string path in _store.List().OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
            {
                try
                {
                    RecipeDocument doc = _store.Load(path);
                    Rows.Add(new RecipeListRow(path, doc.Name, doc.Steps.Count,
                        File.GetLastWriteTime(path), readable: true, inputCount: doc.Inputs?.Count ?? 0));
                }
                catch (RecipeFormatException)
                {
                    Rows.Add(new RecipeListRow(path,
                        System.IO.Path.GetFileNameWithoutExtension(path),
                        0, File.GetLastWriteTime(path), readable: false));
                }
            }
        }

        [RelayCommand]
        private async Task Run(RecipeListRow? row)
        {
            if (row?.CanRun == true)
            {
                await _runRecipe(row.Path).ConfigureAwait(true);
            }
        }

        [RelayCommand]
        private void Edit(RecipeListRow? row)
        {
            if (row?.CanEdit == true)
            {
                _openEditor(row.Path);
            }
        }

        [RelayCommand]
        private void Delete(RecipeListRow? row)
        {
            if (row is null)
            {
                return;
            }

            MessageBoxResult answer = _messages.ShowMessage(
                $"Delete recipe '{row.Name}'?", "Recipes",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (answer != MessageBoxResult.Yes)
            {
                return;
            }

            _store.Delete(row.Path);
            Refresh();
        }

        [RelayCommand]
        private void Rename(RecipeListRow? row)
        {
            if (row?.CanEdit != true)
            {
                return;
            }

            string? name = _promptName(row.Name);
            if (name is null)
            {
                return;
            }

            try
            {
                _store.Rename(row.Path, name);
                Refresh();
            }
            catch (RecipeFormatException ex)
            {
                _messages.ShowMessage(ex.Message, "Recipes", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task OpenFromFile()
        {
            OpenFileDialog dlg = new()
            {
                Filter = $"JSharp Recipe (*{RecipePaths.DefaultExtension})|*{RecipePaths.DefaultExtension}|Recipe Bundle (*{RecipePaths.BundleExtension})|*{RecipePaths.BundleExtension}",
                Title = "Run recipe from file...",
            };
            if (dlg.ShowDialog() != true)
            {
                return;
            }

            await RunFromPath(dlg.FileName).ConfigureAwait(true);
        }

        internal Task RunFromPath(string path) => _runRecipe(path);

        [RelayCommand]
        private void Share(RecipeListRow? row)
        {
            if (row?.CanEdit != true)
            {
                return;
            }

            SaveFileDialog dlg = new()
            {
                Filter = $"Recipe Bundle (*{RecipePaths.BundleExtension})|*{RecipePaths.BundleExtension}",
                FileName = row.Name,
                Title = "Share recipe as bundle...",
            };
            if (dlg.ShowDialog() != true)
            {
                return;
            }

            ShareTo(row, dlg.FileName);
        }

        internal void ShareTo(RecipeListRow row, string zipPath)
        {
            try
            {
                _store.ExportBundle(row.Path, zipPath);
                _messages.ShowMessage($"Bundle saved:{Environment.NewLine}{zipPath}",
                    "Recipes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (RecipeFormatException ex)
            {
                _messages.ShowMessage(ex.Message, "Recipes", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ImportBundle()
        {
            OpenFileDialog dlg = new()
            {
                Filter = $"Recipe Bundle (*{RecipePaths.BundleExtension})|*{RecipePaths.BundleExtension}",
                Title = "Import bundle...",
            };
            if (dlg.ShowDialog() != true)
            {
                return;
            }

            string? name = _promptName(System.IO.Path.GetFileNameWithoutExtension(dlg.FileName));
            if (name is null)
            {
                return;
            }

            ImportFrom(dlg.FileName, name);
        }

        internal void ImportFrom(string zipPath, string name)
        {
            try
            {
                string path = _store.ImportBundle(zipPath, name);
                Refresh();
                _messages.ShowMessage($"Bundle imported:{Environment.NewLine}{path}",
                    "Recipes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (RecipeFormatException ex)
            {
                _messages.ShowMessage(ex.Message, "Recipes", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void RefreshList() => Refresh();
    }
}
