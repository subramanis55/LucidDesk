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
    public partial class DeskProfile : UserControl,IDisposable
    {
        public event EventHandler<Desk> OnClickConnect;
        public event EventHandler<Desk> OnInviteConnect;
        public event EventHandler OnClickIsFavorite;
        private Desk desk;

        public Desk Desk
        {
            set
            {
                MainContainer.DataContext = value;
                desk = value;
                try{
                    if (desk.ProfileImage == null)
                    {
                        DeskUserProfileImage.Image = ColorFeatures.CreateBitmapImageWithCharacter(90, 90, ColorFeatures.GetColorBasedOnFirstChar( desk.ProfileName), desk.ProfileName[0], "Arial", 17, Colors.White);
                    }
                    else
                        DeskUserProfileImage.Image = desk.ProfileImage;
                   
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
            MainContainer. DataContext = Desk;
        }
        public DeskProfile(Desk desk)
        {
            InitializeComponent();
            Desk = desk;


        }



        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            MainContainer.ContextMenu.IsOpen = true;
        }

        private void IsFavoriteIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void IsFavoriteIconMouseDown(object sender, MouseButtonEventArgs e)
        {
            Desk.IsFavorite = !Desk.IsFavorite;

            OnClickIsFavorite?.Invoke(this, EventArgs.Empty);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ConnectClick(object sender, RoutedEventArgs e)
        {
            OnClickConnect?.Invoke(this, Desk);
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

        public void Dispose()
        {
           
        }

        private void IsFavoriteClick(object sender, RoutedEventArgs e)
        {
            Desk.IsFavorite = !Desk.IsFavorite;

            OnClickIsFavorite?.Invoke(this, EventArgs.Empty);
        }

        private void RemoveButtonClick(object sender, RoutedEventArgs e)
        {
          MessageBox.Show("Do want to remove this desk?", "Conformation", MessageBoxButton.YesNo);
        }
    }
}
