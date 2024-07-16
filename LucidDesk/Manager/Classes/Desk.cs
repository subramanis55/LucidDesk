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
        private string hostName;
        private string profileName;
        private BitmapImage profileImage;
        private BitmapImage desktopImage;
        private string password;
        private string macAddress;
        private string osName;
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
        public string DeskUserName
        {
            get
            {
                return profileName;
            }
            set
            {
                profileName = value;
                OnPropertyChanged(nameof(DeskUserName));
            }
        }
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
        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
