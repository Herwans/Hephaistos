using CommunityToolkit.Mvvm.ComponentModel;

namespace Hephaistos.App.Entities
{
    public class RuleEntity : ObservableObject
    {
        public bool IsRegex { get; set; } = false;
        public string Pattern { get; set; } = "";
        public string Replacement { get; set; } = "";
    }
}