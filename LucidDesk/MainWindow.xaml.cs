using LucidDesk.Manager;
using LucidDesk.Manager.Classes;
using LucidDesk.Manager.Database;
using LucidDesk.Manager.Enum;
using LucidDesk.UserControls;
using LucidDesk.UserControls.Common;
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
using System.Windows.Threading;

namespace LucidDesk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ContextMenu MenuContext;
        public static NotificationManager NotificationManager = new NotificationManager();
        public static ClientNetworkManager ClientNetworkManager = new ClientNetworkManager();
        public static ServerNetworkManager ServerNetworkManager = new ServerNetworkManager();
        public static DeskProfile SelectedDeskProfile;
        private Thread listenerThread;
        private bool isConnected;
        public bool IsConnected
        {
            set
            {
                isConnected = value;
            }
            get
            {
                return isConnected;
            }
        }
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
            MethodSubscribe();
            SetUpCheck();
            LoadAnimatedGif("YourGifFile.gif");
            //   IsConnected = true;
            StartServerConnection();
            //MainTabControl.SelectedItem = ScreenSharePage;
        }

        private void SetUpCheck()
        {
            LocalDatabaseManager.SetUp();
            ServerDatabaseManager.Setup();
            if (!ServerDatabaseManager.IsDeskExits(SystemInformationManager.GetMacAddress()))
            {
                ServerDatabaseManager.CreateDeskProfile(new Desk() { IPAddress = SystemInformationManager.GetIpAddresss(SystemInformationManager.GetMacAddress()), IsFavorite = false, HostName = SystemInformationManager.GetHostName(), ProfileName = "UnKnown", ProfileImage = null, DesktopImage = SystemInformationManager.GetDesktopWallpaper(), Password = "", MacAddress = SystemInformationManager.GetMacAddress(), OsName = SystemInformationManager.GetOsName(), PcName = SystemInformationManager.GetPcUserName(), RecentLoginTime = DateTime.MinValue });
                DeskProfileManager.DeskProfilesDictionary = ServerDatabaseManager.GetDeskProfiles();
                List<Desk> deskProfiles = DeskProfileManager.DeskProfilesDictionary.Values.ToList();
                for (int i = 0; i < deskProfiles.Count; i++)
                {
                    DeskProfileManager.CreateDeskProfiledata(deskProfiles[i]);
                }
            }
            else
            {
                DeskProfileManager.DeskProfilesDictionary = ServerDatabaseManager.GetDeskProfiles();
                List<Desk> deskProfiles = DeskProfileManager.DeskProfilesDictionary.Values.ToList();
                for (int i = 0; i < deskProfiles.Count; i++)
                {
                    DeskProfileManager.UpdateDeskProfiledata(deskProfiles[i]);
                }
            }

            DeskProfileManager.DeskProfilesDictionary = DeskProfileManager.GetDeskProfilesData();
            List<Desk> deskProfileList = DeskProfileManager.DeskProfilesDictionary.Values.ToList();

            //DeskProfile Control Create
            for (int i = 0; i < deskProfileList.Count; i++)
            {
                if (SystemInformationManager.GetMacAddress() != deskProfileList[i].MacAddress)
                {
                    if (deskProfileList[i].RecentLoginTime.Month == DateTime.Now.Month && deskProfileList[i].RecentLoginTime.Year == DateTime.Now.Year)
                    {
                        DeskProfile deskProfile = new DeskProfile()
                        {
                            Height = 180,
                            Width = 280,
                            Foreground = Brushes.White,
                            Margin = new Thickness(15)
                        };
                        deskProfile.Desk = deskProfileList[i];
                        deskProfile.OnclickConnect += DeskProfile1OnclickConnect;
                        deskProfile.OnInviteConnect += DeskProfileOnInviteConnect;
                        RecentSessionsDeskControlContainer.Children.Add(deskProfile);

                    }
                    if (deskProfileList[i].IsFavorite)
                    {
                        DeskProfile deskProfile = new DeskProfile()
                        {
                            Height = 180,
                            Width = 280,
                            Foreground = Brushes.White,
                            Margin = new Thickness(15)
                        };
                        deskProfile.Desk = deskProfileList[i];
                        deskProfile.OnclickConnect += DeskProfile1OnclickConnect;
                        deskProfile.OnInviteConnect += DeskProfileOnInviteConnect;
                        FavoritesDeskControlContainer.Children.Add(deskProfile);

                    }
                    DeskProfile deskProfileDicoverd = new DeskProfile()
                    {
                        Height = 180,
                        Width = 280,
                        Foreground = Brushes.White,
                        Margin = new Thickness(15)
                    };
                    deskProfileDicoverd.Desk = deskProfileList[i];
                    deskProfileDicoverd.OnclickConnect += DeskProfile1OnclickConnect;
                    deskProfileDicoverd.OnInviteConnect += DeskProfileOnInviteConnect;
                    DiscoveredDeskControlContainer.Children.Add(deskProfileDicoverd);

                    RecentSessionsDeskControlContainer.MaxHeight = 220;
                    FavoritesDeskControlContainer.MaxHeight = 220;
                    DiscoveredDeskControlContainer.MaxHeight = 220;
                }
                else
                {
                    TextblockId.Text = "" + deskProfileList[i].Id;
                }
            }
        }

        //Invite and connect Part
        private void DeskProfileOnInviteConnect(object sender, Desk desk)
        {

            InviteWindow inviteWindow = new InviteWindow(desk);
            inviteWindow.OnClickInviteButton += InviteWindowOnClickInviteButton;
            inviteWindow.ShowDialog();
        }
 
      

       
        private void ServerNetworkManagerInviteRequestReceivedInvoke(object sender, DeskConnectionInformation deskConnectionInformation)
        {
            if (DeskProfileManager.DeskProfilesDictionary.ContainsKey("" + deskConnectionInformation.SenderDesk.Id))
                deskConnectionInformation.SenderDesk = DeskProfileManager.DeskProfilesDictionary["" + deskConnectionInformation.SenderDesk.Id];
            else
                DeskProfileManager.DeskProfilesDictionary.Add("" + deskConnectionInformation.SenderDesk.Id, deskConnectionInformation.SenderDesk);

         
                
                try { deskConnectionInformation.ReceiverDesk.DesktopImage.Freeze(); } catch { }

                try { deskConnectionInformation.ReceiverDesk.ProfileImage.Freeze(); } catch { }
                try { deskConnectionInformation.SenderDesk.DesktopImage.Freeze(); }catch{ }
         
                try { deskConnectionInformation.SenderDesk.ProfileImage.Freeze(); } catch { }
                
            Dispatcher.Invoke(() => { NotificationManager.CreateInviteRequestNotification(deskConnectionInformation); });

                
            
        }
        private void NotificationManagerOnClickInviteStatusGet(object sender, DeskConnectionInformation deskConnectionInformation)
        {
            if (deskConnectionInformation.InviteStatus == true)
            {
                ClientNetworkManager.DeskConnectionInformation = deskConnectionInformation;
                ClientNetworkManager.ClientIpaddress = deskConnectionInformation.SenderDesk.IPAddress;
                if (!ClientNetworkManager.isConnected)
                {
                    Task.Run(() => ClientNetworkManager.ConnectToServer());
                }
            }
        }

        

        private void InviteWindowOnClickInviteButton(object sender, DeskConnectionInformation deskConnectionInformation)
        {
            ClientNetworkManager.DeskConnectionInformation = deskConnectionInformation;
            ClientNetworkManager.InviteRequestSent(deskConnectionInformation);
            ((Window)(this)).Close();
        }
        private void InviteButtonClick(object sender, RoutedEventArgs e)
        {
            InviteWindow inviteWindow = new InviteWindow();
            inviteWindow.OnClickInviteButton += InviteWindowOnClickInviteButton;
            inviteWindow.ShowDialog();
        }
       

        public void MethodSubscribe()
        {
            NotificationManager.OnClickInviteStatusGet += NotificationManagerOnClickInviteStatusGet;

            SearchBoxControl.OnClickFullScreen += SearchBoxControlOnClickFullScreen;
            SearchBoxControl.OnClickScreenNormal += SearchBoxControlOnClickScreenNormal;
            SearchBoxControl.OnClickScreenStrech += SearchBoxControlOnClickScreenStrech;
            SearchBoxControl.OnClickScreenZoom += SearchBoxControlOnClickScreenZoom;

            ServerNetworkManager.InviteRequestReceivedInvoke += ServerNetworkManagerInviteRequestReceivedInvoke;
       
            ServerNetworkManager.ConnectRequestReceivedInvoke += ServerNetworkManagerConnectRequestReceivedInvoke;
            ServerNetworkManager.ConnectRequestStatusInvoke += ServerNetworkManagerConnectRequestStatusInvoke;

            ClientNetworkManager.ConnectedToSeverInvoke += ClientNetworkManagerConnectedToSeverInvoke;
            ClientNetworkManager.ScreenShareUpdateInvoke += ClientNetworkManagerScreenShareUpdateInvoke;
            ClientNetworkManager.DisConnectedToSeverInvoke += ClientNetworkManagerDisConnectedToSeverInvoke;

            ScreenImage.MouseRightButtonUp += ScreenImage_MouseRightButtonUp;
            ScreenImage.MouseRightButtonDown += ScreenImage_MouseRightButtonDown;
            ScreenImage.MouseUp += ScreenImage_MouseUp;
            ScreenImage.MouseMove += ScreenImage_MouseMove;
            ScreenImage.MouseDown += ScreenImage_MouseDown;

            this.KeyDown += Window_KeyDown;
            this.KeyUp += Window_KeyUp;
            Closed += MainWindowClosed;
            SessionTabHeader.OnClickClose += SessionTabHeaderOnClickClose;
        }

        private void ServerNetworkManagerConnectRequestStatusInvoke(object sender, DeskConnectionInformation deskConnectionInformation)
        {
            if (deskConnectionInformation.InviteStatus)
            {
                ClientNetworkManager.DeskConnectionInformation = deskConnectionInformation;
                Task.Run(() => { ClientNetworkManager.ConnectToServer(); });
            }
            else
            {
                NotificationManager.CreateNotification("(" + deskConnectionInformation.ReceiverDesk.Id + ") " + deskConnectionInformation.ReceiverDesk.ProfileName + "Rejected Connection \n Request", NotificationType.Information);
            }

        }

        private void ConnectAcceptWindowOnClickGetStatus(object sender, DeskConnectionInformation deskConnectionInformation)
        {
            ((Window)(sender)).Close();
            ClientNetworkManager.DeskConnectionInformation = deskConnectionInformation;
            ClientNetworkManager.ClientIpaddress = deskConnectionInformation.SenderDesk.IPAddress;
            ClientNetworkManager.InviteRequestSent(deskConnectionInformation);

        }

        private void ServerNetworkManagerConnectRequestReceivedInvoke(object sender, DeskConnectionInformation deskConnectionInformation)
        {
            try { deskConnectionInformation.ReceiverDesk.DesktopImage.Freeze(); } catch { }

            try { deskConnectionInformation.ReceiverDesk.ProfileImage.Freeze(); } catch { }
            try { deskConnectionInformation.SenderDesk.DesktopImage.Freeze(); } catch { }

            try { deskConnectionInformation.SenderDesk.ProfileImage.Freeze(); } catch { }
            Dispatcher.Invoke(() =>
            {
                ConnectAcceptWindow ConnectAcceptWindow = new ConnectAcceptWindow(deskConnectionInformation);
                ConnectAcceptWindow.OnClickGetStatus += ConnectAcceptWindowOnClickGetStatus;
                ConnectAcceptWindow.ShowDialog();
            }
            );

         
        }

        private void SearchBoxControlOnClickScreenZoom(object sender, EventArgs e)
        {
            ScreenImage.Stretch = Stretch.None;
        }

        private void SearchBoxControlOnClickScreenStrech(object sender, EventArgs e)
        {
            ScreenImage.Stretch = Stretch.Fill;
        }

        private void SearchBoxControlOnClickScreenNormal(object sender, EventArgs e)
        {
            ScreenImage.Stretch = Stretch.Uniform;
        }

        private void SearchBoxControlOnClickFullScreen(object sender, EventArgs e)
        {

        }

        private void SessionTabHeaderOnClickClose(object sender, EventArgs e)
        {
            ClientNetworkManagerDisConnectedToSeverInvoke(this, EventArgs.Empty);
        }

        private void MainWindowClosed(object sender, EventArgs e)
        {
            List<Desk> deskProfiles = DeskProfileManager.DeskProfilesDictionary.Values.ToList();
            for (int i = 0; i < deskProfiles.Count; i++)
            {
                DeskProfileManager.UpdateDeskProfiledata(deskProfiles[i]);
            }
        }

        private void ClientNetworkManagerDisConnectedToSeverInvoke(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                MainTabControl.SelectedItem = HomePage;
                SearchBoxControl.Text = "";
                SearchBoxControl.IsReadOnly = false;
                SearchBoxControl.IsConnected = false;
                SessionTabHeader.IsCloseButtonVisible = false;
                SessionTabHeader.Header = "New session";
                ScreenImage.Focus();
                NewSessionCreateButton.Focus();
            });
        }

        private void ClientNetworkManagerScreenShareUpdateInvoke(object sender, BitmapImage e)
        {
            Dispatcher.Invoke(() =>
            {
                ScreenImage.Source = e;
            });
        }

        private void ClientNetworkManagerConnectedToSeverInvoke(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                MainTabControl.SelectedItem = ScreenSharePage;
                SearchBoxControl.Text = "" + ClientNetworkManager.deskConnectionInformation.ReceiverDesk.Id;
                SearchBoxControl.IsReadOnly = true;
                SearchBoxControl.IsConnected = true;
                ScreenImage.Focus();
                SessionTabHeader.IsCloseButtonVisible = true;
                SessionTabHeader.Header = "" + ClientNetworkManager.deskConnectionInformation.ReceiverDesk.Id;
            });
        }

        private void DeskProfile1OnclickConnect(object sender, EventArgs e)
        {
            SelectedDeskProfile = (DeskProfile)(sender);
            DeskConnectionInformation deskConnectionInformation = new DeskConnectionInformation() { AccessType = AccessType.FullAccess, ConnectionType=ConnectionType.Connect ,AudioAccess = true, ClipboardAccess = true, KeyboardAccess = true, MouseAccess = true, SenderDesk = DeskProfileManager.UserDesk, ReceiverDesk = SelectedDeskProfile.Desk };

            ClientNetworkManager.DeskConnectionInformation = deskConnectionInformation;

            ClientNetworkManager.InviteRequestSent(deskConnectionInformation);

            //ClientNetworkManager.ClientIpaddress = SystemInformationManager.GetPcIPAddress(SelectedDeskProfile.Desk.HostName);
            //if (SelectedDeskProfile.Desk.IPAddress != ClientNetworkManager.ClientIpaddress)
            //{
            //    SelectedDeskProfile.Desk.IPAddress = ClientNetworkManager.ClientIpaddress;
            //}
            //if (!ClientNetworkManager.isConnected)
            //{
            //    Task.Run(() => ClientNetworkManager.ConnectToServer());
            //}
        }

        private void DeskSwicthControlOnclickRecentSessionsButton(object sender, EventArgs e)
        {
            DeskMainContainer.Children.Remove(RecentSessionsDeskContainer);
            DeskMainContainer.Children.Insert(0, RecentSessionsDeskContainer);
        }

        private void DeskSwicthControlOnclickFavoritesButton(object sender, EventArgs e)
        {
            DeskMainContainer.Children.Remove(FavoritesDeskContainer);
            DeskMainContainer.Children.Insert(0, FavoritesDeskContainer);

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
            if (WindowState == WindowState.Maximized)
            {
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
        private void MenuContextShow()
        {
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
            //Visibility = Visibility.Hidden;
            //ShowInTaskbar = false;
            //ServerNetworkManager.StartServer;
            ServerNetworkManager.StartServer();
            // ServerNetworkManager.isStarted = true;

        }


        //Screen Share Client Details
        public void ScreenImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point position = e.GetPosition(this);
            ClientNetworkManager.SendMouseScrollEvent("MouseScroll", position.X, position.Y, e.Delta);
        }

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
                ClientNetworkManager.SendMouseEvent(position, "MouseUp", ScreenImage.ActualWidth, ScreenImage.ActualHeight);
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
            if (RecentSessionsDeskControlContainer.MaxHeight == Double.PositiveInfinity)
            {
                RecentSessionsDeskControlContainer.MaxHeight = 220;
            }
            else
            {

                RecentSessionsDeskControlContainer.MaxHeight = Double.PositiveInfinity;
            }
        }

        private void FavoritesShowMoreClick(object sender, RoutedEventArgs e)
        {
            if (FavoritesDeskControlContainer.MaxHeight == Double.PositiveInfinity)
            {
                FavoritesDeskControlContainer.MaxHeight = 220;
            }
            else
            {
                FavoritesDeskControlContainer.MaxHeight = Double.PositiveInfinity;
            }
        }

        private void DiscoveredShowMoreClick(object sender, RoutedEventArgs e)
        {
            if (DiscoveredDeskControlContainer.MaxHeight == Double.PositiveInfinity)
            {
                DiscoveredDeskControlContainer.MaxHeight = 220;
            }
            else
            {

                DiscoveredDeskControlContainer.MaxHeight = Double.PositiveInfinity;
            }
        }

        private void NewSessionCreateButtonOnClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }




        //Connection window
        private BitmapImage _gifImage;
        private DispatcherTimer _timer;
        private int _currentFrame = 0;
        private BitmapFrame[] _frames;

        private void LoadAnimatedGif(string fileName)
        {
            //var uri = new Uri($"D:\\loadingGif.gif", UriKind.RelativeOrAbsolute);
            //var decoder = new GifBitmapDecoder(uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            //_frames = decoder.Frames.ToArray();
            //_timer = new DispatcherTimer();
            //_timer.Interval = TimeSpan.FromMilliseconds(50); // Adjust the interval to control the animation speed
            //_timer.Tick += (s, e) =>
            //{
            //    _currentFrame = (_currentFrame + 1) % _frames.Length;
            //    AnimatedGifImage.Source = _frames[_currentFrame];
            //};
            //_timer.Start();
        }

        private void ConnectionCancelButtonClick(object sender, RoutedEventArgs e)
        {

        }



    }
}
