using System.IO;
using System.Text.Json;

namespace csharp_DBCourseWork_sem5.Services.Themes
{
    public class AppSettings
    {
        public bool IsDarkTheme { get; set; } = true;

        private static readonly string FilePath = "settings.json";

        public static AppSettings Load()
        {
            if (!File.Exists(FilePath)) return new AppSettings();
            try
            {
                string json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch { return new AppSettings(); }
        }

        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(this);
                File.WriteAllText(FilePath, json);
            }
            catch {}
        }
    }
}