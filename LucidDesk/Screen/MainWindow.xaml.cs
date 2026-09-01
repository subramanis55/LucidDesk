using LucidDesk.DS.Classes;
using LucidDesk.DS.Enum;
using LucidDesk.Manager;
using LucidDesk.Manager.Connection;
using LucidDesk.Manager.Database;
using LucidDesk.Manager.Files;
using LucidDesk.Manager.Settings;
using LucidDesk.Screen;
using LucidDesk.UserControls;
using LucidDesk.UserControls.Common;
using LucidDesK.DS.DataSchema;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ConnectionHandler ConnectionHandler;
        public static DeskProfile SelectedDeskProfile { get; set; }

        private BitmapImage _gifImage;

        private DispatcherTimer ConnectionGifTimer;

        private int _currentFrame = 0;

        private BitmapFrame[] _frames;

        CheckBox SelectedSettingPageButton = null;

        DeskProfile prevDeskProfile;

        private DateTime _lastSend = DateTime.MinValue;

        private readonly TimeSpan _interval = TimeSpan.FromMilliseconds(20);

        private bool isFullScreenMode;

        public bool IsConnected
        {
            get
            {
                return ConnectionHandler?.IsConnected ?? false;
            }
        }

        public bool FullScreenMode
        {
            get
            {
                return isFullScreenMode;
            }
            set
            {
                isFullScreenMode = value;
                if (isFullScreenMode)
                {
                    normalScreenButton.Visibility = Visibility.Visible;
                    maingrid.RowDefinitions[1].Height = new GridLength(0);
                }
                else
                {
                    normalScreenButton.Visibility = Visibility.Hidden;
                    maingrid.RowDefinitions[1].Height = new GridLength(60);
                }
            }

        }

        private LucidDesK.DS.DataSchema.Screens.Screen selectedScreen;

        public MainWindow()
        {
            InitializeComponent();
            profilePagePasswordTextBox.IsPassword = true;
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
            headerText.Text = SettingsManager.Settings.ApplicationName;
            serverStausIndicater.Background = ConnectionManager.IsSeverConnected ? Brushes.Green : Brushes.Red;
            SelectedSettingPageButton = AccountButton;
            MenuContext = this.Resources["MenuContext"] as ContextMenu;
            DeskSwicthControl.OnclickDiscoverdButton += DeskSwicthControlOnclickDiscoverdButton;
            DeskSwicthControl.OnclickFavoritesButton += DeskSwicthControlOnclickFavoritesButton;
            DeskSwicthControl.OnclickRecentSessionsButton += DeskSwicthControlOnclickRecentSessionsButton;
            MethodSubscribe();
            SetUpCheck();

            LoadAnimatedGif("YourGifFile.gif");
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

            ConnectionManager.ServerConnectionStatusInvoke += ConnectionManagerServerConnectionStatusInvoke;
            ConnectionManager.InviteRequestReceivedInvoke += ConnectionManagerInviteRequestReceivedInvoke;
            ConnectionManager.ConnectRequestReceivedInvoke += ConnectionManagerConnectRequestReceivedInvoke;
            ConnectionManager.ConnectRequestStatusInvoke += ConnectionManagerConnectRequestStatusInvoke;
            ConnectionManager.ConnectionResponseReceived += ClientNetworkManagerConnectionResponseReceived;

            screenSwitchControl.ScreenSelectionChanged += ScreenSwitchControlScreenSelectionChanged;

            ScreenImage.MouseRightButtonUp += ScreenImageMouseRightButtonUp;
            ScreenImage.MouseRightButtonDown += ScreenImageMouseRightButtonDown;
            ScreenImage.MouseWheel += ScreenImageMouseWheel;
            ScreenImage.MouseLeftButtonUp += ScreenImageMouseUp;
            ScreenImage.MouseMove += ScreenImageMouseMove;
            ScreenImage.MouseLeftButtonDown += ScreenImageMouseDown;

            this.KeyDown += Window_KeyDown;
            this.KeyUp += Window_KeyUp;
            Closed += MainWindowClosed;
            SessionTabHeader.OnClickClose += SessionTabHeaderOnClickClose;
        }

        private void ConnectionManagerServerConnectionStatusInvoke(bool severConnectionStatus)
        {
            serverStausIndicater.Background = severConnectionStatus ? Brushes.Green : Brushes.Red;
        }

        private void ScreenSwitchControlScreenSelectionChanged(object sender, LucidDesK.DS.DataSchema.Screens.Screen e)
        {
            selectedScreen = e;
            //ToDo:Screen Index 1
            ConnectionHandler.RemoteInputSender.SendScreenSwitchEvent(1, e);
        }

        private void ClientNetworkManagerConnectionResponseReceived(DeskConnectionInformation e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (e.Status == true && e.IsRequestStatusUpdate == true)
                {
                    //DeskProfileManager.UpdateDeskProfileFromUserdata(e.ReceiverDesk.Clone());
                    screenSwitchControl.UpdateScreenInformation(e.ScreenInformation);
                }
                else
                {
                    NotificationManager.CreateNotification(e.Message, NotificationType.Error);
                    ConnectionHandler.Close();
                    ConnectionDisConnectedToSeverInvoke(null);
                }
            }));
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
                    if (deskProfileList[i].RecentConnectedTime.Month == DateTime.Now.Month && deskProfileList[i].RecentConnectedTime.Year == DateTime.Now.Year)
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
                    deskProfileList[i].PropertyChanged -= DeskPropertyChanged;
                    deskProfileList[i].PropertyChanged += DeskPropertyChanged;
                }
                else
                {
                    if (SystemInformationManager.MacAddress == deskProfileList[i].MacAddress)
                    {
                        if (SettingsManager.Settings.ApplicationMode == ApplicationMode.Online)
                            TextblockId.Text = "" + deskProfileList[i].DisplayID;
                        else
                        {
                            TextblockId.Text = "" + SystemInformationManager.IpAddresss;
                            if (DeskProfileManager.UserDesk.IPAddress != SystemInformationManager.IpAddresss)
                                DeskProfileManager.UserDesk.IPAddress = SystemInformationManager.IpAddresss;
                        }
                    }
                }
            }
        }
        #endregion

        #region Desk Information

        private void DeskProfileManagerDeskProfilesUpdated(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => { SetUpCheck(); }));
        }

        private void DeskPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DeskProfileManager.UpdateDeskProfilesdata();
            Desk desk = (Desk)(sender);
            //if (e.PropertyName == nameof(Desk.IsFavorite))
            //{
            //    if (!desk.IsFavorite)
            //    {
            //        foreach (var deskProfileObjInFavorites in FavoritesDeskControlContainer.Children)
            //        {
            //            if (((DeskProfile)deskProfileObjInFavorites).Desk.DeskId == desk.DeskId)
            //            {
            //                ((DeskProfile)deskProfileObjInFavorites).Dispose();
            //                FavoritesDeskControlContainer.Children.Remove((DeskProfile)deskProfileObjInFavorites);
            //                break;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (desk.IsFavorite && !DeskProfileAlreadyExitsInIsFavorite(desk))
            //        {
            //            DeskProfile deskProfileNewObj = DeskProfileControlCreate(desk);
            //            FavoritesDeskControlContainer.Children.Insert(0, deskProfileNewObj);
            //        }
            //    }
            //}
            //else if (e.PropertyName == nameof(Desk.RecentLoginTime))
            //{
            //    RecentSessionAdd(desk);
            //}
            //else
            //{
            SetUpCheck();
            // }
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
            ConnectWithPassword connectWithPassword = new ConnectWithPassword(e.Clone());
            connectWithPassword.OnClickConnectButton += ConnectWithPasswordOnClickConnectButton;
            connectWithPassword.ShowDialog();
        }

        private void ConnectWithPasswordOnClickConnectButton(object sender, Desk e)
        {
            ((Window)sender).Close();
            DeskConnectionInformation deskConnectionInformation = new DeskConnectionInformation() { AccessType = AccessType.FullAccess, ConnectionType = ConnectionType.Password, AudioAccess = true, ClipboardAccess = true, KeyboardAccess = true, MouseAccess = true, SenderDesk = new DeskInfo(DeskProfileManager.UserDesk), ReceiverDesk = new DeskInfo(e) };
            ConnectToServerCall(deskConnectionInformation, ReponseAndReqType.ConnectReq);
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

        private void NormalScreenButtonOnClick(object sender, RoutedEventArgs e)
        {
            FullScreenMode = false;
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
            FullScreenMode = true;
        }
        #endregion
        private void SessionTabHeaderOnClickClose(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ConnectionHandler.Close();
                FullScreenMode = false;
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

        private void ClientNetworkManagerScreenShareUpdateInvoke(DeskImageData e)
        {
            BitmapImage screenShareImage = e.Image;

            screenShareImage?.Freeze();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (MainTabControl.SelectedItem == ConnectionSharePage)
                {
                    ClientNetworkManagerConnectedToSeverInvoke(null, EventArgs.Empty);
                    ConnectionGifTimer.Stop();
                }
                ScreenImage.Source = screenShareImage;
            }));
        }

        private BitmapImage ExtractSpecificMonitor(System.Drawing.Bitmap fullBitmap, LucidDesK.DS.DataSchema.Screens.Screen targetScreen)
        {
            System.Drawing.Rectangle vs = System.Windows.Forms.SystemInformation.VirtualScreen;
            System.Drawing.Rectangle sb = targetScreen.Bounds;

            // Adjust for virtual screen offset
            System.Drawing.Rectangle sourceRect = new System.Drawing.Rectangle(
        sb.X - vs.X,
        sb.Y - vs.Y,
        sb.Width,
        sb.Height);

            System.Drawing.Bitmap monitorBitmap = new System.Drawing.Bitmap(
        sourceRect.Width,
        sourceRect.Height,
        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(monitorBitmap))
            {
                g.DrawImage(
                    fullBitmap,
                    new System.Drawing.Rectangle(0, 0, sourceRect.Width, sourceRect.Height),
                    sourceRect,
                    System.Drawing.GraphicsUnit.Pixel);
            }

            return FileManager.ConvertBitmapToBitmapImage(monitorBitmap);
        }

        public void ScreenImageMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point position = e.GetPosition(this);
            ConnectionHandler.RemoteInputSender.SendMouseScrollEvent(ControlKeyType.Scroll, position, ScreenImage.ActualWidth, ScreenImage.ActualHeight, e.Delta);
        }

        private void ScreenImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ConnectionHandler != null && ConnectionHandler.IsConnected && !ConnectionHandler.IsRemoteServer)
            {
                Point position = e.GetPosition(ScreenImage);
                //PresentationSource source = PresentationSource.FromVisual(ScreenImage);
                //Matrix transform = source.CompositionTarget.TransformToDevice;
                //position = transform.Transform(position);
                ConnectionHandler.RemoteInputSender.SendMouseEvent(ControlKeyType.MouseDown, position, ScreenImage.ActualWidth, ScreenImage.ActualHeight, selectedScreen);
            }
        }

        private void ScreenImageMouseMove(object sender, MouseEventArgs e)
        {
            if (DateTime.UtcNow - _lastSend < _interval)
                return;
            _lastSend = DateTime.UtcNow;

            if (ConnectionHandler != null && ConnectionHandler.IsConnected && !ConnectionHandler.IsRemoteServer)
            {
                Point position = e.GetPosition(ScreenImage);
                //PresentationSource source = PresentationSource.FromVisual(ScreenImage);
                //Matrix transform = source.CompositionTarget.TransformToDevice;
                //position = transform.Transform(position);
                ConnectionHandler.RemoteInputSender.SendMouseEvent(ControlKeyType.MouseMove, position, ScreenImage.ActualWidth, ScreenImage.ActualHeight, selectedScreen);
            }
        }

        private void ScreenImageMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ConnectionHandler != null && ConnectionHandler.IsConnected && !ConnectionHandler.IsRemoteServer)
            {
                Point position = e.GetPosition(ScreenImage);
                //PresentationSource source = PresentationSource.FromVisual(ScreenImage);
                //Matrix transform = source.CompositionTarget.TransformToDevice;
                //position = transform.Transform(position);
                ConnectionHandler.RemoteInputSender.SendMouseEvent(ControlKeyType.MouseUp, position, ScreenImage.ActualWidth, ScreenImage.ActualHeight);
            }
        }

        //clipboard
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //    if (e.Key == Key.V && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            //    {
            //        ClientNetworkManager.SendClipboardContentToServer();
            //    }
            //    if (e.Key == Key.C && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            //    {

            //    }
            if (ConnectionHandler != null && ConnectionHandler.IsConnected && !ConnectionHandler.IsRemoteServer)
            {
                ConnectionHandler.RemoteInputSender.SendKeyEvent(ControlKeyType.KeyDown, e.Key);
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (ConnectionHandler != null && ConnectionHandler.IsConnected && !ConnectionHandler.IsRemoteServer)
            {
                ConnectionHandler.RemoteInputSender.SendKeyEvent(ControlKeyType.KeyUp, e.Key);
            }
        }

        private void ScreenImageMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ConnectionHandler != null && ConnectionHandler.IsConnected && !ConnectionHandler.IsRemoteServer)
            {
                Point position = e.GetPosition(ScreenImage);
                ConnectionHandler.RemoteInputSender.SendMouseRightEvent(ControlKeyType.MouseRightDown, position, ScreenImage.ActualWidth, ScreenImage.ActualHeight);
            }
        }

        private void ScreenImageMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ConnectionHandler != null && ConnectionHandler.IsConnected && !ConnectionHandler.IsRemoteServer)
            {
                Point position = e.GetPosition(ScreenImage);
                ConnectionHandler.RemoteInputSender.SendMouseRightEvent(ControlKeyType.MouseRightUp, position, ScreenImage.ActualWidth, ScreenImage.ActualHeight);
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
                profilePageUserNameTextBox.TextBoxText = DeskProfileManager.UserDesk.ProfileName;
                profilePagePasswordTextBox.TextBoxText = DeskProfileManager.UserDesk.Password;
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
            if (inviteWindow == null)
            {
                inviteWindow = new InviteWindow();
                inviteWindow.OnClickInviteButton += InviteWindowOnClickInviteButton;
            }
            inviteWindow.InviteDesk(desk.Clone());
            inviteWindow.ShowDialog();
        }

        private void ConnectionManagerInviteRequestReceivedInvoke(DeskConnectionInformation deskConnectionInformation)
        {
            //if (DeskProfileManager.DeskProfilesDictionary.ContainsKey("" + deskConnectionInformation.SenderDesk.DeskId))
            //    deskConnectionInformation.SenderDesk = DeskProfileManager.DeskProfilesDictionary["" + deskConnectionInformation.SenderDesk.DeskId];
            //else
            //    DeskProfileManager.DeskProfilesDictionary.Add("" + deskConnectionInformation.SenderDesk.DeskId, deskConnectionInformation.SenderDesk);
            //try
            //{
            //    deskConnectionInformation.ReceiverDesk?.Freeze();
            //    deskConnectionInformation.SenderDesk?.Freeze();

            //}
            //catch (Exception ex)
            //{
            //    LogManager.LogException(ex.ToString());
            //}
            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    ConnectAcceptWindow connectionAcceptWindow = new ConnectAcceptWindow(deskConnectionInformation);
            //    connectionAcceptWindow.OnClickGetStatus += NotificationManagerOnClickInviteStatusGet;
            //    connectionAcceptWindow.ShowDialog();
            //    //NotificationManager.CreateInviteRequestNotification(deskConnectionInformation); 
            //});
        }

        private void NotificationManagerOnClickInviteStatusGet(object sender, DeskConnectionInformation deskConnectionInformation)
        {
            ((Window)sender).Close();
            if (deskConnectionInformation.Status == true)
            {
                if (!ConnectionHandler.IsConnected)
                {
                    deskConnectionInformation.IsRequestStatusUpdate = true;
                    deskConnectionInformation.Status = true;
                    DeskProfileManager.DeskProfilesDictionary[deskConnectionInformation.SenderDesk.DeskId + ""].RecentLoginTime = DateTime.Now;
                    //RecentSessionAdd(deskConnectionInformation.SenderDesk);
                    ConnectToServerCall(deskConnectionInformation, ReponseAndReqType.ReqResponse);
                }
            }
        }

        private void InviteWindowOnClickInviteButton(object sender, DeskConnectionInformation deskConnectionInformation)
        {
            ((Window)(sender)).Hide();
            Task.Run(() =>
            {
                deskConnectionInformation.InviteID = Guid.NewGuid().ToString();
                //ToDo: need to add invite
                //ServerNetworkManager.AddInviteReq(deskConnectionInformation);
                //ClientNetworkManager.InviteRequestSent(deskConnectionInformation);
            });
        }

        private void InviteButtonClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                OpenInviteWindow();
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                    new Action(OpenInviteWindow));
            }

        }
        private InviteWindow inviteWindow;
        private void OpenInviteWindow()
        {
            if (inviteWindow == null)
            {
                inviteWindow = new InviteWindow();
                inviteWindow.OnClickInviteButton += InviteWindowOnClickInviteButton;
            }
            inviteWindow.InviteDesk(null);
            inviteWindow.ShowDialog();
        }
        #endregion

        #region Connection

        private void DeskProfileOnclickConnect(object sender, Desk desk)
        {

            DeskConnectionInformation deskConnectionInformation = new DeskConnectionInformation() { AccessType = AccessType.FullAccess, ConnectionType = ConnectionType.Connect, AudioAccess = true, ClipboardAccess = true, KeyboardAccess = true, MouseAccess = true, SenderDesk = new DeskInfo(DeskProfileManager.UserDesk), ReceiverDesk = new DeskInfo(desk) };
            ConnectToServerCall(deskConnectionInformation, ReponseAndReqType.ConnectReq);
        }

        private void ConnectToServerCall(DeskConnectionInformation deskConnectionInformation, ReponseAndReqType reqType)
        {
            MainTabControl.SelectedItem = ConnectionSharePage;
            ConnectionGifTimer.Start();
            ConnectionManager.ConnectToDeskRequestSent(deskConnectionInformation);
        }

        private void ConnectionCancelButtonClick(object sender, RoutedEventArgs e)
        {

            Dispatcher.Invoke(() =>
            {
                if (MainTabControl.SelectedItem == ConnectionSharePage)
                {
                    MainTabControl.SelectedItem = HomePage;
                    ConnectionGifTimer?.Stop();
                    ConnectionHandler?.Close();
                }
            });
        }

        private void ConnectionDisConnectedToSeverInvoke(string message)
        {

            Dispatcher.Invoke(new Action(() =>
            {
                MainTabControl.SelectedItem = HomePage;
                SearchBoxControl.Text = "";
                SearchBoxControl.IsReadOnly = false;
                SearchBoxControl.IsConnected = false;
                SessionTabHeader.IsCloseButtonVisible = false;
                SessionTabHeader.Header = "New session";
                NewSessionCreateButton.Focus();
                connectedStausIcon.Visibility = Visibility.Hidden;
                screenSwitchControl.Visibility = Visibility.Hidden;
                FullScreenMode = false;
                if (message != null)
                    DeskMessageBox.ShowMessageBox(message, "Error", MessageBoxType.Ok, this);
            }));

        }

        private void ClientNetworkManagerConnectedToSeverInvoke(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
             {
                 MainTabControl.SelectedItem = ScreenSharePage;
                 //ToDo
                 //SearchBoxControl.Text = "" + ClientNetworkManager.deskConnectionInformation.ReceiverDesk.DisplayID;
                 SearchBoxControl.IsReadOnly = true;
                 SearchBoxControl.IsConnected = true;
                 connectedStausIcon.Visibility = Visibility.Visible;
                 ScreenImage.Focus();
                 SessionTabHeader.IsCloseButtonVisible = true;
                 //ToDo
                 //SessionTabHeader.Header = "" + ClientNetworkManager.deskConnectionInformation.ReceiverDesk.DisplayID;
             }));
        }

        private void ClientNetworkManagerConnectionEstabishFailInvoke(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (MainTabControl.SelectedItem == ConnectionSharePage)
                {
                    ConnectionHandler.Close();
                    MainTabControl.SelectedItem = HomePage;
                    connectedStausIcon.Visibility = Visibility.Hidden;
                    ConnectionGifTimer.Stop();
                    NotificationManager.CreateNotification("Connection Establish fail", NotificationType.Information);
                }
            }));

        }

        private void ConnectionManagerConnectRequestStatusInvoke(DeskConnectionInformation deskConnectionInformation)
        {

            if (deskConnectionInformation.Status)
            {
                //ToDo
                //ClientNetworkManager.DeskConnectionInformation = deskConnectionInformation;
                Task.Run(() =>
                {
                    DeskProfileManager.DeskProfilesDictionary[deskConnectionInformation.ReceiverDesk.DeskId + ""].RecentLoginTime = DateTime.Now;
                    try
                    {
                        ConnectToServerCall(deskConnectionInformation, ReponseAndReqType.ConnectReq);
                    }
                    catch (Exception e)
                    {
                        NotificationManager.CreateNotification(e.Message, NotificationType.Error);
                    }
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    MainTabControl.SelectedItem = HomePage;
                    ConnectionGifTimer.Stop();
                    NotificationManager.CreateNotification("(" + deskConnectionInformation.ReceiverDesk.DisplayID + ") " + deskConnectionInformation.ReceiverDesk.ProfileName + " Rejected Connection \n Request", NotificationType.Information);
                });
            }

        }

        private void RecentSessionAdd(Desk desk)
        {
            Dispatcher.BeginInvoke((Action)(() =>
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
            }));
        }

        private void ConnectAcceptWindowOnClickGetStatus(object sender, DeskConnectionInformation deskConnectionInformation)
        {
            ((Window)(sender)).Close();
            if (deskConnectionInformation.Status == true)
            {
                deskConnectionInformation.IsRequestStatusUpdate = true;
                deskConnectionInformation.Message = "Connection Success";
                deskConnectionInformation.AddDeskScreensInformations();

            }
            else
            {
                deskConnectionInformation.IsRequestStatusUpdate = true;
                deskConnectionInformation.Message = "Connection Request rejected";
            }
            // ToDo 
            //ServerNetworkManager.RequestUpdate(deskConnectionInformation);
        }

        private void ConnectionManagerConnectRequestReceivedInvoke(DeskConnectionInformation deskConnectionInformation)
        {
            //try
            //{
            //    deskConnectionInformation.ReceiverDesk.DesktopImage?.Freeze();
            //    deskConnectionInformation.ReceiverDesk.ProfileImage?.Freeze();
            //    deskConnectionInformation.SenderDesk.DesktopImage?.Freeze();
            //    deskConnectionInformation.SenderDesk.ProfileImage?.Freeze();
            //}
            //catch (Exception ex)
            //{
            //    LogManager.LogException(ex.ToString());
            //}
            //Dispatcher.Invoke(() =>
            //{
            //    ConnectAcceptWindow ConnectAcceptWindow = new ConnectAcceptWindow(deskConnectionInformation);
            //    ConnectAcceptWindow.OnClickGetStatus += ConnectAcceptWindowOnClickGetStatus;
            //    ConnectAcceptWindow.ShowDialog();
            //}
            //);
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
            ConnectionHandler.Close();

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

        private void TitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;
            if (e.ButtonState != MouseButtonState.Pressed)
                return;
            try
            {
                DragMove();
            }
            catch (InvalidOperationException)
            {
            }
            e.Handled = true;
        }

        #endregion

        #region Desk user
        private void UserNameTextBoxEdited(object sender, string text)
        {
            DeskProfileManager.UserDesk.ProfileName = text;
            DeskProfileManager.UpdateDeskProfiledata(DeskProfileManager.UserDesk);
        }

        private void ProfileImageEditButtonClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog
            {
                Title = "Select Image",
                Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Multiselect = false
            };
            var res = dialog.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dialog.FileName);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                DeskProfileManager.UserDesk.ProfileImage = bitmap;
            }
            DeskProfileManager.UpdateDeskProfiledata(DeskProfileManager.UserDesk);
        }

        private void PasswordTextBoxEdited(object sender, string newpassword)
        {
            DeskProfileManager.UserDesk.Password = LucidDesk.Manager.Security.SecurityManager.Encrypt(newpassword);
            DeskProfileManager.UpdateDeskProfiledata(DeskProfileManager.UserDesk);
        }
        #endregion

        private void RefreshButtonClick(object sender, RoutedEventArgs e)
        {
            ConnectionManager.Restart();
            SystemInformationManager.Refresh();
            DeskProfileManager.Refresh();
            SetUpCheck();
        }


    }
}
