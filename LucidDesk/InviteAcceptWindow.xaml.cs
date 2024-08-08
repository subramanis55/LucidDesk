using LucidDesk.Manager;
using LucidDesk.Manager.Classes;
using LucidDesk.Manager.Enum;
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
    public partial class ConnectAcceptWindow : Window
    {
        private Desk desk;
        public event EventHandler<DeskConnectionInformation> OnClickGetStatus;

        private DeskConnectionInformation deskConnectionInformation;
        public Desk Desk
        {
            set
            {
                desk = value;
               
                DeskUserNameTextBlock.Text = desk.ProfileName;
                DeskIdTextBlock.Text = "(" + desk.Id+ ")";
                DeskUserProfileImageComponent.Image = desk.DesktopImage;
             
            }
            get
            {
                return desk;
            }
        }

        public DeskConnectionInformation DeskConnectionInformation
        {
            get
            {
                return deskConnectionInformation;
            }

            set
            {
                deskConnectionInformation = value;

            }
        }

        public ConnectAcceptWindow()
        {
            InitializeComponent();
        }

        public ConnectAcceptWindow(DeskConnectionInformation deskConnectionInformation)
        {
            InitializeComponent();
            AccessTypeCombobox.ItemsSource = Enum.GetNames(typeof(AccessType));
            DeskConnectionInformation = deskConnectionInformation;
            AccessTypeCombobox.SelectedItem = deskConnectionInformation.AccessType.ToString();
            Desk = deskConnectionInformation.SenderDesk;

        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            DeskConnectionInformation.InviteStatus = false;
            OnClickGetStatus?.Invoke(this, DeskConnectionInformation);
            this.Close();

        }

        private void MaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void MinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

       

        private void AcceptClick(object sender, RoutedEventArgs e)
        {
            DeskConnectionInformation.InviteStatus = true;
            OnClickGetStatus?.Invoke(this, DeskConnectionInformation);
        }

       

        private void AccessTypeComboboxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AccessTypeCombobox.SelectedItem.ToString() == AccessType.FullAccess.ToString())
            {
                KeyboardAccessCheckBox.IsChecked = true;
                MouseAccessCheckBox.IsChecked = true;
                ClipboardAccessCheckBox.IsChecked = true;
                AudioAccessCheckBox.IsChecked = true;
            }
            else if (AccessTypeCombobox.SelectedItem.ToString() == AccessType.ScreenShareing.ToString())
            {
                KeyboardAccessCheckBox.IsChecked = false;
                MouseAccessCheckBox.IsChecked = false;
                ClipboardAccessCheckBox.IsChecked = false;
                AudioAccessCheckBox.IsChecked = false;
            }
            else
            {
                KeyboardAccessCheckBox.IsChecked = true;
                MouseAccessCheckBox.IsChecked = true;
                ClipboardAccessCheckBox.IsChecked = true;
                AudioAccessCheckBox.IsChecked = false;
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void TopPanelMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ButtonState==MouseButtonState.Pressed){
                this.DragMove();
            }
            e.Handled = true;
        }

        private void RejectButtonClick(object sender, RoutedEventArgs e)
        {
            DeskConnectionInformation.InviteStatus = false;
            OnClickGetStatus?.Invoke(this, DeskConnectionInformation);
        }
    }
}
