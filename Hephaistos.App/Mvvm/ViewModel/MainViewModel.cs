using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hephaistos.App.Entities;
using Hephaistos.App.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;

namespace Hephaistos.App.Mvvm.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly SaveService saveService;

        [ObservableProperty]
        private string saveFile = "";

        [ObservableProperty]
        private string? rootDirectory;

        [ObservableProperty]
        private ObservableCollection<LineEntity> lines = [];

        [ObservableProperty]
        private ObservableCollection<RuleEntity> rules = [];

        [ObservableProperty]
        private ObservableCollection<string> rulesSavedFiles = [];

        [ObservableProperty]
        private string? selectedFile;

        public MainViewModel()
        {
            saveService = new SaveService();
            Rules = new(saveService.AutoLoad() ?? []);
            RulesSavedFiles = new(saveService.GetRulesFiles());
        }

        [RelayCommand]
        private void ClosingView()
        {
            saveService.AutoSave(Rules);
        }

        [RelayCommand]
        private void BrowseDirectory()
        {
            Microsoft.Win32.OpenFolderDialog dialog = new()
            {
                Multiselect = false,
                Title = "Select a folder"
            };

            if (dialog.ShowDialog() == true)
            {
                RootDirectory = dialog.FolderName;
            }
        }

        [RelayCommand]
        private void SaveRules()
        {
            if (SaveFile == "") return;
            saveService.SaveRules(SaveFile, Rules);
            RulesSavedFiles = new(saveService.GetRulesFiles());
        }

        [RelayCommand]
        private void LoadRules()
        {
            string? name = Path.GetFileNameWithoutExtension(SelectedFile);
            if (name == null) return;
            Rules = new(saveService.LoadRules(name));
            SaveFile = name;
        }

        [RelayCommand]
        private void LoadDirectoryContent()
        {
            if (!Directory.Exists(RootDirectory)) return;
            Lines = [];
            foreach (string element in Directory.GetFiles(RootDirectory))
            {
                Lines.Add(new LineEntity
                {
                    OldValue = Path.GetFileNameWithoutExtension(element),
                    NewValue = Path.GetFileNameWithoutExtension(element),
                    Extension = Path.GetExtension(element)
                });
            }

            foreach (string element in Directory.GetDirectories(RootDirectory))
            {
                Lines.Add(new LineEntity
                {
                    IsDirectory = true,
                    OldValue = Path.GetFileNameWithoutExtension(element),
                    NewValue = Path.GetFileNameWithoutExtension(element)
                });
            }
        }

        [RelayCommand]
        private void RefreshPreview()
        {
            if (Lines == null || Rules == null) return;
            foreach (LineEntity line in Lines)
            {
                if (line.OldValue == null) continue;
                string preview = line.OldValue;
                foreach (RuleEntity rule in Rules.Where(r => r.Pattern != ""))
                {
                    if (rule.IsRegex)
                    {
                        preview = Regex.Replace(preview, rule.Pattern, rule.Replacement);
                    }
                    else
                    {
                        preview = preview.Replace(rule.Pattern, rule.Replacement);
                    }
                }
                line.NewValue = preview;
            }
        }

        [RelayCommand]
        private void AddRule()
        {
            Rules.Add(new());
        }

        [RelayCommand]
        private void UpRule(int position)
        {
            if (position == 0) return;
            Rules.Move(position, position - 1);
        }

        [RelayCommand]
        private void DownRule(int position)
        {
            if (position == Rules.Count - 1) return;
            Rules.Move(position, position + 1);
        }

        [RelayCommand]
        private void RemoveRule(RuleEntity rule)
        {
            Rules.Remove(rule);
        }

        [RelayCommand]
        private void ApplyRules()
        {
            if (!Lines.Any() || RootDirectory == null) return;
            foreach (LineEntity element in Lines.Where(line => line.IsChecked))
            {
                if (element.OldValue == null || element.NewValue == null) continue;
                if (element.IsDirectory)
                {
                    Directory.Move(
                        Path.Combine(RootDirectory, element.OldValue),
                        Path.Combine(RootDirectory, element.NewValue)
                    );
                }
                else
                {
                    File.Move(
                        Path.Combine(RootDirectory, element.OldValue + element.Extension),
                        Path.Combine(RootDirectory, element.NewValue + element.Extension),
                        false
                    );
                }
            }
        }
    }
}