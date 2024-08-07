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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LucidDesk.UserControls.Common
{
    /// <summary>
    /// Interaction logic for NotificationControl.xaml
    /// </summary>
    public partial class NotificationControl : Window
    {
        private static int borderRadius;
        public Brush themeBrush = Application.Current.Resources["MainColorBrush"] as Brush;
        public DeskConnectionInformation DeskConnectionInformation;
        public Brush ThemeBrush
        {
            set
            {
                themeBrush = value;
            }
            get
            {
                return themeBrush;
            }
        }
        private NotificationType notificationType;
        public NotificationType NotificationType
        {
            set
            {
                notificationType = value;
                if (notificationType == NotificationType.Error)
                {
                    ThemeBrush = new SolidColorBrush(Color.FromArgb(100, 255, 87, 57));
                    MainGrid.RowDefinitions[2].Height = new GridLength( 0);
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
        public event EventHandler OnEnd;
        public NotificationControl(string message, NotificationType notificationType)
        {
            InitializeComponent();
            NotificationType = notificationType;
        }
        public NotificationControl(string message, DeskConnectionInformation deskConnectionInformation)
        {
            InitializeComponent();
            DeskConnectionInformation = deskConnectionInformation;
            
            NotificationType = NotificationType.Invite;
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
    }
}
