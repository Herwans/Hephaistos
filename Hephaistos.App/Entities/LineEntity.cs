using CommunityToolkit.Mvvm.ComponentModel;

namespace Hephaistos.App.Entities
{
    public class LineEntity : ObservableObject
    {
        private string? newValue;
        public bool IsChecked { get; set; } = false;
        public bool IsDirectory { get; set; } = false;
        public string? OldValue { get; set; }

        public string? NewValue
        {
            get => newValue;
            set => SetProperty(ref newValue, value);
        }

        public string? Extension { get; set; }
    }
}