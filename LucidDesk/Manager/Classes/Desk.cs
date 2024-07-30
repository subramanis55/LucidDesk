using LucidDesk.Manager.Files;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LucidDesk.Manager
{
    public class Desk : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int id;
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
        private DateTime recentLoginTime;
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public string IPAddress{
        get{
                return iPAddress;
        }
        set{
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
                OnPropertyChanged(nameof(DesktopImage));
            }
        }
        public string OsName{
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

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
       
    }
}
