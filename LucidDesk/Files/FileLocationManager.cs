using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DeskUI.Files
{
    public static class FileLocationManager
    {

        public static FileInfo settingsFileInfo = new FileInfo($"{Path.GetDirectoryName(Application.ResourceAssembly.Location)}" + "/settings/settings.xml");

    }
}
