
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

namespace LucidDesk.Manager.Database
{
   public static  class ServerDatabaseManager
    {
        public static string ServerHostname;
        public static string ServerIpaddress;
        public static string DatabaseName="DeskApplication";
        public static string ServerDatabasePassword;
        public static MySqlCommand mySqlCommand=new MySqlCommand();
        public static MySqlConnection mySqlConnection;
        public static void Setup(){

        }
        private static bool DatabaseConnection(){
        try{
                string connectionstring = $"server={ServerIpaddress};port=3306;uid=root;pwd={ ServerDatabasePassword};databasename={DatabaseName}";
                mySqlConnection = new MySqlConnection(connectionstring);
                mySqlConnection.Open();
                mySqlCommand.Connection = mySqlConnection;
                return true;
            }
            catch(Exception e){
              
            }
            return false;   
        } 
        private static bool CreateDeskProfile(Desk deskProfile){
            try
            {
                string CreateDeskProfileQuery = $"Insert   into(IPAddress,IsFavorite,HostName,ProfileName,ProfileImage,DesktopImage,Password,MacAddress,OsName,PcName,RecentLoginTime) DeskProfile values('{deskProfile.IPAddress}',{deskProfile.IsFavorite},'{deskProfile.HostName}','{deskProfile.ProfileName}','{FileManager.ImageToString(deskProfile.ProfileImage)}','{FileManager.ImageToString(deskProfile.DesktopImage)}','{deskProfile.Password}','{deskProfile.MacAddress}','{deskProfile.OsName}','{deskProfile.PcName}','{deskProfile.RecentLoginTime.ToString("yyyy-MM-dd HH:mm:ss")}')";
                return true;
            }
            catch(Exception e)
            {

            }
            return false;
        }
        private static Desk GetDeskProfile()
        {
            DataTable dataTable = new DataTable();
            string GetDataQuery = "Select * from DeskProfile where ";
            mySqlCommand.CommandText = GetDataQuery;
            dataTable.Load(mySqlCommand.ExecuteReader());
           
               Desk desk= new Desk() { IPAddress = Convert.ToString(dataTable.Rows[0]["IPAddress"]), IsFavorite = Convert.ToBoolean(dataTable.Rows[0]["IsFavorite"]), HostName = Convert.ToString(dataTable.Rows[0]["HostName"]), ProfileName = Convert.ToString(dataTable.Rows[0]["ProfileName"]), ProfileImage = FileManager.ConvertBase64ToBitmapImage(Convert.ToString(dataTable.Rows[0]["ProfileImage"])), DesktopImage = FileManager.ConvertBase64ToBitmapImage(Convert.ToString(dataTable.Rows[0]["DesktopImage"])), Password = Convert.ToString(dataTable.Rows[0]["Password"]), MacAddress = Convert.ToString(dataTable.Rows[0]["MacAddress"]), OsName = Convert.ToString(dataTable.Rows[0]["OsName"]), PcName = Convert.ToString(dataTable.Rows[0]["PcName"]), RecentLoginTime = (DateTime)dataTable.Rows[0]["PcName"] };
            
            return desk;
        }
        private static Dictionary<string,Desk> GetDeskProfiles()
        {
           Dictionary<string,Desk> Profiles = new Dictionary<string,Desk>();
            DataTable dataTable = new DataTable();
            string GetDataQuery = "Select * from DeskProfile";
            mySqlCommand.CommandText = GetDataQuery;
            dataTable.Load(mySqlCommand.ExecuteReader());
            for (int i = 0; i<dataTable.Rows.Count; i++)
            {
                Profiles.Add(""+dataTable.Rows[i]["Id"],new Desk() {IPAddress=Convert.ToString(dataTable.Rows[i]["IPAddress"]), IsFavorite = Convert.ToBoolean(dataTable.Rows[i]["IsFavorite"]), HostName = Convert.ToString(dataTable.Rows[i]["HostName"]), ProfileName = Convert.ToString(dataTable.Rows[i]["ProfileName"]),ProfileImage = FileManager.ConvertBase64ToBitmapImage(Convert.ToString(dataTable.Rows[i]["ProfileImage"])), DesktopImage =FileManager.ConvertBase64ToBitmapImage(Convert.ToString(dataTable.Rows[i]["DesktopImage"])), Password=Convert.ToString(dataTable.Rows[i]["Password"]), MacAddress = Convert.ToString(dataTable.Rows[i]["MacAddress"]), OsName= Convert.ToString(dataTable.Rows[i]["OsName"]), PcName = Convert.ToString(dataTable.Rows[i]["PcName"]), RecentLoginTime=(DateTime) dataTable.Rows[i]["PcName"] });
            }
            return Profiles;
        }

    }
}
