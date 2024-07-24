using LucidDesk.Manager;
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

namespace LucidDesk
{
    /// <summary>
    /// Interaction logic for InviteAcceptWindow.xaml
    /// </summary>
    public partial class InviteAcceptWindow : Window
    {
        private Desk desk;

        public Desk Desk
        {
            set
            {
                desk = value;
               
                DeskUserNameTextBlock.Text = desk.DeskUserName;
               
                DeskIdTextBlock.Text = "" + desk.Id;
                DeskUserProfileImageComponent.Image = desk.DesktopImage;
             
            }
            get
            {
                return desk;
            }
        }
        public InviteAcceptWindow()
        {
            InitializeComponent();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void MaximizeButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void MinimizeButtonClick(object sender, RoutedEventArgs e)
        {

        }

       

        private void AcceptClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
