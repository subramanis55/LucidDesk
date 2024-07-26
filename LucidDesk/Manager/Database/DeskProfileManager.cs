using System;
using System.Collections.Generic;
using System.Linq;
using LucidDesk.Manager.Security;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using System.IO;

namespace LucidDesk.Manager.Database
{
    public static class DeskProfileManager
    {
        public static List<Desk> DeskProfiles;
        public static Dictionary<string, Desk> DeskProfilesDictionary;

        public static Dictionary<string, Desk> GetDeskProfilesData()
        {
            Dictionary<string, Desk> deskProfilesDictionary = new Dictionary<string, Desk>();
            string[] DeskProfilesDataPath = Directory.GetFiles(LocalDatabaseManager.DatabaseFolderPath);
            for (int i = 0; i < DeskProfilesDataPath.Length; i++)
            {
                try
                {
                    string deskProfiledata = File.ReadAllText(DeskProfilesDataPath[i]);
                    Desk desk = JsonConvert.DeserializeObject<Desk>(SecurityManager.Decrypt(deskProfiledata));
                    deskProfilesDictionary.Add("" + desk.Id, desk);
                }
                catch (Exception e)
                {

                }
            }
            return deskProfilesDictionary;
        }
        public static bool CreateDeskProfiledata(Desk deskProfile)
        {
            try
            {
                string deskProfileData = JsonConvert.SerializeObject(deskProfile);
                string encodedData = SecurityManager.Encrypt(deskProfileData);
                string deskProfilePath = LocalDatabaseManager.DatabaseFolderPath + deskProfile.Id + ".txt";
                if (Directory.Exists(deskProfilePath))
                    Directory.CreateDirectory(deskProfilePath);
                File.WriteAllText(deskProfilePath, encodedData);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
