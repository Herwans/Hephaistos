using CommunityToolkit.Mvvm.ComponentModel;

namespace Hephaistos.App.Entities
{
    public partial class RuleEntity : ObservableObject
    {
        [ObservableProperty]
        private RuleType type = RuleType.Replace;

        [ObservableProperty]
        private string pattern = "";

        [ObservableProperty]
        private string replacement = "";

        public bool HasReplacement => Type != RuleType.Trim;

        partial void OnTypeChanged(RuleType value)
        {
            OnPropertyChanged(nameof(HasReplacement));
        }
    }

    public enum RuleType
    {
        Replace,
        Regex,
        Regex_Rearrange,
        Trim
    }
}