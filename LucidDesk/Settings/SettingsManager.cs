using LucidDesk.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
namespace LucidDesk.Settings
{
    public static class SettingsManager
    {
        private static Settings settings;
        public static bool IsIntialized { get; private set; }

        public static Settings Settings => settings ?? (settings = readSettingsFile());

        public static bool Initialize()
        {
            if (IsIntialized) return IsIntialized;
            IsIntialized = true;
            return IsIntialized;
        }
        private static Settings readSettingsFile()
        {
            if (System.IO.File.Exists(FileLocationManager.settingsFileInfo.FullName))
            {
                return ReadXml<Settings>(FileLocationManager.settingsFileInfo.FullName);
            }
            Settings settings = new Settings();
            settings.WriteXml(FileLocationManager.settingsFileInfo.FullName);
            return settings;

        }
        public static T ReadXml<T>(string path) where T : class
        {
            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
                return null;
            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    return xmlSerializer.Deserialize(fileStream) as T;
                }

            }
            catch
            {
                return null;
            }
        }
        public static void WriteXml(this object obj, string path)
        {
            if (!Directory.Exists(FileLocationManager.settingsFileInfo.FullName))
            {
                Directory.CreateDirectory(FileLocationManager.settingsFileInfo.DirectoryPath);
            }

            using (FileStream fileStream = System.IO.File.Create(path))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(string.Empty, string.Empty);
                xmlSerializer.Serialize(fileStream, obj, xmlSerializerNamespaces);
            }
        }
    }
}
