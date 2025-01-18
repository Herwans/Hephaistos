using CommunityToolkit.Mvvm.ComponentModel;

namespace Hephaistos.App.Entities
{
    public class RuleEntity : ObservableObject
    {
        private int position;

        public int Position
        {
            get => position;
            set => SetProperty(ref position, value);
        }

        public bool IsRegex { get; set; } = false;
        public string Pattern { get; set; } = "";
        public string Replacement { get; set; } = "";
    }
}