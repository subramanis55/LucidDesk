using LucidDesk.DS.Classes;
using LucidDesk.Manager;
using LucidDesk.Manager.Files;
using LucidDesk.Manager.Services;
using LucidDesk.Models;
using LucidDesk.UserControls;
using System;
using System.Windows;

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

        private async void NextButtonClick(object sender, RoutedEventArgs e)
        {
            if (passwordTextBox.Password == "")
            {
                PasswordError.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordError.Visibility = Visibility.Hidden;
            }
            if (ProfileNameTextBox.PART_VisibleTextBox.Text == "")
            {
                NameError.Visibility = Visibility.Visible;
            }
            else
            {
                NameError.Visibility = Visibility.Hidden;
            }
            if (PasswordError.Visibility == Visibility.Hidden && NameError.Visibility == Visibility.Hidden)
            {
                UserCreateDTO userCreateDTO = new UserCreateDTO() { MacAddress = SystemInformationManager.GetMacAddress(), FirstName = ProfileNameTextBox.PART_VisibleTextBox.Text, Password = passwordTextBox.Password, PcName = SystemInformationManager.GetPcUserName(), DesktopImageString = FileManager.ImageToString(SystemInformationManager.GetDesktopWallpaper()) };
                var user = await UserService.CreateUserAsync(userCreateDTO);
                if (user == null)
                {
                    DeskMessageBox.ShowMessageBox("User Login Failed", "Error", MessageBoxType.Ok, this);
                    return;
                }
                Desk desk = new Desk(user);
                //Desk desk = new Desk() { GUID= SystemInformationManager.GetFromRegistry(), IPAddress = SystemInformationManager.GetIpAddresss(SystemInformationManager.GetMacAddress()), IsFavorite = false, HostName = SystemInformationManager.GetHostName(), ProfileName = ProfileNameTextBox.mainTextBox.Text, ProfileImage = null, DesktopImage = SystemInformationManager.GetDesktopWallpaper(), Password = SecurityManager.Encrypt( passwordTextBox.Password), MacAddress = SystemInformationManager.GetMacAddress(), OsName = SystemInformationManager.GetOsName(), PcName = SystemInformationManager.GetPcUserName(), RecentLoginTime = DateTime.MinValue };
                OnClickNext?.Invoke(this, desk);
            }

        }
    }
}
