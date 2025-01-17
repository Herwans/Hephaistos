namespace Hephaistos.App.Entities
{
    public class LineEntity
    {
        public bool IsChecked { get; set; } = false;
        public bool IsDirectory { get; set; } = false;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}