using LucidDesk.Manager;
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
        private Desk desk;
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
        public event EventHandler OnClickInviteButton;
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
            OnClickInviteButton?.Invoke(this, EventArgs.Empty);
        }

        private void AccessTypeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AccessTypeComboboxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
