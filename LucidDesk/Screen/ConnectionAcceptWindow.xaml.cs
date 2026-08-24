using LucidDesk.DS.Classes;
using LucidDesk.DS.Enum;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LucidDesk
{
    /// <summary>
    /// Interaction logic for InviteAcceptWindow.xaml
    /// </summary>
    public partial class ConnectAcceptWindow : Window
    {
        private Desk desk;
        public event EventHandler<DeskConnectionInformation> OnClickGetStatus;
        private bool isInternal;
        private DeskConnectionInformation deskConnectionInformation;
        public Desk Desk
        {
            set
            {
                desk = value;

                DeskUserNameTextBlock.Text = desk.ProfileName;
                DeskIdTextBlock.Text = "(" + desk.DisplayID + ")";
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
                KeyboardAccessCheckBox.IsChecked = deskConnectionInformation.KeyboardAccess;
                MouseAccessCheckBox.IsChecked = deskConnectionInformation.MouseAccess;
                ClipboardAccessCheckBox.IsChecked = deskConnectionInformation.ClipboardAccess;
                AudioAccessCheckBox.IsChecked = deskConnectionInformation.AudioAccess;

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
            isInternal = true;
            AccessTypeCombobox.SelectedItem = deskConnectionInformation.AccessType.ToString();
            isInternal = false;
            DeskConnectionInformation = deskConnectionInformation;
            Desk = deskConnectionInformation.SenderDesk;
            header.Text = deskConnectionInformation.ConnectionType == ConnectionType.Invite ? "Invite Request" : "Connection Request";
            AccessCheckBoxDisable(deskConnectionInformation.ConnectionType != ConnectionType.Invite);
        }

        private void AccessCheckBoxDisable(bool enable)
        {
            AccessTypeCombobox.IsEnabled = KeyboardAccessCheckBox.IsEnabled = MouseAccessCheckBox.IsEnabled = ClipboardAccessCheckBox.IsEnabled = AudioAccessCheckBox.IsEnabled = enable;
        }
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            DeskConnectionInformation.Status = false;
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
            DeskConnectionInformation.Status = true;
            OnClickGetStatus?.Invoke(this, DeskConnectionInformation);
        }




        private void AccessTypeComboboxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInternal)
                return;
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
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
            e.Handled = true;
        }

        private void RejectButtonClick(object sender, RoutedEventArgs e)
        {
            DeskConnectionInformation.Status = false;
            OnClickGetStatus?.Invoke(this, DeskConnectionInformation);
        }
    }
}
