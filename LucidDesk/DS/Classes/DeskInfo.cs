using LucidDesk.DS.Enum;
using LucidDesk.DS.Models;
using LucidDesk.Manager.Settings;
using Newtonsoft.Json;

namespace LucidDesk.DS.Classes
{
    public class DeskInfo
    {
        public DeskInfo()
        {
        }
        public DeskInfo(Desk desk)
        {
            GUID = desk.GUID;
            Id = desk.Id;
            IPAddress = desk.IPAddress;
            HostName = desk.HostName;
            ProfileName = desk.ProfileName;
            MacAddress = desk.MacAddress;
            OsName = desk.OsName;
            PcName = desk.PcName;
            DesktopImageString = desk.DesktopImageString;
            ProfileImageString = desk.ProfileImageString;
            PeerInfo = desk.PeerInfo;
        }
        public string GUID { set; get; }

        [JsonIgnore]
        public string DisplayID
        {
            get
            {
                return SettingsManager.Settings.ApplicationMode == ApplicationMode.Online ? Id.ToString() : IPAddress;
            }
        }
        [JsonIgnore]
        public string DeskId
        {
            get
            {
                return SettingsManager.Settings.ApplicationMode == ApplicationMode.Online ? Id.ToString() : GUID;
            }
        }
        public string Id { get; set; }
        public string IPAddress { get; set; }
        public string HostName { get; set; }
        public string ProfileName { get; set; }
        public string MacAddress { get; set; }
        public string OsName { get; set; }
        public string PcName { get; set; }
        public string DesktopImageString { get; set; }
        public string ProfileImageString { get; set; }
        public PeerInfo PeerInfo { get; set; }
    }
}
