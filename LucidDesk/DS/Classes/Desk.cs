using LucidDesk.DS.Enum;
using LucidDesk.DS.Models;
using LucidDesk.Manager.Files;
using LucidDesk.Manager.Settings;
using LucidDesk.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace LucidDesk.DS.Classes
{
    public class Desk : INotifyPropertyChanged
    {
        public Desk() { }
        public Desk(User user)
        {
            GUID = user.UserID.ToString();
            Id = user.UserNumber;
            ProfileImageString = user.profileImageString;
            DesktopImageString = user.DesktopImageString;
            PcName = user.PCName;
            MacAddress = user.MacAddress;
            ProfileName = user.FirstName;
        }

        public event EventHandler OnClickObjectDisposed;
        public event PropertyChangedEventHandler PropertyChanged;
        private string id;

        private string iPAddress;
        private bool isFavorite;
        private string hostName;
        private string profileName;

        [JsonIgnore]
        private BitmapImage profileImage;
        [JsonIgnore]
        private BitmapImage desktopImage;

        private string profileImageString;
        private string desktopImageString;
        private string password;
        private string macAddress;
        private string osName;
        private string pcName;
        private DateTime recentLoginTime, recentConnectedTime = default;
        public string GUID { set; get; }

        public string DisplayID
        {
            get
            {
                return SettingsManager.Settings.ApplicationMode == ApplicationMode.Online ? Id.ToString() : IPAddress;
            }
        }
        public string DeskId
        {
            get
            {
                return SettingsManager.Settings.ApplicationMode == ApplicationMode.Online ? Id.ToString() : GUID;
            }
        }
        [JsonProperty]
        public string Id
        {
            get
            {
                return id;
            }
            private set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public string IPAddress
        {
            get
            {
                return iPAddress;
            }
            set
            {
                iPAddress = value;
                OnPropertyChanged(nameof(IPAddress));
            }
        }

        public string HostName
        {
            get
            {
                return hostName;
            }
            set
            {
                hostName = value;
                OnPropertyChanged(nameof(HostName));
            }
        }
        public string ProfileName
        {
            get
            {
                return profileName;
            }
            set
            {
                profileName = value;
                OnPropertyChanged(nameof(ProfileName));
            }
        }

        [JsonIgnore]
        public BitmapImage ProfileImage
        {
            get
            {
                return profileImage;
            }
            set
            {
                profileImage = value;
                profileImageString = FileManager.ImageToString(profileImage);
                OnPropertyChanged(nameof(ProfileImage));
            }
        }
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string MacAddress
        {
            get
            {
                return macAddress;
            }
            set
            {
                macAddress = value;
                OnPropertyChanged(nameof(MacAddress));
            }
        }
        [JsonIgnore]
        public BitmapImage DesktopImage
        {
            get
            {
                return desktopImage;
            }
            set
            {
                desktopImage = value;
                desktopImageString = FileManager.ImageToString(desktopImage);
                OnPropertyChanged(nameof(DesktopImage));
            }
        }
        public string OsName
        {
            get
            {
                return osName;
            }
            set
            {
                osName = value;
                OnPropertyChanged(nameof(OsName));
            }
        }

        public bool IsFavorite
        {
            get
            {
                return isFavorite;
            }

            set
            {
                isFavorite = value;
                OnPropertyChanged(nameof(IsFavorite));
            }
        }

        public DateTime RecentLoginTime
        {
            get
            {
                return recentLoginTime;
            }

            set
            {
                recentLoginTime = value;
                OnPropertyChanged(nameof(RecentLoginTime));
            }
        }

        public DateTime RecentConnectedTime
        {
            get
            {
                return recentLoginTime;
            }

            set
            {
                recentLoginTime = value;
                OnPropertyChanged(nameof(RecentConnectedTime));
            }
        }


        public string PcName
        {
            get
            {
                return pcName;
            }

            set
            {
                pcName = value;
            }
        }


        public string DesktopImageString
        {
            get
            {
                return desktopImageString;
            }

            set
            {
                desktopImageString = value;
                desktopImage = FileManager.ConvertBase64ToBitmapImage(desktopImageString);
            }
        }

        public string ProfileImageString
        {
            get
            {
                return profileImageString;
            }

            set
            {
                profileImageString = value;
                profileImage = FileManager.ConvertBase64ToBitmapImage(profileImageString);
            }
        }

        public PeerInfo PeerInfo { get; set; }

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public void Dispose()
        {
            OnClickObjectDisposed?.Invoke(this, EventArgs.Empty);
        }
        public void Freeze()
        {
            DesktopImage?.Freeze();
            profileImage?.Freeze();
        }

        public Desk Clone()
        {
            Desk desk = new Desk();
            desk.Id = id;
            desk.IPAddress = iPAddress;
            desk.HostName = hostName;
            desk.profileName = profileName;
            desk.ProfileImageString = ProfileImageString;
            desk.DesktopImageString = DesktopImageString;
            desk.OsName = OsName;
            desk.MacAddress = MacAddress;
            desk.PcName = PcName;
            desk.GUID = GUID;
            desk.recentLoginTime = recentLoginTime;
            return desk;
        }
    }
}
