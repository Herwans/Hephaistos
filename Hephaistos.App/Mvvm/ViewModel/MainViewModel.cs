using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hephaistos.App.Entities;
using System.Collections.ObjectModel;
using System.IO;

namespace Hephaistos.App.Mvvm.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? rootDirectory;

        [ObservableProperty]
        private ObservableCollection<LineEntity>? lines;

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
        private void LoadDirectoryContent()
        {
            if (!Directory.Exists(RootDirectory)) return;
            Lines = [];
            foreach (string element in Directory.GetFiles(RootDirectory))
            {
                Lines.Add(new LineEntity
                {
                    OldValue = Path.GetFileName(element),
                    NewValue = Path.GetFileName(element)
                });
            }

            foreach (string element in Directory.GetDirectories(RootDirectory))
            {
                Lines.Add(new LineEntity
                {
                    IsDirectory = true,
                    OldValue = Path.GetFileName(element),
                    NewValue = Path.GetFileName(element)
                });
            }
        }
    }
}