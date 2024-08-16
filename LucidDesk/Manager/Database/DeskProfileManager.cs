using System;
using System.Collections.Generic;
using System.Linq;
using LucidDesk.Manager.Security;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using System.IO;
using LucidDesk.UserControls;

namespace LucidDesk.Manager.Database
{
    public static class DeskProfileManager
    {
        public static Desk UserDesk;
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
                    if(desk.MacAddress==SystemInformationManager.GetMacAddress()){
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
                string fileName = "/"+deskProfile.Id + ".txt";
                string filePath = folderPath+ fileName;
                // Ensure the directory exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                // Write to the file
                File.WriteAllText(filePath, encodedData);
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
                string deskProfileData = JsonConvert.SerializeObject(deskProfile);
                string encodedData = SecurityManager.Encrypt(deskProfileData);
                string deskProfilePath = LocalDatabaseManager.DatabaseFolderPath +"/"+ deskProfile.Id + ".txt";
                File.WriteAllText(deskProfilePath, encodedData);
                return true;
            }
            catch(Exception e)
            {
               
            }
            return false;
        }
        public static bool UpdateDeskProfilesdata(List<Desk> deskProfiles)
        {
            try
            {
            for(int i=0;i< deskProfiles.Count;i++){
                    string deskProfileData = JsonConvert.SerializeObject(deskProfiles[i]);
                    string encodedData = SecurityManager.Encrypt(deskProfileData);
                    string deskProfilePath = LocalDatabaseManager.DatabaseFolderPath + deskProfiles[i].Id + ".txt";
                    //if (Directory.Exists(deskProfilePath))
                    //    Directory.CreateDirectory(deskProfilePath);
                    File.WriteAllText(deskProfilePath, encodedData);
                }
               
                return true;
            }
            catch (Exception e)
            {

            }
            return false;
        }
        public static bool  DeleteProfile(int id){
            try{
                File.Delete(LocalDatabaseManager.DatabaseFolderPath +"/"+ id+ ".txt");
                DeskProfilesDictionary.Remove(id + "");
                DeskProfiles = DeskProfilesDictionary.Values.ToList();
                return true;
            }
            catch{

            }
          return false;
        }
    }
}
