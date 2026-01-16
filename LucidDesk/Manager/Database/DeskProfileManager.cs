using System;
using System.Collections.Generic;
using System.Linq;
using DeskUI.Manager.Security;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using System.IO;
using DeskUI.UserControls;

namespace DeskUI.Manager.Database
{
    public static class DeskProfileManager
    {
        public static event EventHandler DeskProfilesUpdated;
        private static Dictionary<string, Desk> deskProfilesDictionary = new Dictionary<string, Desk>();
        private static Desk desk;
        public static Desk UserDesk
        {
            set
            {
                desk = value;
                desk.IPAddress = SystemInformationManager.IpAddresss;
                desk.DesktopImage= SystemInformationManager.DesktopWallpaper;
            }

            get
            {
                return desk;
            }
        }

        public static List<Desk> DeskProfiles { get => DeskProfilesDictionary.Values.ToList(); }
        public static Dictionary<string, Desk> DeskProfilesDictionary
        {
            get => deskProfilesDictionary;
            set
            {
                deskProfilesDictionary = value;
                DeskProfilesUpdated?.Invoke(null, EventArgs.Empty);
            }
        }

        public static void Intialize()
        {
            deskProfilesDictionary = GetDeskProfilesData();
        }

        public static bool DeskExits(string macAddress)
        {
            if (DeskProfiles == null)
                return false;
            foreach (var desk in DeskProfiles)
            {
                if (desk.MacAddress == macAddress) return true;
            }
            return false;
        }

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
                    deskProfilesDictionary.Add("" + desk.DeskId, desk);
                    if (desk.MacAddress == SystemInformationManager.MacAddress)
                    {
                        UserDesk = desk;
                    }
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
                string folderPath = LocalDatabaseManager.DatabaseFolderPath;
                string fileName = "/" + deskProfile.DeskId + ".txt";
                string filePath = folderPath + fileName;
                // Ensure the directory exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                // Write to the file
                File.WriteAllText(filePath, encodedData);
                DeskProfilesDictionary = GetDeskProfilesData();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static bool UpdateDeskProfiledata(Desk deskProfile)
        {
            try
            {
                if (!DeskProfilesDictionary.ContainsKey(deskProfile.DeskId))
                    return CreateDeskProfiledata(deskProfile);
                string deskProfileData = JsonConvert.SerializeObject(deskProfile);
                string encodedData = SecurityManager.Encrypt(deskProfileData);
                string deskProfilePath = LocalDatabaseManager.DatabaseFolderPath + "/" + deskProfile.DeskId + ".txt";
                File.WriteAllText(deskProfilePath, encodedData);
                DeskProfilesDictionary[deskProfile.DeskId] = deskProfile;
                DeskProfilesUpdated?.Invoke(null, EventArgs.Empty);
                return true;
            }
            catch (Exception e)
            {

            }

            return false;
        }
        public static bool UpdateDeskProfilesdata()
        {
            try
            {
                for (int i = 0; i < DeskProfiles.Count; i++)
                {
                    string deskProfileData = JsonConvert.SerializeObject(DeskProfiles[i]);
                    string encodedData = SecurityManager.Encrypt(deskProfileData);
                    string deskProfilePath = LocalDatabaseManager.DatabaseFolderPath + DeskProfiles[i].DeskId + ".txt";
                    if (Directory.Exists(deskProfilePath))
                        Directory.CreateDirectory(deskProfilePath);
                    File.WriteAllText(deskProfilePath, encodedData);
                }
                return true;
            }
            catch (Exception e)
            {

            }

            return false;
        }
        public static bool DeleteProfile(string id)
        {
            try
            {
                File.Delete(LocalDatabaseManager.DatabaseFolderPath + "/" + id + ".txt");
                DeskProfilesDictionary.Remove(id + "");
                DeskProfilesDictionary = GetDeskProfilesData();
                return true;
            }
            catch
            {

            }
            return false;
        }
    }
}
