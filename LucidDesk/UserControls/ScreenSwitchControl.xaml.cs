using LucidDesK.DS.DataSchema.Screens;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace LucidDesk.UserControls
{
    /// <summary>
    /// Interaction logic for ScreenSwitchControl.xaml
    /// </summary>
    public partial class ScreenSwitchControl : UserControl
    {
        public event EventHandler<LucidDesK.DS.DataSchema.Screens.Screen> ScreenSelectionChanged;

        public ScreenInformation Screeninfo;

        private Button selectedButton;
        private Button SelectedButton
        {
            set
            {
                if (selectedButton != null)
                    selectedButton.Style = (Style)Application.Current.Resources["NormalButton"];
                selectedButton = value;
                selectedButton.Style = (Style)Application.Current.Resources["MainButton"];
            }
            get
            {
                return selectedButton;
            }
        }
        public ScreenSwitchControl()
        {
            InitializeComponent();
        }
        public void UpdateScreenInformation(ScreenInformation screeninfo)
        {
            Screeninfo = screeninfo;
            if (screeninfo.Count <= 1)
            {
                Visibility = Visibility.Collapsed;
                return;
            }
            Visibility = Visibility.Visible;
            ScreenSelectPanel.Children.Clear();
            ScreenSelectPanel.Children.Add(ScreenButtonCreate("ALL"));
            foreach (var screen in screeninfo.Screens)
            {
                ScreenSelectPanel.Children.Add(ScreenButtonCreate("" + ScreenSelectPanel.Children.Count));
            }
            SelectedButton = (Button)ScreenSelectPanel.Children[0];
        }
        private Button ScreenButtonCreate(string name)
        {
            Button screenBtn = new Button()
            {
                Content = name,
                Height = 35,
                Width = 40,
                Style = (Style)Application.Current.Resources["NormalButton"],
                Margin = new Thickness(4),
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Gainsboro
            };
            screenBtn.Click += ScreenBtnClick;
            return screenBtn;
        }

        private void ScreenBtnClick(object sender, RoutedEventArgs e)
        {
            SelectedButton = (Button)sender;
            if (ScreenSelectPanel.Children.IndexOf((Button)sender) == 0)
                ScreenSelectionChanged?.Invoke(sender, null);
            else
                ScreenSelectionChanged?.Invoke(sender, Screeninfo.Screens[ScreenSelectPanel.Children.IndexOf((Button)sender) - 1]);
        }
    }
}
