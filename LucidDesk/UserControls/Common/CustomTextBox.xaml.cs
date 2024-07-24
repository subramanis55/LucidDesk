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
    /// Interaction logic for CustomTextBox.xaml
    /// </summary>
    public partial class CustomTextBox : UserControl
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder",typeof(string),typeof(CustomTextBox),new PropertyMetadata(""));
        public static readonly DependencyProperty PlaceholderColorProperty=DependencyProperty.Register("PlaceholderColor", typeof(Brush), typeof(CustomTextBox), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDD"))));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CustomTextBox));
        public static readonly DependencyProperty TextPaddingProperty = DependencyProperty.Register("TextPadding", typeof(Thickness), typeof(CustomTextBox), new PropertyMetadata(new Thickness(10, 5, 10, 5)));
        public static readonly DependencyProperty TextboxBorderThicknessProperty =
         DependencyProperty.Register("TextboxBorderThickness", typeof(Thickness), typeof(CustomTextBox), new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty TextboxBorderBrushProperty =
         DependencyProperty.Register("TextboxBorderBrush", typeof(Brush), typeof(CustomTextBox), new PropertyMetadata(new SolidColorBrush(Colors.White)));

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
            set { SetValue(TextProperty, value); }
        }

        public string Placeholder{
             set{
                SetValue( PlaceholderProperty, value);
             }
             get{
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
        public CustomTextBox()
        {
            InitializeComponent();

            PlaceholderLabel.MouseDown += CustomTextBoxMouseDown;
            Textbox.GotFocus += TextboxGotFocus;
            Textbox.LostFocus += TextboxLostFocus;
            DataContext = this;
        }

        private void TextboxLostFocus(object sender, RoutedEventArgs e)
        {
            TextboxBorderThickness = new Thickness(0, 0, 0, 2);
            if (string.IsNullOrEmpty(Textbox.Text)){
                PlaceholderLabel.Visibility = Visibility.Visible;
            }
           else{
                PlaceholderLabel.Visibility = Visibility.Hidden;
            }
        }

        private void TextboxGotFocus(object sender, RoutedEventArgs e)
        {
            TextboxBorderThickness = new Thickness(0, 0, 0, 3);
                PlaceholderLabel.Visibility = Visibility.Hidden;
        }

        private void CustomTextBoxMouseDown(object sender, MouseButtonEventArgs e)
        {
            Textbox.Focus();
            e.Handled = true;
        }

    }
}
