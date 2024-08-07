using LucidDesk.Manager;
using LucidDesk.Manager.Classes;
using LucidDesk.Manager.Database;
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
    /// Interaction logic for InviteWindow.xaml
    /// </summary>
    public partial class InviteWindow : Window
    {
        public event EventHandler<DeskConnectionInformation> OnClickInviteButton;
        private Desk desk;
        public DeskConnectionInformation DeskConnectionInformation;
        public Desk Desk
        {
            set
            {
                desk = value;
                UserId = ""+desk.Id;
            }
            get
            {
                return desk;
            }
        }
        public string UserId
        {
            set
            {
                UserIdLabelTextblock.Text = value;
            }
        }
 
        public InviteWindow()
        {
            InitializeComponent();
            Loaded += InviteWindowLoaded;
         
        }
        public InviteWindow(Desk desk)
        {
            InitializeComponent();
            Desk = desk;
            Panel.SetZIndex(UserIdLabelContainer,1);
            Loaded += InviteWindowLoaded;
          
        }

        private void InviteWindowLoaded(object sender, RoutedEventArgs e)
        {
            AccessTypeCombobox.ItemsSource = Enum.GetNames(typeof(AccessType));
            AccessTypeCombobox.SelectedItem = "Default";
            KeyboardAccessCheckBox.IsChecked = true;
            MouseAccessCheckBox.IsChecked = true;
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void KeyboardAccessCheckBoxClick(object sender, RoutedEventArgs e)
        {

        }

        private void MouseAccessCheckBoxClick(object sender, RoutedEventArgs e)
        {

        }

        private void FileAccessCheckBoxClick(object sender, RoutedEventArgs e)
        {

        }

        private void ClipboardAccessCheckBoxClick(object sender, RoutedEventArgs e)
        {

        }

        private void InviteButtonClick(object sender, RoutedEventArgs e)
        {
            OnClickInviteButton?.Invoke(this, new DeskConnectionInformation() { AccessType = (AccessType)Enum.Parse(typeof(AccessType),AccessTypeCombobox.SelectedItem.ToString()), AudioAccess=(bool)AudioAccessCheckBox.IsChecked, ClipboardAccess= (bool)ClipboardAccessCheckBox.IsChecked, KeyboardAccess = (bool)KeyboardAccessCheckBox.IsChecked,MouseAccess = (bool)MouseAccessCheckBox.IsChecked ,SenderDesk= DeskProfileManager.UserDesk, ReceiverDesk = this.Desk, });
        }

        private void AccessTypeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AccessTypeComboboxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(AccessTypeCombobox.SelectedItem.ToString()==AccessType.FullAccess.ToString()){
                KeyboardAccessCheckBox.IsChecked = true;
                MouseAccessCheckBox.IsChecked = true;
                ClipboardAccessCheckBox.IsChecked = true;
                AudioAccessCheckBox.IsChecked = true;
            }
            else if(AccessTypeCombobox.SelectedItem.ToString() == AccessType.ScreenShareing.ToString()){
                KeyboardAccessCheckBox.IsChecked = false;
                MouseAccessCheckBox.IsChecked = false;
                ClipboardAccessCheckBox.IsChecked = false;
                AudioAccessCheckBox.IsChecked = false;
            }
            else{
                KeyboardAccessCheckBox.IsChecked = true;
                MouseAccessCheckBox.IsChecked = true;
                ClipboardAccessCheckBox.IsChecked = true;
                AudioAccessCheckBox.IsChecked = false;
            }
        }

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void TopPanelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        if(e.ButtonState==MouseButtonState.Pressed){
                this.DragMove();
            }
            e.Handled=true;
        }
    }
}
