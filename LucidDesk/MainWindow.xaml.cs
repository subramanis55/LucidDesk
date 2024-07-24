using LucidDesk.Manager;
using LucidDesk.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LucidDesk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ContextMenu MenuContext;
        public static ClientNetworkManager ClientNetworkManager=new ClientNetworkManager();
        public static ServerNetworkManager ServerNetworkManager=new ServerNetworkManager();
        public static DeskProfile SelectedDeskProfile;
        private Thread listenerThread;
       
        public MainWindow()
        {
            InitializeComponent();
          
            Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {

            MainTabControl.SelectedItem = HomePage;
            MenuContext = this.Resources["MenuContext"] as ContextMenu;

            DeskSwicthControl.OnclickDiscoverdButton += DeskSwicthControlOnclickDiscoverdButton;
            DeskSwicthControl.OnclickFavoritesButton += DeskSwicthControlOnclickFavoritesButton;
            DeskSwicthControl.OnclickRecentSessionsButton += DeskSwicthControlOnclickRecentSessionsButton;

            DeskProfile deskProfile1 = new DeskProfile() { Desk = new Desk() { IPAddress = "192.168.3.201", DeskUserName = "Sharuk", PcName = "SEZ-01", Id = 712345678 }, Height = 200, Width = 280, Margin = new Thickness(15), Foreground = Brushes.White };
            DeskProfile deskProfile2 = new DeskProfile() { Desk = new Desk() { IPAddress = "192.168.3.50", DeskUserName = "Subramani.S", PcName = "SEZ-01", Id = 912345678, DesktopImage = SystemInformationManager.GetDesktopWallpaper() }, Height = 200, Width = 280, Margin = new Thickness(15), Foreground = Brushes.White };
            deskProfile1.OnclickConnect += DeskProfile1OnclickConnect;
            deskProfile2.OnclickConnect += DeskProfile1OnclickConnect;

            RecentSessionsDeskControlContainer.Children.Add(deskProfile1);
            RecentSessionsDeskControlContainer.Children.Add(deskProfile2);

            ClientNetworkManager.ConnectedToSeverInvoke += ClientNetworkManagerConnectedToSeverInvoke;
            ClientNetworkManager.ScreenShareUpdateInvoke += ClientNetworkManagerScreenShareUpdateInvoke;
            ClientNetworkManager.DisConnectedToSeverInvoke += ClientNetworkManagerDisConnectedToSeverInvoke;

           ScreenImage.MouseRightButtonUp += ScreenImage_MouseRightButtonUp;
            ScreenImage.MouseRightButtonDown += ScreenImage_MouseRightButtonDown;
            ScreenImage.MouseUp += ScreenImage_MouseUp;
            ScreenImage.MouseMove += ScreenImage_MouseMove;
            ScreenImage.MouseDown+= ScreenImage_MouseDown;

            this.KeyDown += Window_KeyDown;
            this.KeyUp += Window_KeyUp;
        //    StartServerConnection();
        }

        private void ClientNetworkManagerDisConnectedToSeverInvoke(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                MainTabControl.SelectedItem = HomePage;
                SearchBoxControl.Text = "";
                SearchBoxControl.IsReadOnly = false;
                ScreenImage.Focus();
            });
        }

        private void ClientNetworkManagerScreenShareUpdateInvoke(object sender, BitmapImage e)
        {
            Dispatcher.Invoke(()=>
            {
                ScreenImage.Source = e;
            });
        }

        private void ClientNetworkManagerConnectedToSeverInvoke(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                MainTabControl.SelectedItem = ScreenSharePage;
            SearchBoxControl.Text = SelectedDeskProfile.Desk.IPAddress;
                SearchBoxControl.IsReadOnly = true;
                ScreenImage.Focus();
            });
        }

        private void DeskProfile1OnclickConnect(object sender, EventArgs e)
        {
            SelectedDeskProfile = (DeskProfile)(sender);
            ClientNetworkManager.ClientIpaddress = SelectedDeskProfile.Desk.IPAddress;
            if (!ClientNetworkManager.isConnected)
            {
                Task.Run(() => ClientNetworkManager.ConnectToServer());
            }
        }

        private void DeskSwicthControlOnclickRecentSessionsButton(object sender, EventArgs e)
        {
            DeskMainContainer.Children.Remove(RecentSessionsDeskContainer);
            DeskMainContainer.Children.Insert(0, RecentSessionsDeskContainer);
          
        }

        private void DeskSwicthControlOnclickFavoritesButton(object sender, EventArgs e)
        {
            DeskMainContainer.Children.Remove(FavoritesDeskContainer);
            DeskMainContainer.Children.Insert(0,FavoritesDeskContainer);

        }

        private void DeskSwicthControlOnclickDiscoverdButton(object sender, EventArgs e)
        {
            DeskMainContainer.Children.Remove(DiscoveredDeskContainer);
            DeskMainContainer.Children.Insert(0, DiscoveredDeskContainer);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized){
                WindowState = WindowState.Normal;
            }
            else
                WindowState = WindowState.Maximized;
        }

        private void MinimizeButtonClick(object sender, RoutedEventArgs e)
        {
       
            WindowState = WindowState.Minimized;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void TitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
            e.Handled = true;
        }

       

        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            MenuContextShow();
            e.Handled = true;

        }
        private void MenuContextShow(){
            MenuContext.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Size contextMenuSize = MenuContext.DesiredSize;

            double offsetX = (MenuButton.ActualWidth - contextMenuSize.Width);
            double offsetY = MenuButton.ActualHeight;
            MenuContext.PlacementTarget = MenuButton;
            MenuContext.Placement = PlacementMode.Relative;
            MenuContext.HorizontalOffset = offsetX;
            MenuContext.VerticalOffset = offsetY;
            MenuContext.IsOpen = true;
        }
 
        private void MenuButtonMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MenuContextShow();
            e.Handled = true;
        }

        //Screen Share Server Details  

        private void StartServerConnection()
        {

            if (ServerNetworkManager.isStarted) return;

            Visibility = Visibility.Hidden;
            ShowInTaskbar = false;
            listenerThread = new Thread(new ThreadStart(ServerNetworkManager.StartListening))
            {
                IsBackground = true
            };
            listenerThread.Start();
            ServerNetworkManager.isStarted = true;
        }


        //Screen Share Client Details
     

        private void ScreenImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ClientNetworkManager.isConnected)
            {
                Point position = e.GetPosition(ScreenImage);
                ClientNetworkManager.SendMouseEvent(position, "MouseDown", ScreenImage.ActualWidth, ScreenImage.ActualHeight);
            }
        }

        private void ScreenImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (ClientNetworkManager.isConnected && e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(ScreenImage);
                ClientNetworkManager.SendMouseEvent(position, "MouseMove", ScreenImage.ActualWidth, ScreenImage.ActualHeight);
            }
        }

        private void ScreenImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ClientNetworkManager.isConnected)
            {
                Point position = e.GetPosition(ScreenImage);
                ClientNetworkManager.SendMouseEvent(position, "MouseUp",ScreenImage.ActualWidth, ScreenImage.ActualHeight);
            }
        }

        //clipboard
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                ClientNetworkManager.SendClipboardContentToServer();
            }
            if (e.Key == Key.C && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {

            }
            ClientNetworkManager.SendKeyEvent(e.Key, "KeyDown");
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            ClientNetworkManager.SendKeyEvent(e.Key, "KeyUp");
        }
        private void ScreenImage_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ClientNetworkManager.isConnected)
            {
                Point position = e.GetPosition(ScreenImage);
                ClientNetworkManager.SendMouseRightEvent(position, "MouseRightDown", ScreenImage.ActualWidth, ScreenImage.ActualHeight);
            }
        }

        private void ScreenImage_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ClientNetworkManager.isConnected)
            {
                Point position = e.GetPosition(ScreenImage);
                ClientNetworkManager.SendMouseRightEvent(position, "MouseRightUp", ScreenImage.ActualWidth, ScreenImage.ActualHeight);
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RecentSessionsShowMoreClick(object sender, RoutedEventArgs e)
        {

        }

        private void FavoritesShowMoreClick(object sender, RoutedEventArgs e)
        {

        }

        private void DiscoveredShowMoreClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
