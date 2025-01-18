using Hephaistos.App.Entities;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace Hephaistos.App.Services
{
    public class SaveService
    {
        public const string SAVE_DIRECTORY = ".hephaistos";
        public static readonly string FULL_SAVE_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), SAVE_DIRECTORY);
        public static readonly string CURRENT_SAVE = Path.Combine(FULL_SAVE_PATH, "current.json");

        public SaveService()
        {
            if (!Directory.Exists(FULL_SAVE_PATH))
            {
                Directory.CreateDirectory(FULL_SAVE_PATH);
            }
        }

        public ObservableCollection<RuleEntity>? GetCurrent()
        {
            if (!File.Exists(CURRENT_SAVE)) return null;
            string content = File.ReadAllText(CURRENT_SAVE);
            if (content == "") return null;
            ObservableCollection<RuleEntity>? rules = JsonConvert.DeserializeObject<ObservableCollection<RuleEntity>>(content);
            return rules;
        }

        public void SaveCurrent(ObservableCollection<RuleEntity> rules)
        {
            File.WriteAllText(CURRENT_SAVE, JsonConvert.SerializeObject(rules));
        }
    }
}