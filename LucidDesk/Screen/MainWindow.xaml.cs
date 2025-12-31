using LucidDesk.Log;
using LucidDesk.Manager;
using LucidDesk.Manager.Classes;
using LucidDesk.Manager.Classes.DataSchema;
using LucidDesk.Manager.Database;
using LucidDesk.Manager.Enum;
using LucidDesk.Screen;
using LucidDesk.Settings;
using LucidDesk.UserControls;
using LucidDesk.UserControls.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
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

        private BitmapImage _gifImage;

        private DispatcherTimer ConnectionGifTimer;

        private int _currentFrame = 0;

        private BitmapFrame[] _frames;

        CheckBox SelectedSettingPageButton = null;

        DeskProfile prevDeskProfile;

        private DateTime _lastSend = DateTime.MinValue;

        private readonly TimeSpan _interval = TimeSpan.FromMilliseconds(20);
        public MainWindow()
        {
            InitializeComponent();
        }
        public void Load()
        {
            Loaded += MainWindowLoaded;
            this.Closed += (sender, e) => Environment.Exit(0);
        }
        #region setapplication
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
        public void MethodSubscribe()
        {
            NotificationManager.OnClickInviteStatusGet += NotificationManagerOnClickInviteStatusGet;

            SearchBoxControl.OnClickFullScreen += SearchBoxControlOnClickFullScreen;
            SearchBoxControl.OnClickScreenNormal += SearchBoxControlOnClickScreenNormal;
            SearchBoxControl.OnClickScreenStretch += SearchBoxControlOnClickScreenStrech;
            SearchBoxControl.OnClickScreenZoom += SearchBoxControlOnClickScreenZoom;
            SearchBoxControl.OnClickConnect += DeskProfileOnclickConnect;
            SearchBoxControl.OnClickConnectWithPassword += DeskProfileOnClickConnectWithPassword;
            DeskProfileManager.DeskProfilesUpdated += DeskProfileManagerDeskProfilesUpdated;

            ServerNetworkManager.InviteRequestReceivedInvoke += ServerNetworkManagerInviteRequestReceivedInvoke;

            ServerNetworkManager.ConnectRequestReceivedInvoke += ServerNetworkManagerConnectRequestReceivedInvoke;
            ServerNetworkManager.ConnectRequestStatusInvoke += ServerNetworkManagerConnectRequestStatusInvoke;

            ClientNetworkManager.ConnectedToSeverInvoke += ClientNetworkManagerConnectedToSeverInvoke;
            ClientNetworkManager.ScreenShareUpdateInvoke += ClientNetworkManagerScreenShareUpdateInvoke;
            ClientNetworkManager.DisConnectedToSeverInvoke += ClientNetworkManagerDisConnectedToSeverInvoke;
            ClientNetworkManager.ConnectionEstabishFailInvoke += ClientNetworkManagerConnectionEstabishFailInvoke;

            ScreenImage.MouseRightButtonUp += ScreenImageMouseRightButtonUp;
            ScreenImage.MouseRightButtonDown += ScreenImageMouseRightButtonDown;
            ScreenImage.MouseWheel += ScreenImageMouseWheel;
            ScreenImage.MouseUp += ScreenImageMouseUp;
            ScreenImage.MouseMove += ScreenImageMouseMove;
            ScreenImage.MouseDown += ScreenImageMouseDown;

            this.KeyDown += Window_KeyDown;
            this.KeyUp += Window_KeyUp;
            Closed += MainWindowClosed;
            SessionTabHeader.OnClickClose += SessionTabHeaderOnClickClose;
        }

        private void SetUpCheck()
        {
            DiscoveredDeskControlContainer.Children.Clear();
            RecentSessionsDeskControlContainer.Children.Clear();
            FavoritesDeskControlContainer.Children.Clear();
            List<Desk> deskProfileList = DeskProfileManager.DeskProfilesDictionary.Values.ToList();
            //DeskProfile Control Create
            for (int i = 0; i < deskProfileList.Count; i++)
            {
                if (SystemInformationManager.MacAddress != deskProfileList[i].MacAddress)
                {
                    if (deskProfileList[i].RecentLoginTime.Month == DateTime.Now.Month && deskProfileList[i].RecentLoginTime.Year == DateTime.Now.Year)
                    {
                        DeskProfile deskProfile = DeskProfileControlCreate(deskProfileList[i]);
                        RecentSessionsDeskControlContainer.Children.Add(deskProfile);
                    }
                    if (deskProfileList[i].IsFavorite)
                    {
                        FavoritesDeskControlContainer.Children.Add(DeskProfileControlCreate(deskProfileList[i]));
                    }
                    DeskProfile deskProfileDicoverd = DeskProfileControlCreate(deskProfileList[i]);
                    DiscoveredDeskControlContainer.Children.Add(deskProfileDicoverd);
                    RecentSessionsDeskControlContainer.MaxHeight = 210;
                    FavoritesDeskControlContainer.MaxHeight = 210;
                    DiscoveredDeskControlContainer.MaxHeight = 210;
                    deskProfileList[i].PropertyChanged += DeskPropertyChanged;
                }
                else
                {
                    if (SettingsManager.Settings.ApplicationMode == ApplicationMode.Online)
                        TextblockId.Text = "" + deskProfileList[i].DisplayID;
                    else
                        TextblockId.Text = "" + SystemInformationManager.IpAddresss;
                }
            }
        }
        #endregion

        #region Desk Information
        private void DeskProfileManagerDeskProfilesUpdated(object sender, EventArgs e)
        {
            SetUpCheck();
        }
        private void DeskPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DeskProfileManager.UpdateDeskProfilesdata(DeskProfileManager.DeskProfiles);
            Desk desk = (Desk)(sender);
            if (e.PropertyName == "IsFavorite")
            {
                if (!desk.IsFavorite)
                {
                    foreach (var deskProfileObjInFavorites in FavoritesDeskControlContainer.Children)
                    {
                        if (((DeskProfile)deskProfileObjInFavorites).Desk.DeskId == desk.DeskId)
                        {
                            ((DeskProfile)deskProfileObjInFavorites).Dispose();
                            FavoritesDeskControlContainer.Children.Remove((DeskProfile)deskProfileObjInFavorites);
                            break;
                        }
                    }
                }
                else
                {
                    if (desk.IsFavorite && !DeskProfileAlreadyExitsInIsFavorite(desk))
                    {
                        DeskProfile deskProfileNewObj = DeskProfileControlCreate(desk);
                        FavoritesDeskControlContainer.Children.Insert(0, deskProfileNewObj);
                    }
                }
            }
        }
        private DeskProfile DeskProfileControlCreate(Desk desk)
        {
            DeskProfile deskProfile = new DeskProfile()
            {
                Foreground = Brushes.White,
                Margin = new Thickness(10)
            };
            deskProfile.Desk = desk;
            deskProfile.OnClickConnectWithPassword += DeskProfileOnClickConnectWithPassword;
            deskProfile.OnClickConnect += DeskProfileOnclickConnect;
            deskProfile.OnClickIsFavorite += DeskProfileOnClickIsFavorite;
            deskProfile.OnClickUnFavorite += DeskProfileOnClickUnFavorite;
            deskProfile.OnInviteConnect += DeskProfileOnInviteConnect;
            return deskProfile;
        }

        private void DeskProfileOnClickConnectWithPassword(object sender, Desk e)
        {
            ConnectWithPassword connectWithPassword = new ConnectWithPassword(e);
            connectWithPassword.OnClickConnectButton += ConnectWithPasswordOnClickConnectButton;
            connectWithPassword.ShowDialog();
        }

        private void ConnectWithPasswordOnClickConnectButton(object sender, Desk e)
        {
            ((Window)sender).Close();
            DeskConnectionInformation deskConnectionInformation = new DeskConnectionInformation() { AccessType = AccessType.FullAccess, ConnectionType = ConnectionType.Password, AudioAccess = true, ClipboardAccess = true, KeyboardAccess = true, MouseAccess = true, SenderDesk = DeskProfileManager.UserDesk, ReceiverDesk = e };
            ConnectToServerCall(deskConnectionInformation);
        }

        private void DeskProfileOnClickUnFavorite(object sender, EventArgs e)
        {

        }
        private bool DeskProfileAlreadyExitsInIsFavorite(Desk desk)
        {

            for (int i = 0; i < FavoritesDeskControlContainer.Children.Count; i++)
            {
                if (((DeskProfile)FavoritesDeskControlContainer.Children[i]).Desk.DeskId == desk.DeskId)
                {
                    return true;
                }
            }
            return false;
        }
        private void DeskProfileOnClickIsFavorite(object sender, EventArgs e)
        {
            DeskProfile deskProfile = (DeskProfile)(sender);
        }
        #endregion
        //Invite and connect Part
        #region Search Box
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

        #endregion
        private void SessionTabHeaderOnClickClose(object sender, EventArgs e)
        {
            //ClientNetworkManagerDisConnectedToSeverInvoke(this, EventArgs.Empty);
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ClientNetworkManager.ConnectionClose();
            }
            ));

        }

        #region Desk Section
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
        #endregion

        #region Menu
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
        #endregion

        #region   Screen Share
        private void ClientNetworkManagerScreenShareUpdateInvoke(object sender, DeskImageData e)
        {

            Dispatcher.Invoke(() =>
            {
                if (MainTabControl.SelectedItem == ConnectionSharePage)
                {
                    ClientNetworkManagerConnectedToSeverInvoke(null, EventArgs.Empty);
                    ConnectionGifTimer.Stop();
                }
                ScreenImage.Source = e.Image;
            });
        }

        public void ScreenImageMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point position = e.GetPosition(this);
            ClientNetworkManager.SendMouseScrollEvent(ControlKeyType.Scroll, position.X, position.Y, e.Delta);
        }

        private void ScreenImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ClientNetworkManager.isConnected)
            {
                Point position = e.GetPosition(ScreenImage);
                ClientNetworkManager.SendMouseEvent(ControlKeyType.MouseDown, position, ScreenImage.ActualWidth, ScreenImage.ActualHeight);
            }
        }

        private void ScreenImageMouseMove(object sender, MouseEventArgs e)
        {
            if (DateTime.UtcNow - _lastSend < _interval)
                return;
            _lastSend = DateTime.UtcNow;

            if (ClientNetworkManager.isConnected)
            {
                Point position = e.GetPosition(ScreenImage);
                ClientNetworkManager.SendMouseEvent(ControlKeyType.MouseMove, position, ScreenImage.ActualWidth, ScreenImage.ActualHeight);
            }
        }

        private void ScreenImageMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ClientNetworkManager.isConnected)
            {
                Point position = e.GetPosition(ScreenImage);
                ClientNetworkManager.SendMouseEvent(ControlKeyType.MouseUp, position, ScreenImage.ActualWidth, ScreenImage.ActualHeight);
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
            ClientNetworkManager.SendKeyEvent(ControlKeyType.KeyDown, e.Key);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            ClientNetworkManager.SendKeyEvent(ControlKeyType.KeyUp, e.Key);
        }

        private void ScreenImageMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ClientNetworkManager.isConnected)
            {
                Point position = e.GetPosition(ScreenImage);
                ClientNetworkManager.SendMouseRightEvent(ControlKeyType.MouseRightDown, position, ScreenImage.ActualWidth, ScreenImage.ActualHeight);
            }
        }

        private void ScreenImageMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ClientNetworkManager.isConnected)

            {
                Point position = e.GetPosition(ScreenImage);
                ClientNetworkManager.SendMouseRightEvent(ControlKeyType.MouseRightUp, position, ScreenImage.ActualWidth, ScreenImage.ActualHeight);
            }
        }

        private void ButtonPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        private void NewSessionCreateButtonOnClick(object sender, RoutedEventArgs e)
        {

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void LoadAnimatedGif(string fileName)
        {
            try
            {
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
            catch (Exception e)
            {

            }


        }

        private void SettingPageTabButtonClick(object sender, RoutedEventArgs e)
        {
            ((CheckBox)sender).IsChecked = true;
            if (SelectedSettingPageButton != null)
                SelectedSettingPageButton.IsChecked = false;
            SelectedSettingPageButton = ((CheckBox)sender);
            SelectedSettingPageButton.IsChecked = true;
            if (SelectedSettingPageButton == AccountButton)
                settingPageTabControl.SelectedItem = ProfilePage;
            if (SelectedSettingPageButton == HelpButton)
                settingPageTabControl.SelectedItem = HelpPage;
        }

        private void SettingPageClick(object sender, RoutedEventArgs e)
        {

            MainTabControl.SelectedItem = SettingPage;
            settingPageTabControl.SelectedItem = ProfilePage;
            SelectedSettingPageButton = AccountButton;
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

        private void MainTabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedItem == SettingPage)
            {
                ProfilePicture.Image = DeskProfileManager.UserDesk.ProfileImage;
                ProfilePageUserNameTextBox.Textbox.Text = DeskProfileManager.UserDesk.ProfileName;
                ProfilePageUserDeskOsName.Content = DeskProfileManager.UserDesk.OsName;
                ProfilePageUserDeskId.Content = "" + DeskProfileManager.UserDesk.DisplayID;
                ProfilePageUserDeskName.Content = "" + DeskProfileManager.UserDesk.PcName;
            }
        }

        private void HelpButtonClick(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)(sender);
            checkBox.IsChecked = true;
            SelectedSettingPageButton.IsChecked = false;

        }

        #region Invite
        private void DeskProfileOnInviteConnect(object sender, Desk desk)
        {
            InviteWindow inviteWindow = new InviteWindow(desk);
            inviteWindow.OnClickInviteButton += InviteWindowOnClickInviteButton;
            inviteWindow.ShowDialog();
        }

        private void ServerNetworkManagerInviteRequestReceivedInvoke(object sender, DeskConnectionInformation deskConnectionInformation)
        {
            if (DeskProfileManager.DeskProfilesDictionary.ContainsKey("" + deskConnectionInformation.SenderDesk.DeskId))
                deskConnectionInformation.SenderDesk = DeskProfileManager.DeskProfilesDictionary["" + deskConnectionInformation.SenderDesk.DeskId];
            else
                DeskProfileManager.DeskProfilesDictionary.Add("" + deskConnectionInformation.SenderDesk.DeskId, deskConnectionInformation.SenderDesk);
            try
            {
                deskConnectionInformation.ReceiverDesk.DesktopImage.Freeze();
                deskConnectionInformation.ReceiverDesk.ProfileImage.Freeze();
                deskConnectionInformation.SenderDesk.DesktopImage.Freeze();
                deskConnectionInformation.SenderDesk.ProfileImage.Freeze();
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex.ToString());
            }
            Dispatcher.Invoke(() => { NotificationManager.CreateInviteRequestNotification(deskConnectionInformation); });
        }

        private void NotificationManagerOnClickInviteStatusGet(object sender, DeskConnectionInformation deskConnectionInformation)
        {
            if (deskConnectionInformation.Status == true)
            {
                ClientNetworkManager.DeskConnectionInformation = deskConnectionInformation;
                ClientNetworkManager.ClientIpaddress = deskConnectionInformation.SenderDesk.IPAddress;
                if (!ClientNetworkManager.isConnected)
                {
                    DeskProfileManager.DeskProfilesDictionary[deskConnectionInformation.SenderDesk.DeskId + ""].RecentLoginTime = DateTime.Now;
                    RecentSessionAdd(deskConnectionInformation.SenderDesk);
                    ConnectToServerCall(deskConnectionInformation);
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
        #endregion

        #region Connection

        private void DeskProfileOnclickConnect(object sender, Desk desk)
        {

            DeskConnectionInformation deskConnectionInformation = new DeskConnectionInformation() { AccessType = AccessType.FullAccess, ConnectionType = ConnectionType.Connect, AudioAccess = true, ClipboardAccess = true, KeyboardAccess = true, MouseAccess = true, SenderDesk = DeskProfileManager.UserDesk, ReceiverDesk = desk };
            ConnectToServerCall(deskConnectionInformation);

        }
        private void ConnectToServerCall(DeskConnectionInformation deskConnectionInformation)
        {
            MainTabControl.SelectedItem = ConnectionSharePage;
            ConnectionGifTimer.Start();
            Task.Run(() => ClientNetworkManager.ConnectToServer(deskConnectionInformation));
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

        private void ClientNetworkManagerDisConnectedToSeverInvoke(object sender, string message)
        {

            Dispatcher.BeginInvoke(new Action(() =>
            {
                MainTabControl.SelectedItem = HomePage;
                SearchBoxControl.Text = "";
                SearchBoxControl.IsReadOnly = false;
                SearchBoxControl.IsConnected = false;
                SessionTabHeader.IsCloseButtonVisible = false;
                SessionTabHeader.Header = "New session";
                ScreenImage.Focus();
                NewSessionCreateButton.Focus();
            }));
            MessageBox2.ShowMessageBox(message, "Error", MessageBoxType.Ok, this);
        }

        private void ClientNetworkManagerConnectedToSeverInvoke(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                MainTabControl.SelectedItem = ScreenSharePage;
                SearchBoxControl.Text = "" + ClientNetworkManager.deskConnectionInformation.ReceiverDesk.DisplayID;
                SearchBoxControl.IsReadOnly = true;
                SearchBoxControl.IsConnected = true;
                ScreenImage.Focus();
                SessionTabHeader.IsCloseButtonVisible = true;
                SessionTabHeader.Header = "" + ClientNetworkManager.deskConnectionInformation.ReceiverDesk.DisplayID;
            }));
        }

        private void ClientNetworkManagerConnectionEstabishFailInvoke(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (MainTabControl.SelectedItem == ConnectionSharePage)
                {
                    MainTabControl.SelectedItem = HomePage;
                    ConnectionGifTimer.Stop();
                    NotificationManager.CreateNotification("Connection Establish fail", NotificationType.Information);
                }
            }));

        }

        private void ServerNetworkManagerConnectRequestStatusInvoke(object sender, DeskConnectionInformation deskConnectionInformation)
        {
            if (deskConnectionInformation.statusUpdate == false)
            {

            }
            if (deskConnectionInformation.Status)
            {
                ClientNetworkManager.DeskConnectionInformation = deskConnectionInformation;
                Task.Run(() =>
                {
                    DeskProfileManager.DeskProfilesDictionary[deskConnectionInformation.ReceiverDesk.DeskId + ""].RecentLoginTime = DateTime.Now;
                    RecentSessionAdd(deskConnectionInformation.ReceiverDesk);
                    try
                    {
                        ConnectToServerCall(deskConnectionInformation);
                    }
                    catch
                    {

                    }
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    MainTabControl.SelectedItem = HomePage;
                    ConnectionGifTimer.Stop();
                    NotificationManager.CreateNotification("(" + deskConnectionInformation.ReceiverDesk.DeskId + ") " + deskConnectionInformation.ReceiverDesk.ProfileName + " Rejected Connection \n Request", NotificationType.Information);
                });
            }

        }

        private void RecentSessionAdd(Desk desk)
        {
            Dispatcher.Invoke(() =>
            {
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
            });
        }

        private void ConnectAcceptWindowOnClickGetStatus(object sender, DeskConnectionInformation deskConnectionInformation)
        {
            ((Window)(sender)).Close();
            if (deskConnectionInformation.Status == true)
            {
                ClientNetworkManager.DeskConnectionInformation.statusUpdate = true;
                if (!DeskProfileManager.DeskProfilesDictionary.ContainsKey("" + deskConnectionInformation.SenderDesk.DeskId))
                {
                    DeskProfileManager.CreateDeskProfiledata(deskConnectionInformation.SenderDesk);

                }
                ServerNetworkManager.RequestUpdate(deskConnectionInformation);
            }


        }

        private void ServerNetworkManagerConnectRequestReceivedInvoke(object sender, DeskConnectionInformation deskConnectionInformation)
        {
            try
            {
                deskConnectionInformation.ReceiverDesk.DesktopImage?.Freeze();
                deskConnectionInformation.ReceiverDesk.ProfileImage?.Freeze();
                deskConnectionInformation.SenderDesk.DesktopImage?.Freeze();
                deskConnectionInformation.SenderDesk.ProfileImage?.Freeze();
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex.ToString());
            }
            Dispatcher.Invoke(() =>
            {
                ConnectAcceptWindow ConnectAcceptWindow = new ConnectAcceptWindow(deskConnectionInformation);
                ConnectAcceptWindow.OnClickGetStatus += ConnectAcceptWindowOnClickGetStatus;
                ConnectAcceptWindow.ShowDialog();
            }
            );
        }
        #endregion

        #region   Mainwindow
        private void MainWindowClosed(object sender, EventArgs e)
        {
            List<Desk> deskProfiles = DeskProfileManager.DeskProfilesDictionary.Values.ToList();
            for (int i = 0; i < deskProfiles.Count; i++)
            {
                DeskProfileManager.UpdateDeskProfiledata(deskProfiles[i]);
            }
            ClientNetworkManager.ConnectionClose();

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
        #endregion
    }
}
