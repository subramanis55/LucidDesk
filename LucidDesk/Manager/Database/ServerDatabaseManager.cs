
using LucidDesk.Manager.Files;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Security.Cryptography;
using LucidDesk.Settings;

namespace LucidDesk.Manager.Database
{
    public static class ServerDatabaseManager
    {

        public static string DefalutDatabaseName = "mysql";

        public static MySqlCommand mySqlCommand = new MySqlCommand();
        public static MySqlConnection mySqlConnection = new MySqlConnection();
        public static void Setup()
        {
            if (!DatabaseConnection())
            {
                string connectionString = $"Server={SettingsManager.Settings.ServerIpAddress};Port={SettingsManager.Settings.ServerPort};Uid={SettingsManager.Settings.ServerUserName};Pwd={SettingsManager.Settings.ServerPassword};Database={DefalutDatabaseName};";
                mySqlConnection.ConnectionString = connectionString;
                mySqlConnection.Open();
                mySqlCommand.Connection = mySqlConnection;
                mySqlCommand.CommandText = $"Create Database  {SettingsManager.Settings.ServerDatabase}";
                mySqlCommand.ExecuteNonQuery();
                mySqlCommand.CommandText = "CREATE TABLE DeskProfile(Id INT PRIMARY KEY AUTO_INCREMENT,IPAddress VARCHAR(100),IsFavorite BIT,HostName VARCHAR(100), ProfileName VARCHAR(100), ProfileImage TEXT,DesktopImage TEXT,Password VARCHAR(100),MacAddress VARCHAR(100) UNIQUE,OsName VARCHAR(100),PcName VARCHAR(100),RecentLoginTime DATETIME)AUTO_INCREMENT=1000000; ";
                mySqlCommand.ExecuteNonQuery();
                DatabaseConnection();
            }
        }
        private static bool DatabaseConnection()
        {
            try
            {
                string connectionString = $"Server={SettingsManager.Settings.ServerIpAddress};Port={SettingsManager.Settings.ServerPort};Uid={SettingsManager.Settings.ServerUserName};Pwd={SettingsManager.Settings.ServerPassword};Database={SettingsManager.Settings.ServerDatabase};"; 
                mySqlConnection.ConnectionString = connectionString;
                mySqlConnection.Open();
                mySqlCommand.Connection = mySqlConnection;
                return true;
            }
            catch (Exception e)
            {
            }
            return false;
        }
        public static bool IsDeskExits(string macAddress)
        {
            string IsExitsCheckQuery = $"Select * from DeskProfile where MacAddress='{macAddress}'";
            DataTable dataTable = new DataTable();
            mySqlCommand.CommandText = IsExitsCheckQuery;
            dataTable.Load(mySqlCommand.ExecuteReader());
            if (dataTable.Rows.Count == 1)
                return true;
            else
                return false;
        }
        public static bool DeskAlreadyExits(string macAddress)
        {

            Desk deskProfile = GetDeskProfile(macAddress);
            if (deskProfile == null)
                return false;
            return true;
        }
        public static bool CreateDeskProfile(Desk deskProfile)
        {
            try
            {
                if (!DeskAlreadyExits(deskProfile.MacAddress))
                {
                    string CreateDeskProfileQuery = $"Insert into  DeskProfile(IPAddress,HostName,ProfileName,ProfileImage,DesktopImage,Password,MacAddress,OsName,PcName,RecentLoginTime)   values('{deskProfile.IPAddress}','{deskProfile.HostName}','{deskProfile.ProfileName}','{FileManager.ImageToString(deskProfile.ProfileImage)}','{FileManager.ImageToString(deskProfile.DesktopImage)}','{deskProfile.Password}','{deskProfile.MacAddress}','{deskProfile.OsName}','{deskProfile.PcName}','{deskProfile.RecentLoginTime.ToString("yyyy-MM-dd HH:mm:ss")}')";
                    mySqlCommand.CommandText = CreateDeskProfileQuery;
                    mySqlCommand.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception e)
            {

            }
            return false;
        }

        public static Desk GetDeskProfile(string macAddress)
        {
            DataTable dataTable = new DataTable();
            string GetDataQuery = $"Select * from DeskProfile where MacAddress='{macAddress}'";
            mySqlCommand.CommandText = GetDataQuery;
            dataTable.Load(mySqlCommand.ExecuteReader());
            if (dataTable.Rows == null || dataTable.Rows.Count == 0)
                return null;
            Desk desk = new Desk() { IPAddress = Convert.ToString(dataTable.Rows[0]["IPAddress"]), IsFavorite = Convert.ToBoolean(dataTable.Rows[0]["IsFavorite"]), HostName = Convert.ToString(dataTable.Rows[0]["HostName"]), ProfileName = Convert.ToString(dataTable.Rows[0]["ProfileName"]), ProfileImage = FileManager.ConvertBase64ToBitmapImage(Convert.ToString(dataTable.Rows[0]["ProfileImage"])), DesktopImage = FileManager.ConvertBase64ToBitmapImage(Convert.ToString(dataTable.Rows[0]["DesktopImage"])), Password = Convert.ToString(dataTable.Rows[0]["Password"]), MacAddress = Convert.ToString(dataTable.Rows[0]["MacAddress"]), OsName = Convert.ToString(dataTable.Rows[0]["OsName"]), PcName = Convert.ToString(dataTable.Rows[0]["PcName"]), RecentLoginTime = (DateTime)dataTable.Rows[0]["PcName"] };
            return desk;
        }
        public static Dictionary<string, Desk> GetDeskProfiles()
        {
            Dictionary<string, Desk> Profiles = new Dictionary<string, Desk>();
            //DataTable dataTable = new DataTable();
            //string GetDataQuery = "Select * from DeskProfile";
            //mySqlCommand.CommandText = GetDataQuery;
            //dataTable.Load(mySqlCommand.ExecuteReader());
            //for (int i = 0; i < dataTable.Rows.Count; i++)
            //{
            //    try
            //    {
            //        Profiles.Add("" + dataTable.Rows[i]["Id"], new Desk() { Id = Convert.ToInt32("" + dataTable.Rows[i]["Id"]), IPAddress = Convert.ToString(dataTable.Rows[i]["IPAddress"]), HostName = Convert.ToString(dataTable.Rows[i]["HostName"]), ProfileName = Convert.ToString(dataTable.Rows[i]["ProfileName"]), ProfileImageString = Convert.ToString(dataTable.Rows[i]["ProfileImage"]), DesktopImageString = Convert.ToString(dataTable.Rows[i]["DesktopImage"]), Password = Convert.ToString(dataTable.Rows[i]["Password"]), MacAddress = Convert.ToString(dataTable.Rows[i]["MacAddress"]), OsName = Convert.ToString(dataTable.Rows[i]["OsName"]), PcName = Convert.ToString(dataTable.Rows[i]["PcName"]), RecentLoginTime = (DateTime)dataTable.Rows[i]["RecentLoginTime"] });
            //    }
            //    catch (Exception e)
            //    {

            //    }
            //}
            return Profiles;
        }

    }
}
