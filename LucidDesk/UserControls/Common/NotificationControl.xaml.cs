using LucidDesk.Manager.Classes;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LucidDesk.UserControls.Common
{
    /// <summary>
    /// Interaction logic for NotificationControl.xaml
    /// </summary>
    public partial class NotificationControl : Window
    {
     
        private static int borderRadius;
        private string message;
        public int IncreaseWidthCount = 8;

        public Duration Duration = new Duration(TimeSpan.FromSeconds(8));

        public event EventHandler<DeskConnectionInformation> OnClickInviteStatusGet;
       
        public event EventHandler OnEnd;
       
        public DeskConnectionInformation DeskConnectionInformation;
        public DispatcherTimer DispatcherTimer = new DispatcherTimer();
        public string Message{
          set{
                message = value;
                MessageText.Text = message;
          }
          get{
                return message;
          }
        }
      
        


        public Brush ThemeBrush
        {
            get { return (Brush)GetValue(ThemeBrushProperty); }
            set { SetValue(ThemeBrushProperty, value); }
        }

        public static readonly DependencyProperty ThemeBrushProperty =
            DependencyProperty.Register("ThemeBrush", typeof(Brush), typeof(NotificationControl), new PropertyMetadata(Application.Current.Resources["MainColorBrush"]));


        private NotificationType notificationType;
        public NotificationType NotificationType
        {
            set
            {
                notificationType = value;
                if (notificationType == NotificationType.Error)
                {
                    ThemeBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5739"));
                    MainGrid.RowDefinitions[2].Height = new GridLength(0);
                    HeaderText.Text = "Error";
                }
                else if (notificationType == NotificationType.Information)
                {
                    MainGrid.RowDefinitions[2].Height = new GridLength(0);
                    HeaderText.Text = "Information";
                }
                else if (notificationType == NotificationType.Invite)
                {
                    HeaderText.Text = "Invite";
                }
                else
                {
                    MainGrid.RowDefinitions[2].Height = new GridLength(0);
                }
            }
            get
            {
                return notificationType;
            }
        }

        public NotificationControl(string message, NotificationType notificationType)
        {
            InitializeComponent();
            
            DataContext = this;
            Message = message;
            NotificationType = notificationType;
            Loaded += NotificationControlLoaded;
        }
        public NotificationControl( DeskConnectionInformation deskConnectionInformation)
        {
            InitializeComponent();
            DataContext = this;
            DeskConnectionInformation = deskConnectionInformation;
            MessageText.FontSize = 15;
            Duration = new Duration(TimeSpan.FromSeconds(40));
           Message = "would  you like to Connect " + DeskConnectionInformation.SenderDesk.ProfileName + "\n (" + DeskConnectionInformation.SenderDesk.Id + ")";
            NotificationType = NotificationType.Invite;
            Loaded += NotificationControlLoaded;
        }

        private void NotificationControlLoaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation() { From = 6, To = ActualWidth, Duration = this.Duration };
            doubleAnimation.Completed += IncreaseWidthCompleted;
            IncreaseLine.BeginAnimation(WidthProperty, doubleAnimation);
            //DispatcherTimer.Interval = new TimeSpan(0, 0,0,0,50);
            //DispatcherTimer.Tick += NotificatinTimeEndCheck;
            //DispatcherTimer.Start();
        }

        private void IncreaseWidthCompleted(object sender, EventArgs e)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation() {  To = 0.1, Duration = new Duration(TimeSpan.FromSeconds(2)) };
            doubleAnimation.Completed += OpacityCompleted;
            this.BeginAnimation(OpacityProperty, doubleAnimation);
          
        }

        private void OpacityCompleted(object sender, EventArgs e)
        {
            OnEnd?.Invoke(this, EventArgs.Empty);
        }

        private void NotificatinTimeEndCheck(object sender, EventArgs e)
        {
           
            if (Opacity <=0.1)
            {
                DispatcherTimer.Stop();
                OnEnd?.Invoke(this, EventArgs.Empty);
            }
            else if (IncreaseLine.ActualWidth <= ActualWidth)
            {
                IncreaseLine.Width += IncreaseWidthCount;
            }
            else if(IncreaseLine.ActualWidth> ActualWidth) {
                Opacity -= 0.1;
            }
        }

        public NotificationControl()
        {
            InitializeComponent();
        }
        public static int BorderRadius
        {
            get
            {
                return borderRadius;
            }
            set
            {
                borderRadius = value;
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            DispatcherTimer.Stop();
            OnEnd.Invoke(this, EventArgs.Empty);
        }

        private void RejectButtonClick(object sender, RoutedEventArgs e)
        {
            DeskConnectionInformation.InviteStatus = false;
            OnClickInviteStatusGet?.Invoke(this, DeskConnectionInformation);
            OnEnd?.Invoke(this, EventArgs.Empty);
        }

        private void AcceptButtonClick(object sender, RoutedEventArgs e)
        {
            DeskConnectionInformation.InviteStatus = true;
            OnClickInviteStatusGet?.Invoke(this, DeskConnectionInformation);
            OnEnd?.Invoke(this, EventArgs.Empty);
        }
        public void Invoke(){

          

            DoubleAnimation doubleAnimation = new DoubleAnimation() {From= Left + Width, To = Left, Duration = new Duration(TimeSpan.FromSeconds(0.2)) };
           Show();
            this.BeginAnimation(Window.LeftProperty, doubleAnimation);
        }
    }
}
