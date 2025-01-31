using CommunityToolkit.Mvvm.ComponentModel;

namespace Hephaistos.App.Entities
{
    public partial class LineEntity : ObservableObject
    {
        private string? newValue;

        [ObservableProperty]
        private bool isChecked = false;

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