using LucidDesk.DS.Settings;
using LucidDesk.Files;
using System;
using System.IO;
using System.Xml.Serialization;
namespace LucidDesk.Manager.Settings
{
    public static class SettingsManager
    {
        private static SystemSettings settings;
        public static bool IsIntialized { get; private set; }

        public static SystemSettings Settings => settings ?? (settings = readSettingsFile());

        public static bool Initialize()
        {
            if (IsIntialized) return IsIntialized;
            IsIntialized = true;
            return IsIntialized;
        }
        private static SystemSettings readSettingsFile()
        {
            if (File.Exists(FileLocationManager.settingsFileInfo.FullName))
            {
                return ReadXml<SystemSettings>(FileLocationManager.settingsFileInfo.FullName);
            }
            SystemSettings settings = new SystemSettings();
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
            catch (Exception ex)
            {
                //ToDo Log
            }
            return null;
        }

        public static void WriteXml(this object obj, string path)
        {
            try
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
            catch (Exception ex)
            {
                //ToDo Log
            }
        }
    }
}
