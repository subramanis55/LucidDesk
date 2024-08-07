using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for SearchBoxControl.xaml
    /// </summary>
    public partial class SearchBoxControl : UserControl
    {
        public event EventHandler OnClickFullScreen;
        public event EventHandler OnClickScreenStrech;
        public event EventHandler OnClickScreenZoom;
        public event EventHandler OnClickScreenNormal;
        private bool isConnected = true;
        public bool IsConnected
        {
            set
            {
                isConnected = value;
                if (isConnected)
                {
                    SearchBoxMainGrid.ColumnDefinitions[2].Width = new GridLength(120);
                }
                else
                {
                    SearchBoxMainGrid.ColumnDefinitions[2].Width = new GridLength(0);
                }
            }
            get
            {
                return isConnected;
            }
        }
        public ContextMenu ScreenButtonMenu;

        private bool isLabelVisible;
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(SearchBoxControl), new PropertyMetadata(""));
        public static readonly DependencyProperty PlaceholderColorProperty = DependencyProperty.Register("PlaceholderColor", typeof(Brush), typeof(SearchBoxControl), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDD"))));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SearchBoxControl));
        public static readonly DependencyProperty TextPaddingProperty = DependencyProperty.Register("TextPadding", typeof(Thickness), typeof(SearchBoxControl), new PropertyMetadata(new Thickness(15, 8, 10, 5)));
        public static readonly DependencyProperty TextboxBorderThicknessProperty =
         DependencyProperty.Register("TextboxBorderThickness", typeof(Thickness), typeof(SearchBoxControl), new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty TextboxBorderBrushProperty =
         DependencyProperty.Register("TextboxBorderBrush", typeof(Brush), typeof(SearchBoxControl), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush TextboxBorderBrush
        {
            get { return (Brush)GetValue(TextboxBorderBrushProperty); }
            set { SetValue(TextboxBorderBrushProperty, value); }
        }


        public Thickness TextboxBorderThickness
        {
            get { return (Thickness)GetValue(TextboxBorderThicknessProperty); }
            set { SetValue(TextboxBorderThicknessProperty, value); }
        }



        public Thickness TextPadding
        {
            get { return (Thickness)GetValue(TextPaddingProperty); }
            set { SetValue(TextPaddingProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set
            {
                SetValue(TextProperty, value);
                Textbox.Focus();
            }
        }

        public string Placeholder
        {
            set
            {
                SetValue(PlaceholderProperty, value);
            }
            get
            {
                return (string)GetValue(PlaceholderProperty);
            }
        }
        public Brush PlaceholderColor
        {
            set
            {
                SetValue(PlaceholderColorProperty, value);
            }
            get
            {
                return (Brush)GetValue(PlaceholderColorProperty);
            }
        }
        public bool IsLabelVisible
        {
            set
            {
                isLabelVisible = value;
                if (isLabelVisible)
                {
                    Panel.SetZIndex(IsLabel, 1);
                    Textbox.Visibility = Visibility.Hidden;
                    PlaceholderLabel.Visibility = Visibility.Hidden;
                }
                else
                {
                    Panel.SetZIndex(IsLabel, -1);
                    Textbox.Visibility = Visibility.Visible;
                    PlaceholderLabel.Visibility = Visibility.Visible;
                }
            }
            get
            {
                return isLabelVisible;
            }
        }
        public bool IsReadOnly
        {
            set
            {
                Textbox.IsReadOnly = value;
                IsLabelVisible = Textbox.IsReadOnly;
            }
            get
            {
                return Textbox.IsReadOnly;
            }
        }
        public SearchBoxControl()
        {
            InitializeComponent();
           
          Loaded += SearchBoxControlLoaded;
            PlaceholderLabel.MouseDown += CustomTextBoxMouseDown;
            Textbox.GotFocus += TextboxGotFocus;
            Textbox.LostFocus += TextboxLostFocus;
            DataContext = this;
            ScreenButtonMenu = Resources["ScreenButtonContext"] as ContextMenu;
        }

        private void SearchBoxControlLoaded(object sender, RoutedEventArgs e)
        {
            SearchBoxMainGrid.ColumnDefinitions[2].Width = new GridLength(0);
        }

        private void TextboxLostFocus(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(Textbox.Text))
            {
                PlaceholderLabel.Visibility = Visibility.Visible;
            }
            else
            {
                PlaceholderLabel.Visibility = Visibility.Hidden;
            }
        }

        private void TextboxGotFocus(object sender, RoutedEventArgs e)
        {

            PlaceholderLabel.Visibility = Visibility.Hidden;
        }

        private void CustomTextBoxMouseDown(object sender, MouseButtonEventArgs e)
        {
            Textbox.Focus();
            e.Handled = true;
        }

      
        private void ScreenButtonClick(object sender, RoutedEventArgs e)
        {
            ScreenButtonContextShow();
        }

        private void ScreenButtonContextShow()
        {
    

            ScreenButtonMenu.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Size contextMenuSize = ScreenButtonMenu.DesiredSize;
            double x = ScreenButton.ActualWidth/2 - contextMenuSize.Width / 2;
            ScreenButtonMenu.PlacementTarget = ScreenButton;
            double y = ScreenButton.ActualHeight;
            ScreenButtonMenu.Placement = PlacementMode.Relative;
      
            ScreenButtonMenu.HorizontalOffset = x;
            ScreenButtonMenu.VerticalOffset = y;
            ScreenButtonMenu.IsOpen = true;
        }

      

        private void ScreenButtonMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ScreenButtonContextShow();
        }
        private void FullScreenClick(object sender, RoutedEventArgs e)
        {
            OnClickFullScreen?.Invoke(this, EventArgs.Empty);
        }

        private void ScreenStrechClick(object sender, RoutedEventArgs e)
        {
            OnClickScreenStrech?.Invoke(this, EventArgs.Empty);
        }

        private void ScreenZoomClick(object sender, RoutedEventArgs e)
        {
            OnClickScreenZoom?.Invoke(this, EventArgs.Empty);
        }

        private void ScreenNormalClick(object sender, RoutedEventArgs e)
        {
            OnClickScreenNormal?.Invoke(this, EventArgs.Empty);
        }
    }
}
