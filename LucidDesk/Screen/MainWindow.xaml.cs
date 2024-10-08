﻿using LucidDesk.Manager;
using LucidDesk.Manager.Classes;
using LucidDesk.Manager.Database;
using LucidDesk.Manager.Enum;
using LucidDesk.UserControls;
using LucidDesk.UserControls.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace LucidDesk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ContextMenu MenuContext;
        public static UserControls.Common.NotificationManager NotificationManager = new UserControls.Common.NotificationManager();
        public static ClientNetworkManager ClientNetworkManager = new ClientNetworkManager();
        public static ServerNetworkManager ServerNetworkManager = new ServerNetworkManager();
        public static DeskProfile SelectedDeskProfile { get; set; }
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



        }

        public void Load()
        {
            Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {

            MainTabControl.SelectedItem = HomePage;
            SelectedSettingPageButton = AccountButton;
            MenuContext = this.Resources["MenuContext"] as ContextMenu;
            DeskSwicthControl.OnclickDiscoverdButton += DeskSwicthControlOnclickDiscoverdButton;
            DeskSwicthControl.OnclickFavoritesButton += DeskSwicthControlOnclickFavoritesButton;
            DeskSwicthControl.OnclickRecentSessionsButton += DeskSwicthControlOnclickRecentSessionsButton;
            MethodSubscribe();
            SetUpCheck();
            LoadAnimatedGif("YourGifFile.gif");
            //   IsConnected = true;
            //MainTabControl.SelectedItem = ScreenSharePage;

        }


        private void SetUpCheck()
        {

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
                           
                            Foreground = Brushes.White,
                            Margin = new Thickness(10)
                        };
                        deskProfile.Desk = deskProfileList[i];
                        deskProfile.OnClickConnect += DeskProfileOnclickConnect;
                        deskProfile.OnInviteConnect += DeskProfileOnInviteConnect;
                        deskProfile.OnClickIsFavorite += DeskProfileOnClickIsFavorite;
                        RecentSessionsDeskControlContainer.Children.Add(deskProfile);
                    }
                    if (deskProfileList[i].IsFavorite)
                    {
                        DeskProfile deskProfile = new DeskProfile()
                        {

                            Foreground = Brushes.White,
                            Margin = new Thickness(10)
                        };
                        deskProfile.Desk = deskProfileList[i];
                        deskProfile.OnClickConnect += DeskProfileOnclickConnect;
                        deskProfile.OnClickIsFavorite += DeskProfileOnClickIsFavorite;
                        deskProfile.OnInviteConnect += DeskProfileOnInviteConnect;
                        FavoritesDeskControlContainer.Children.Add(deskProfile);

                    }
                    DeskProfile deskProfileDicoverd = new DeskProfile()
                    {
                       
                        Foreground = Brushes.White,
                        Margin = new Thickness(10)
                    };
                    deskProfileDicoverd.Desk = deskProfileList[i];
                    deskProfileDicoverd.OnClickConnect += DeskProfileOnclickConnect;
                    deskProfileDicoverd.OnClickIsFavorite += DeskProfileOnClickIsFavorite;
                    deskProfileDicoverd.OnInviteConnect += DeskProfileOnInviteConnect;
                    DiscoveredDeskControlContainer.Children.Add(deskProfileDicoverd);
                    RecentSessionsDeskControlContainer.MaxHeight = 210;
                    FavoritesDeskControlContainer.MaxHeight = 210;
                    DiscoveredDeskControlContainer.MaxHeight = 210;
                }
                else
                {
                    TextblockId.Text = "" + deskProfileList[i].Id;
                }
            }
        }

        private void DeskProfileOnClickIsFavorite(object sender, EventArgs e)
        {
            DeskProfile deskProfile = (DeskProfile)(sender);
            if (deskProfile.Desk.IsFavorite)
            {

                DeskProfile deskProfileNewObj = new DeskProfile(deskProfile.Desk)
                {
                   
                    Foreground = Brushes.White,
                  
                };

                deskProfileNewObj.OnClickConnect += DeskProfileOnclickConnect;
                deskProfileNewObj.OnClickIsFavorite += DeskProfileOnClickIsFavorite;
                deskProfileNewObj.OnInviteConnect += DeskProfileOnInviteConnect;
                FavoritesDeskControlContainer.Children.Insert(0, deskProfileNewObj);
            }
            else
            {

                foreach (var deskProfileObjInFavorites in FavoritesDeskControlContainer.Children)
                {
                    if (((DeskProfile)deskProfileObjInFavorites).DeskId == deskProfile.DeskId)
                    {
                        ((DeskProfile)deskProfileObjInFavorites).Dispose();
                        FavoritesDeskControlContainer.Children.Remove((DeskProfile)deskProfileObjInFavorites);
                        break;
                    }
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
            try { deskConnectionInformation.SenderDesk.DesktopImage.Freeze(); } catch { }
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
                    DeskProfileManager.DeskProfilesDictionary[deskConnectionInformation.SenderDesk.Id + ""].RecentLoginTime = DateTime.Now;
                    RecentSessionAdd(deskConnectionInformation.SenderDesk);
                    Task.Run(() => ClientNetworkManager.ConnectToServer());
                }
            }
        }



        private void InviteWindowOnClickInviteButton(object sender, DeskConnectionInformation deskConnectionInformation)
        {
            ClientNetworkManager.DeskConnectionInformation = deskConnectionInformation;
            ((Window)(sender)).Close();
            Task.Run(() =>
            {
                ClientNetworkManager.InviteRequestSent(deskConnectionInformation);
            });


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
            SearchBoxControl.OnClickConnect += DeskProfileOnclickConnect;

            ServerNetworkManager.InviteRequestReceivedInvoke += ServerNetworkManagerInviteRequestReceivedInvoke;

            ServerNetworkManager.ConnectRequestReceivedInvoke += ServerNetworkManagerConnectRequestReceivedInvoke;
            ServerNetworkManager.ConnectRequestStatusInvoke += ServerNetworkManagerConnectRequestStatusInvoke;

            ClientNetworkManager.ConnectedToSeverInvoke += ClientNetworkManagerConnectedToSeverInvoke;
            ClientNetworkManager.ScreenShareUpdateInvoke += ClientNetworkManagerScreenShareUpdateInvoke;
            ClientNetworkManager.DisConnectedToSeverInvoke += ClientNetworkManagerDisConnectedToSeverInvoke;
            ClientNetworkManager.ConnectionEstabishFailInvoke += ClientNetworkManagerConnectionEstabishFailInvoke;

            ScreenImage.MouseRightButtonUp += ScreenImage_MouseRightButtonUp;
            ScreenImage.MouseRightButtonDown += ScreenImage_MouseRightButtonDown;
            ScreenImage.MouseWheel += ScreenImage_MouseWheel;
            ScreenImage.MouseUp += ScreenImage_MouseUp;
            ScreenImage.MouseMove += ScreenImage_MouseMove;
            ScreenImage.MouseDown += ScreenImage_MouseDown;

            this.KeyDown += Window_KeyDown;
            this.KeyUp += Window_KeyUp;
            Closed += MainWindowClosed;
            SessionTabHeader.OnClickClose += SessionTabHeaderOnClickClose;
        }

        private void ClientNetworkManagerConnectionEstabishFailInvoke(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (MainTabControl.SelectedItem == ConnectionSharePage)
                {
                    MainTabControl.SelectedItem = HomePage;
                    ConnectionGifTimer.Stop();
                    NotificationManager.CreateNotification("Connection Establish fail", NotificationType.Information);
                }
            });
           
        }

        private void ServerNetworkManagerConnectRequestStatusInvoke(object sender, DeskConnectionInformation deskConnectionInformation)
        {
            if (deskConnectionInformation.InviteStatus)
            {
                
                ClientNetworkManager.DeskConnectionInformation = deskConnectionInformation;
                
                Task.Run(() => {
                    DeskProfileManager.DeskProfilesDictionary[deskConnectionInformation.ReceiverDesk.Id + ""].RecentLoginTime = DateTime.Now;
                    RecentSessionAdd(deskConnectionInformation.ReceiverDesk);
                    ClientNetworkManager.ConnectToServer(); });
            }
            else
            {
                Dispatcher.Invoke(() => { NotificationManager.CreateNotification("(" + deskConnectionInformation.ReceiverDesk.Id + ") " + deskConnectionInformation.ReceiverDesk.ProfileName + " Rejected Connection \n Request", NotificationType.Information); });
            }

        }
         private void RecentSessionAdd(Desk desk){
            DeskProfile deskProfile = new DeskProfile()
            {

                Foreground = Brushes.White,
                Margin = new Thickness(10)
            };
            deskProfile.Desk = desk;
            deskProfile.OnClickConnect += DeskProfileOnclickConnect;
            deskProfile.OnInviteConnect += DeskProfileOnInviteConnect;
            deskProfile.OnClickIsFavorite += DeskProfileOnClickIsFavorite;
            RecentSessionsDeskControlContainer.Children.Add(deskProfile);
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
            //ClientNetworkManagerDisConnectedToSeverInvoke(this, EventArgs.Empty);
            ClientNetworkManager.ConnectionClose();
        }

        private void MainWindowClosed(object sender, EventArgs e)
        {
            List<Desk> deskProfiles = DeskProfileManager.DeskProfilesDictionary.Values.ToList();
            for (int i = 0; i < deskProfiles.Count; i++)
            {
                DeskProfileManager.UpdateDeskProfiledata(deskProfiles[i]);
            }
            ClientNetworkManager.ConnectionClose();

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
                if (MainTabControl.SelectedItem == ConnectionSharePage)
                {
                    MainTabControl.SelectedItem = ScreenSharePage;
                    ConnectionGifTimer.Stop();
                }
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

        private void DeskProfileOnclickConnect(object sender, Desk desk)
        {

            MainTabControl.SelectedItem = ConnectionSharePage;
            ConnectionGifTimer.Start();
            DeskConnectionInformation deskConnectionInformation = new DeskConnectionInformation() { AccessType = AccessType.FullAccess, ConnectionType = ConnectionType.Connect, AudioAccess = true, ClipboardAccess = true, KeyboardAccess = true, MouseAccess = true, SenderDesk = DeskProfileManager.UserDesk, ReceiverDesk = desk };

            ClientNetworkManager.DeskConnectionInformation = deskConnectionInformation;
            Task.Run(() =>
            {
                ClientNetworkManager.InviteRequestSent(deskConnectionInformation);
            });

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
                RecentSessionsDeskControlContainer.MaxHeight = 210;
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
                FavoritesDeskControlContainer.MaxHeight = 210;
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
                DiscoveredDeskControlContainer.MaxHeight = 210;
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
        private DispatcherTimer ConnectionGifTimer;
        private int _currentFrame = 0;
        private BitmapFrame[] _frames;

        private void LoadAnimatedGif(string fileName)
        {
            //var uri = new Uri("C:/Users/Lucid/Downloads/loadingGif.gif", UriKind.RelativeOrAbsolute);
            //var decoder = new GifBitmapDecoder(uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            //_frames = decoder.Frames.ToArray();
            //ConnectionGifTimer = new DispatcherTimer();
            //ConnectionGifTimer.Interval = TimeSpan.FromMilliseconds(50); // Adjust the interval to control the animation speed
            //ConnectionGifTimer.Tick += (s, e) =>
            //{
            //    _currentFrame = (_currentFrame + 1) % _frames.Length;
            //    AnimatedGifImage.Source = _frames[_currentFrame];
            //};
            var uri = new Uri("pack://application:,,,/LucidDesk;component/Resources/loadingGif.gif", UriKind.Absolute);
            var decoder = new GifBitmapDecoder(uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            _frames = decoder.Frames.ToArray();

            ConnectionGifTimer = new DispatcherTimer();
            ConnectionGifTimer.Interval = TimeSpan.FromMilliseconds(50); // Adjust the interval to control the animation speed
            ConnectionGifTimer.Tick += (s, e) =>
            {
                _currentFrame = (_currentFrame + 1) % _frames.Length;
                AnimatedGifImage.Source = _frames[_currentFrame];
            };

        }

        private void ConnectionCancelButtonClick(object sender, RoutedEventArgs e)
        {

            Dispatcher.Invoke(() =>
            {
                if (MainTabControl.SelectedItem == ConnectionSharePage)
                {
                    MainTabControl.SelectedItem = HomePage;
                    ConnectionGifTimer.Stop();
                    ClientNetworkManager.ConnectionClose();
                }
            });
        }


        //SettingPage
        CheckBox SelectedSettingPageButton = null;
        private void SettingPageTabButtonClick(object sender, RoutedEventArgs e)
        {
            ((CheckBox)sender).IsChecked = true;
            if (SelectedSettingPageButton != null)
                SelectedSettingPageButton.IsChecked = false;
            SelectedSettingPageButton = ((CheckBox)sender);
        }

        private void SettingPageClick(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = SettingPage;
        }

        private void HeaderSessionSwitchClick(object sender, MouseButtonEventArgs e)
        {
            if (IsConnected)
            {
                MainTabControl.SelectedItem = ScreenSharePage;
            }
            else
            {
                MainTabControl.SelectedItem = HomePage;
            }
        }
    }
}
