using CommunityToolkit.Mvvm.ComponentModel;

namespace Hephaistos.App.Entities
{
    public partial class RuleEntity : ObservableObject
    {
        [ObservableProperty]
        private bool isRegex = false;

        [ObservableProperty]
        private string pattern = "";

        [ObservableProperty]
        private string replacement = "";
    }
}