using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Settings.Files
{
    public static class FileLocationManager
    {

        public static FileInfo settingsFileInfo = new FileInfo($"{Path.GetDirectoryName(AppContext.BaseDirectory)}" + "/settings/settings.xml");

    }
}
