using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hephaistos.App.Entities;
using Hephaistos.App.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Documents;

namespace Hephaistos.App.Mvvm.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly SaveService saveService;

        public IEnumerable<RuleType> RuleTypes { get; } = Enum.GetValues(typeof(RuleType)).Cast<RuleType>();

        [ObservableProperty]
        private ObservableCollection<LineEntity> linesUnfiltered;

        public ICollectionView LinesFiltered { get; }

        [ObservableProperty]
        private int countOfItems;

        [ObservableProperty]
        private string saveFile = "";

        [ObservableProperty]
        private string? rootDirectory;

        [ObservableProperty]
        private ObservableCollection<RuleEntity> rules = [];

        [ObservableProperty]
        private ObservableCollection<string> rulesSavedFiles = [];

        [ObservableProperty]
        private string? selectedFile;

        public MainViewModel()
        {
            LinesUnfiltered = new();
            LinesFiltered = CollectionViewSource.GetDefaultView(LinesUnfiltered);
            LinesFiltered.Filter = LineFilter;

            saveService = new SaveService();
            Rules.CollectionChanged += OnRulesChangedEvent;
            foreach (RuleEntity r in saveService.AutoLoad())
            {
                Rules.Add(r);
            }
            RulesSavedFiles = new(saveService.GetRulesFiles());
        }

        private bool LineFilter(object obj)
        {
            if (obj is LineEntity line)
            {
                return line.NewValue != line.OldValue && !Path.Exists(Path.Combine(RootDirectory, line.NewValue + line.Extension));
            }
            return false;
        }

        private bool FilterChecked(object obj)
        {
            if (obj is LineEntity line)
            {
                return line.IsChecked;
            }
            return false;
        }

        partial void OnRootDirectoryChanged(string? value)
        {
            LoadDirectory();
        }

        private void LoadDirectory()
        {
            if (!Directory.Exists(RootDirectory)) return;
            LinesUnfiltered.Clear();
            foreach (string element in Directory.GetFiles(RootDirectory))
            {
                LinesUnfiltered.Add(new LineEntity
                {
                    OldValue = Path.GetFileNameWithoutExtension(element),
                    NewValue = Path.GetFileNameWithoutExtension(element),
                    Extension = Path.GetExtension(element)
                });
            }

            foreach (string element in Directory.GetDirectories(RootDirectory))
            {
                LinesUnfiltered.Add(new LineEntity
                {
                    IsDirectory = true,
                    OldValue = Path.GetFileNameWithoutExtension(element),
                    NewValue = Path.GetFileNameWithoutExtension(element)
                });
            }
            ApplyRulesPreview();
        }

        private void ApplyRulesPreview()
        {
            if (LinesUnfiltered == null || Rules == null) return;
            foreach (LineEntity line in LinesUnfiltered)
            {
                if (line.OldValue == null) continue;
                string preview = line.OldValue;
                foreach (RuleEntity rule in Rules.Where(r => r.Pattern != ""))
                {
                    switch (rule.Type)
                    {
                        case RuleType.Replace:
                            preview = preview.Replace(rule.Pattern, rule.Replacement);
                            break;

                        case RuleType.Regex:
                            try
                            {
                                preview = Regex.Replace(preview, rule.Pattern, rule.Replacement);
                            }
                            catch { }
                            break;

                        case RuleType.Trim:
                            preview = preview.Trim(rule.Pattern.ToCharArray());
                            break;

                        case RuleType.Regex_Rearrange:
                            preview = Regex.Replace(line.OldValue, rule.Pattern, rule.Replacement);
                            break;
                    }
                }
                line.NewValue = preview.Trim();
            }
            LinesFiltered.Refresh();
        }

        private void OnRulesChangedEvent(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (RuleEntity newItem in e.NewItems)
                {
                    if (newItem is INotifyPropertyChanged npc)
                    {
                        npc.PropertyChanged += OnRuleChanged;
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (RuleEntity oldItem in e.OldItems)
                {
                    if (oldItem is INotifyPropertyChanged npc)
                    {
                        npc.PropertyChanged -= OnRuleChanged;
                    }
                }
            }

            CountOfItems = Rules.Count;
            ApplyRulesPreview();
        }

        private void OnRuleChanged(object? sender, PropertyChangedEventArgs e)
        {
            ApplyRulesPreview();
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
            if (LinesFiltered.IsEmpty || RootDirectory == null) return;
            LinesFiltered.Filter = FilterChecked;
            foreach (LineEntity element in LinesFiltered)
            {
                if (element.OldValue == null || element.NewValue == null || element.OldValue == element.NewValue) continue;
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
            LinesFiltered.Filter = LineFilter;
            LoadDirectory();
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
        private void SelectAll()
        {
            foreach (var item in LinesFiltered)
            {
                if (item is LineEntity line)
                    line.IsChecked = true;
            }
        }

        [RelayCommand]
        private void UnselectAll()
        {
            foreach (var item in LinesFiltered)
            {
                if (item is LineEntity line)
                    line.IsChecked = false;
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
        private void RefreshPreview()
        {
            ApplyRulesPreview();
        }

        [RelayCommand]
        private void AddRule()
        {
            Rules.Add(new());
        }
    }
}