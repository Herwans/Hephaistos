using Hephaistos.App.Entities;
using Newtonsoft.Json;
using System.IO;

namespace Hephaistos.App.Services
{
    public class SaveService
    {
        public const string DIRECTORY_NAME = ".hephaistos";
        public const string AUTO_SAVE_FILE = "autosave";
        public static readonly string FULL_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), DIRECTORY_NAME);

        public SaveService()
        {
            if (!Directory.Exists(FULL_PATH))
            {
                Directory.CreateDirectory(FULL_PATH);
            }
        }

        public string[] GetRulesFiles()
        {
            return Directory.GetFiles(FULL_PATH)
                .Where(f => Path.GetExtension(f).ToLower() == ".json"
                && Path.GetFileNameWithoutExtension(f) != AUTO_SAVE_FILE).ToArray();
        }

        public void SaveRules(string file, IEnumerable<RuleEntity> rules)
        {
            File.WriteAllText(Path.Combine(FULL_PATH, file + ".json"), JsonConvert.SerializeObject(rules));
        }

        public IEnumerable<RuleEntity> LoadRules(string file)
        {
            string path = Path.Combine(FULL_PATH, file + ".json");
            if (!File.Exists(path)) return [];
            string content = File.ReadAllText(path);
            if (content == "") return [];
            IEnumerable<RuleEntity>? rules = JsonConvert.DeserializeObject<IEnumerable<RuleEntity>>(content);
            return rules ?? [];
        }

        public IEnumerable<RuleEntity> AutoLoad()
        {
            return LoadRules(AUTO_SAVE_FILE);
        }

        public void AutoSave(IEnumerable<RuleEntity> rules)
        {
            SaveRules(AUTO_SAVE_FILE, rules);
        }
    }
}