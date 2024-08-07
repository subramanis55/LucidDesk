using LucidDesk.FeaturesFuntionMangerClasses;
using LucidDesk.Manager;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LucidDesk.UserControls
{
    /// <summary>
    /// Interaction logic for DeskProfile.xaml
    /// </summary>
    public partial class DeskProfile : UserControl
    {
        public event EventHandler OnclickConnect;
        public event EventHandler<Desk> OnInviteConnect;
        private Desk desk;

        public Desk Desk
        {
            set
            {
                desk = value;
                try{
                    if (desk.ProfileImage == null)
                    {
                        DeskUserProfileImage.Image = ColorFeatures.CreateBitmapImageWithCharacter(90, 90, ColorFeatures.GetColorBasedOnFirstChar(DeskUserName = desk.ProfileName), desk.ProfileName[0], "Arial", 17, Colors.White);
                    }
                    else
                        DeskUserProfileImage.Image = desk.ProfileImage;
                    IsFavorite = desk.IsFavorite;
                    DeskUserName = desk.ProfileName;
                    PCName = desk.PcName;
                    DeskOSName = desk.OsName;
                    DeskId = "" + desk.Id;
                    DesktopWallPaper.Image = desk.DesktopImage;
                }
                 catch{

                 }
            }
            get
            {
                return desk;
            }
        }
        private bool isFavorite;
        public bool IsFavorite
        {
            set
            {
                isFavorite = value;
                if (!isFavorite)
                {
                    IsFavoriteIcon.Fill = Brushes.Transparent;
                }
                else
                {
                    IsFavoriteIcon.Fill = Brushes.Gold;
                }
            }
            get
            {
                return isFavorite;
            }
        }
        public string DeskUserName
        {
            get { return (string)GetValue(DeskUserNameProperty); }
            set { SetValue(DeskUserNameProperty, value); }
        }

        public static readonly DependencyProperty DeskUserNameProperty =
            DependencyProperty.Register("DeskUserName", typeof(string), typeof(DeskProfile), new PropertyMetadata("Name"));

        public string PCName
        {
            get { return (string)GetValue(PCNameProperty); }
            set { SetValue(PCNameProperty, value); }
        }

        public static readonly DependencyProperty PCNameProperty =
            DependencyProperty.Register("PCName", typeof(string), typeof(DeskProfile), new PropertyMetadata("PcName"));



        public string DeskOSName
        {
            get { return (string)GetValue(DeskOSNameProperty); }
            set { SetValue(DeskOSNameProperty, value); }
        }

        public static readonly DependencyProperty DeskOSNameProperty =
            DependencyProperty.Register("DeskOSName", typeof(string), typeof(DeskProfile), new PropertyMetadata("Windows"));



        public string DeskId
        {
            get { return (string)GetValue(DeskIdProperty); }
            set { SetValue(DeskIdProperty, value); }
        }

        public static readonly DependencyProperty DeskIdProperty =
            DependencyProperty.Register("DeskId", typeof(string), typeof(DeskProfile), new PropertyMetadata(""));



        public Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(DeskProfile), new PropertyMetadata(new SolidColorBrush(Colors.DodgerBlue)));



        public DeskProfile()
        {
            InitializeComponent();

            DataContext = this;
        }



        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void IsFavoriteIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void IsFavoriteIconMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsFavorite = !IsFavorite;
            Desk.IsFavorite = IsFavorite;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ConnectClick(object sender, RoutedEventArgs e)
        {
            OnclickConnect?.Invoke(this, EventArgs.Empty);
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void ControlMouseEnter(object sender, MouseEventArgs e)
        {
            Opacity = 1;
        }

        private void ControlMouseLeave(object sender, MouseEventArgs e)
        {
            Opacity = 0.9;
        }

        private void InviteClick(object sender, RoutedEventArgs e)
        {
            OnInviteConnect?.Invoke(this,Desk);
        }
    }
}
