using Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Settings.Enum;

namespace DeskUI.DeskStructure
{
    public class Desk : INotifyPropertyChanged
    {
        public event EventHandler OnClickDeleted;
        public event PropertyChangedEventHandler PropertyChanged;
        private int id;

        private string iPAddress;
        private bool isFavorite;
        private string hostName;
        private string profileName;

        private byte[] profileImage;
        private byte[] desktopImage;

        private string profileImageString;
        private string desktopImageString;
        private string password;
        private string macAddress;
        private string osName;
        private string pcName;
        private DateTime recentLoginTime;
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
        private int Id
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

        public byte[] ProfileImage
        {
            get
            {
                return profileImage;
            }
            set
            {
                profileImage = value;
                //profileImageString = FileManager.ImageToString(desktopImage);
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
        public byte[] DesktopImage
        {
            get
            {
                return desktopImage;
            }
            set
            {
                desktopImage = value;
                //desktopImageString = FileManager.ImageToString(desktopImage);
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

        //public string DesktopImageString
        //{
        //    get
        //    {
        //        return desktopImageString;
        //    }

        //    set
        //    {
        //        desktopImageString = value;
        //        desktopImage = FileManager.ConvertBase64ToBitmapImage(desktopImageString);
        //    }
        //}

        //public string ProfileImageString
        //{
        //    get
        //    {
        //        return profileImageString;
        //    }

        //    set
        //    {
        //        profileImageString = value;
        //        profileImage = FileManager.ConvertBase64ToBitmapImage(profileImageString);
        //    }
        //}

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public void Dispose()
        {
            OnClickDeleted?.Invoke(this, EventArgs.Empty);
        }
        public void Freeze(){
        //DesktopImage?.Freeze();
        //profileImage?.Freeze();
        }
    }
}
