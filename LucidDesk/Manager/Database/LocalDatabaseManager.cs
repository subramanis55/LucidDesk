using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LucidDesk.Manager.Database
{

    public static class LocalDatabaseManager
    {
        public static string ApplicationFolderPath = @"c:/Desk";
        public static string DatabaseFolderPath = @"c:/Desk/DeskDatabase";
        public static string SettingFolderPath = @"c:/Desk/Setting";
        public static bool SetUp()
        {
            if (Directory.Exists(ApplicationFolderPath))
            {
                Directory.CreateDirectory(ApplicationFolderPath);
            }
            if (Directory.Exists(DatabaseFolderPath))
            {
                Directory.CreateDirectory(DatabaseFolderPath);
            }
            if (Directory.Exists(SettingFolderPath))
            {
                Directory.CreateDirectory(SettingFolderPath);
            }
            return true;
        }
       
    }
}
