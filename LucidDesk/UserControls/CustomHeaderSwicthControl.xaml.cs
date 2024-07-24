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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LucidDesk.UserControls
{
    /// <summary>
    /// Interaction logic for CustomHeaderSwicthControl.xaml
    /// </summary>
    public partial class CustomHeaderSwicthControl : UserControl
    {

        public event EventHandler OnclickRecentSessionsButton;
        public event EventHandler OnclickFavoritesButton;
        public event EventHandler OnclickDiscoverdButton;

        List<CheckBox> ButtonsList;
        public CheckBox PrevButton;

        public CustomHeaderSwicthControl()
        {
            InitializeComponent();
            ButtonsList = new List<CheckBox>() {RecentSessionsButton,FavoritesButton,DiscoverdButton};
            RecentSessionsButton.Click += RecentSessionsButtonClick;
            FavoritesButton.Click += FavoritesButtonClick;
            DiscoverdButton.Click += DiscoverdButtonClick;
            PrevButton = ButtonsList[0];
            RecentSessionsButton.IsChecked = true;
           Loaded += CustomHeaderSwicthControlLoaded;
        }

        private void DiscoverdButtonClick(object sender, RoutedEventArgs e)
        {
            OnclickDiscoverdButton?.Invoke(sender, e);
        }

        private void FavoritesButtonClick(object sender, RoutedEventArgs e)
        {
            OnclickFavoritesButton?.Invoke(sender, e);
        }

        private void RecentSessionsButtonClick(object sender, RoutedEventArgs e)
        {
            OnclickRecentSessionsButton?.Invoke(sender, e);
        }

        private void CustomHeaderSwicthControlLoaded(object sender, RoutedEventArgs e)
        {
            SelectedNotifyBorder.Margin = new Thickness(((ButtonsList[0].ActualWidth / 2) - SelectedNotifyBorder.ActualWidth / 2),0,0,0);

        }

     
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
           
            int indexCurrent = ButtonsList.IndexOf((CheckBox)(sender));
            int indexPrev = ButtonsList.IndexOf((CheckBox)(PrevButton));
            CheckBox currentButton = (CheckBox)sender;
            currentButton.IsChecked = true;
            double marginPoint = 0;

                for (int i=0;i< indexCurrent;i++){
                   marginPoint+= ButtonsList[i].ActualWidth;
                }
            marginPoint += (ButtonsList[indexCurrent].ActualWidth / 2) - SelectedNotifyBorder.ActualWidth / 2;

            ThicknessAnimation thicknessAnimation = new ThicknessAnimation(new Thickness( marginPoint,0,0,0),new Duration(TimeSpan.FromSeconds(0.3)));

            SelectedNotifyBorder.BeginAnimation(MarginProperty, thicknessAnimation);
            PrevButton.IsChecked = false;
            PrevButton = currentButton;
        }
    }
}
