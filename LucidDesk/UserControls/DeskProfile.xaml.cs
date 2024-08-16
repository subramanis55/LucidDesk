using LucidDesk.FeaturesFuntionMangerClasses;
using LucidDesk.Manager;
using LucidDesk.Manager.Database;
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
    public partial class DeskProfile : UserControl, IDisposable
    {
        public event EventHandler<Desk> OnClickConnect;
        public event EventHandler<Desk> OnInviteConnect;
        public  event EventHandler OnClickIsFavorite;
        private Desk desk;
        private bool isFavorite;
        public Desk Desk
        {
            set
            {
                MainContainer.DataContext = value;
                desk = value;
                try
                {
                    if (desk.ProfileImage == null)
                    {
                        DeskUserProfileImage.Image = ColorFeatures.CreateBitmapImageWithCharacter(90, 90, ColorFeatures.GetColorBasedOnFirstChar(desk.ProfileName), desk.ProfileName[0], "Arial", 17, Colors.White);
                    }
                    else
                        DeskUserProfileImage.Image = desk.ProfileImage;
                    DeskId = "" + desk.Id;
                    DesktopWallPaper.Image = desk.DesktopImage;
                    desk.PropertyChanged += DeskPropertyChanged;
                    Desk.OnClickDeleted += DeskOnClickDeleted;
                }
                catch
                {
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
        public bool IsFavorite
        {
            get { return (bool)GetValue(IsFavoriteProperty); }
            set { SetValue(IsFavoriteProperty, value);
               
            }
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

        public static readonly DependencyProperty IsFavoriteProperty =
     DependencyProperty.Register(
         nameof(IsFavorite),
         typeof(bool),
         typeof(DeskProfile), // Replace with the actual class name
         new PropertyMetadata(false, IsFavoriteChanged));

        private static void IsFavoriteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           
           
        }

        public DeskProfile()
        {
            InitializeComponent();
            DataContext = this;
            MainContainer.DataContext = Desk;
            //IsFavoriteCheckBox.Unchecked += IsFavoriteCheckBoxIsChecked;
            //IsFavoriteCheckBox.Checked += IsFavoriteCheckBoxIsChecked;
            //Binding binding = new Binding("IsFavorite");
            //binding.Source = Desk;
            //binding.Mode = BindingMode.TwoWay;
            //this.SetBinding(IsFavoriteProperty, binding);
            this.Loaded += DeskProfileLoaded;
          
        }

        private void DeskProfileLoaded(object sender, RoutedEventArgs e)
        {
          
        }

        private void DeskPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           if(e.PropertyName== "IsFavorite"){
                OnClickIsFavorite?.Invoke(this, e);
            }
        }

        public DeskProfile(Desk desk)
        {
            InitializeComponent();
            Desk = desk;
            DataContext = this;
            Desk.OnClickDeleted += DeskOnClickDeleted;
            //IsFavoriteCheckBox.Unchecked += IsFavoriteCheckBoxIsChecked;
            //IsFavoriteCheckBox.Checked += IsFavoriteCheckBoxIsChecked;
            //Binding binding = new Binding("IsFavorite");
            //binding.Source = Desk;
            //binding.Mode = BindingMode.OneWay;
            //this.SetBinding(IsFavoriteProperty, binding);

        }

        private void DeskOnClickDeleted(object sender, EventArgs e)
        {
            this.Dispose();
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
            //OnClickIsFavorite?.Invoke(this, EventArgs.Empty);
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
            OnInviteConnect?.Invoke(this, Desk);
        }

        public void Dispose()
        {
            desk.PropertyChanged -= DeskPropertyChanged;

            WrapPanel wrapPanel = this.Parent as WrapPanel;
            if(wrapPanel!=null)
            wrapPanel.Children.Remove(this);
        }

       

        private void RemoveButtonClick(object sender, RoutedEventArgs e)
        {
           MessageBox removeConfirmDialog=new MessageBox();
            removeConfirmDialog.ShowMessageBox("Do want to remove this desk?", "Conformation", MessageBoxType.YesOrNo);
            if(removeConfirmDialog.DialogResult==System.Windows.Forms.DialogResult.Yes) {
               if(DeskProfileManager.DeleteProfile(desk.Id)){
                    desk.Dispose();
               }
             
            }
        }

        private void IsFavoriteClick(object sender, RoutedEventArgs e)
        {
            Desk.IsFavorite = !Desk.IsFavorite;
        }

        private void ConnectWithPasswordClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
