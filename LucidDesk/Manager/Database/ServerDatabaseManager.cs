using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            string CreateDeskProfileQuery = $"Insert   into(IPAddress,IsFavorite,HostName,ProfileName,DesktopImage,Password,MacAddress,OsName,PcName,RecentLoginTime) DeskApplication values('{deskProfile.IPAddress}',{deskProfile.IsFavorite},{deskProfile.HostName},{deskProfile.ProfileImage},)";
               
 
        
    }
        private static Desk GetDeskProfile()
        {

        }
        private static Desk GetDeskProfiles()
        {

        }

    }
}
