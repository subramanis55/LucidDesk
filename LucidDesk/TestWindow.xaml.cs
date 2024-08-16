using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Forms.Integration;
using LucidDesk.UserControls.Common;

namespace LucidDesk
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {

        UserControls.Common.NotificationManager notificationManager = new UserControls.Common.NotificationManager();
        public TestWindow()
        {
            InitializeComponent();
           
        }

     

        private void CreateNotificationClick(object sender, RoutedEventArgs e)
        {
            notificationManager.CreateNotification(MessageTextBox.Text, UserControls.Common.NotificationType.Information);
        }

        private void CreateDeskNotificationClick(object sender, RoutedEventArgs e)
        {
            notificationManager.CreateInviteRequestNotification(new Manager.Classes.DeskConnectionInformation() { SenderDesk = new Manager.Desk() { Id = 10000001, ProfileName = "Subramani" } });
        }

        private void Create3NotificationClick(object sender, RoutedEventArgs e)
        {
            notificationManager.CreateNotification(MessageTextBox.Text, UserControls.Common.NotificationType.Error);
        }
    }
}
