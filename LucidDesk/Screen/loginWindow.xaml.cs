using LucidDesk.Manager;
using LucidDesk.Manager.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SecurityManager = LucidDesk.Manager.Security.SecurityManager;

namespace LucidDesk
{
    /// <summary>
    /// Interaction logic for loginWindow.xaml
    /// </summary>
    public partial class loginWindow : Window
    {
        public event EventHandler<Desk> OnClickNext;
        public loginWindow()
        {
            InitializeComponent();
            applicationNameLBL.Text = SystemInformationManager.ApplicationName;
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
       
            if (passwordTextBox.mainTextBox.Text == "")
            {
                PasswordError.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordError.Visibility = Visibility.Hidden;
            }
            if (ProfileNameTextBox.mainTextBox.Text == "")
            {
                NameError.Visibility = Visibility.Visible;
            }
            else
            {
                NameError.Visibility = Visibility.Hidden;
            }
            if (PasswordError.Visibility == Visibility.Hidden && NameError.Visibility == Visibility.Hidden)
            {
                Desk desk = new Desk() { GUID= SystemInformationManager.GetFromRegistry(), IPAddress = SystemInformationManager.GetIpAddresss(SystemInformationManager.GetMacAddress()), IsFavorite = false, HostName = SystemInformationManager.GetHostName(), ProfileName = ProfileNameTextBox.mainTextBox.Text, ProfileImage = null, DesktopImage = SystemInformationManager.GetDesktopWallpaper(), Password = SecurityManager.Encrypt( passwordTextBox.Password), MacAddress = SystemInformationManager.GetMacAddress(), OsName = SystemInformationManager.GetOsName(), PcName = SystemInformationManager.GetPcUserName(), RecentLoginTime = DateTime.MinValue };
                OnClickNext?.Invoke(this, desk);
            }

        }
    }
}
