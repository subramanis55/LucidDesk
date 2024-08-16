using LucidDesk.Manager.Files;
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

namespace LucidDesk.UserControls.Common
{
    /// <summary>
    /// Interaction logic for ProfileImageSelect.xaml
    /// </summary>
    public partial class ProfileImageSelect : UserControl
    {
        private BitmapImage profileImage;
        private string profileImageFilePath;
        public Brush BorderColor
        {
            set
            {
                ProfileImage.BorderBrush = value;
            }
            get
            {
                return ProfileImage.BorderBrush;
            }
        }
        public BitmapImage Profile
        {
            set
            {
                profileImage = value;
                ProfileImage.Image = profileImage;
            }
            get
            {
                return profileImage;
            }
        }
        public string ProfileFilePath
        {
            set
            {
                try
                {
                    profileImageFilePath = value;
                    Profile = new BitmapImage(new Uri(profileImageFilePath));
                }
                catch
                {

                }
            }
            get
            {
                return profileImageFilePath;
            }
        }

        public double WhiteBackgroundCorner
        {
            get { return (double)GetValue(WhiteBackgroundCornerProperty); }
            set { SetValue(WhiteBackgroundCornerProperty, value); }
        }


        public static readonly DependencyProperty WhiteBackgroundCornerProperty =
            DependencyProperty.Register("WhiteBackgroundCorner", typeof(double), typeof(ProfileImageSelect), new PropertyMetadata());

        public double WhiteBackgroundSize
        {
            get { return (double)GetValue(WhiteBackgroundSizeProperty); }
            set { SetValue(WhiteBackgroundSizeProperty, value); }
        }


        public static readonly DependencyProperty WhiteBackgroundSizeProperty =
            DependencyProperty.Register("WhiteBackgroundSize", typeof(double), typeof(ProfileImageSelect), new PropertyMetadata());



        public double OpacityBackground
        {
            get { return (double)GetValue(OpacityBackgroundProperty); }
            set { SetValue(OpacityBackgroundProperty, value); }
        }

        public static readonly DependencyProperty OpacityBackgroundProperty =
            DependencyProperty.Register("OpacityBackground", typeof(double), typeof(ProfileImageSelect), new PropertyMetadata());

        public System.Windows.Media.Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(ProfileImageSelect), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public ProfileImageSelect()
        {
            InitializeComponent();
            DataContext = this;
            OpacityBackground = 1;
            WhiteBackgroundSize = Height;
            BackgroundColor = new SolidColorBrush(Colors.White);
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {

        }
        //private void RoundedImage_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    AddImageToUriList(Properties.Resources.edit__1_, "edit_1_");
        //}

        //private BitmapImage AddImageToUriList(byte[] imageResource, string fileName)
        //{
        //    string tempFilePath = Path.Combine(Path.GetTempPath(), fileName);
        //    File.WriteAllBytes(tempFilePath, imageResource);

        //    // Create a new BitmapImage and set its UriSource to the temporary file path
        //    BitmapImage bitmapImage = new BitmapImage();
        //    bitmapImage.BeginInit();
        //    bitmapImage.UriSource = new Uri(tempFilePath, UriKind.Absolute);
        //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // Load the image immediately
        //    bitmapImage.EndInit();

        //    uris.Add(new Uri(tempFilePath, UriKind.Absolute));

        //    return bitmapImage;
        //}

        //BitmapImage BitmapImage1 = new BitmapImage(new Uri("Resources/edit(4).png"));
        //    BitmapImage BitmapImage2 = new BitmapImage(new Uri("C:/Users/Lucid/OneDrive/Desktop/WPFPractice/WPFPractice/Resources/user-20.png"));
        private void ProfileMouseLeave(object sender, MouseEventArgs e)
        {
            ProfileImage.Image = profileImage;
           
            BackgroundColor = new SolidColorBrush(Colors.White);
            OpacityBackground = 1;
        }

        private void ProfileMouseEnter(object sender, MouseEventArgs e)
        {
            ProfileImage.Image = FileManager.ConvertBitmapToBitmapImage(Properties.Resources.edit__3_);
            
            BackgroundColor = new SolidColorBrush(Colors.LightGray);
            OpacityBackground = 0.5;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void GridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            WhiteBackgroundCorner = Height;
            WhiteBackgroundSize = Height;
        }

        private void BackgroundMouseDown(object sender, MouseButtonEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";
                dialog.Title = "Select a Image";
                dialog.ShowDialog();
                ProfileFilePath = dialog.FileName;
            }
        }
    }
}

