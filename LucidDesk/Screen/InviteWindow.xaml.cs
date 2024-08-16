using LucidDesk.Manager;
using LucidDesk.Manager.Classes;
using LucidDesk.Manager.Database;
using LucidDesk.Manager.Enum;
using LucidDesk.UserControls.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private ContextMenu SuggestionsDeskMenu = new ContextMenu() { Focusable = false };
        private Style SuggestionsDeskMenuStyle = Application.Current.Resources["SuggestionDeskMenuItem"] as Style;
        public event EventHandler<DeskConnectionInformation> OnClickInviteButton;
        private Desk desk;
        public DeskConnectionInformation DeskConnectionInformation;
        public Desk Desk
        {
            set
            {
                desk = value;
                UserId = "" + desk.Id;
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
            Panel.SetZIndex(UserIdLabelContainer, 1);
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
            if (desk == null)
            {
                if (DeskProfileManager.DeskProfilesDictionary.ContainsKey("" + Textbox.Text))
                    desk = DeskProfileManager.DeskProfilesDictionary[Textbox.Text];
            }
            if (desk != null)
                OnClickInviteButton?.Invoke(this, new DeskConnectionInformation() { AccessType = (AccessType)Enum.Parse(typeof(AccessType), AccessTypeCombobox.SelectedItem.ToString()), ConnectionType = ConnectionType.Invite, AudioAccess = (bool)AudioAccessCheckBox.IsChecked, ClipboardAccess = (bool)ClipboardAccessCheckBox.IsChecked, KeyboardAccess = (bool)KeyboardAccessCheckBox.IsChecked, MouseAccess = (bool)MouseAccessCheckBox.IsChecked, SenderDesk = DeskProfileManager.UserDesk, ReceiverDesk = this.Desk, });
            else
                MainWindow.NotificationManager.CreateNotification("Id Doesn't exits", NotificationType.Information);
        }

        private void AccessTypeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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


        private void SuggestionDeskShowInvoke()
        {
            SuggestionsDeskMenu.Items.Clear();
            int count = 0;
            //DeskProfile Control Create
            for (int i = 0; i < DeskProfileManager.DeskProfiles.Count; i++)
            {
                if (("" + DeskProfileManager.DeskProfiles[i].Id).Contains(Textbox.Text) || DeskProfileManager.DeskProfiles[i].ProfileName.Contains(Textbox.Text))
                {
                    MenuItem menuItem = new MenuItem { Foreground = Brushes.Black, Focusable = false, MinWidth = Textbox.ActualWidth-3, Background = Brushes.White };
                    menuItem.Style = SuggestionsDeskMenuStyle;
                    menuItem.DataContext = DeskProfileManager.DeskProfiles[i];
                    menuItem.Click += SuggestionDeskClick;
                    SuggestionsDeskMenu.Items.Add(menuItem);
                    count++;
                }
                if (count == 10)
                    break;
            }

            SuggestionDeskShow();
            Textbox.Focus();
        }
        private void SuggestionDeskClick(object sender, RoutedEventArgs e)
        {  
            Desk desk = (Desk)((MenuItem)sender).DataContext;
            Textbox.Text = "" + desk.Id;
            SuggestionsDeskMenu.IsOpen = false;
        }
        private void SuggestionDeskShow()
        {
            SuggestionsDeskMenu.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Size SuggestionsDeskMenuSize = SuggestionsDeskMenu.DesiredSize;
            SuggestionsDeskMenu.Style = Application.Current.Resources["CustomContextMenu"] as Style;
            SuggestionsDeskMenu.PlacementTarget = Textbox;
            SuggestionsDeskMenu.Placement = PlacementMode.Relative;

            double y = Textbox.ActualHeight;
            SuggestionsDeskMenu.HorizontalOffset = -2;
            SuggestionsDeskMenu.VerticalOffset = y;
            SuggestionsDeskMenu.IsOpen = true;
        }
        private void TopPanelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
            e.Handled = true;
        }

        private void TextboxGotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TextboxLostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TextboxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (Textbox.Text != "")
                SuggestionDeskShowInvoke();
        }
    }
}
