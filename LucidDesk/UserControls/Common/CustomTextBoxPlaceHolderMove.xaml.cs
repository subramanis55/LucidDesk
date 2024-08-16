using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Brush = System.Windows.Media.Brush;
using MediaColor = System.Windows.Media.Color;
using DrawingColor = System.Drawing.Color;

namespace LucidDesk.UserControls.Common
{
    
    public partial class AnimatedTextBox : UserControl
    {
        public AnimatedTextBox()
        {
            InitializeComponent();
            InitializeControl();
            placeHolder.Foreground = System.Windows.Media.Brushes.Gray;
            roundedRectangle.Stroke = System.Windows.Media.Brushes.Gray; 
        }
        private bool isPasswordType;
        public double AnimatedTextBoxBorderRadius
        {
            get => roundedRectangle.RadiusX;
            set => roundedRectangle.RadiusX = roundedRectangle.RadiusY = value;

        }

        public char AnimatedTextBoxPassChar
        {
            get => animatedPassChar;
            set
            {
                if (Convert.ToString(value) != "")
                {
                    animatedPassChar = value;
                }
            }
        }

        public string AnimatedTextBoxPlaceHolder
        {
            get => Convert.ToString(placeHolder.Content);
            set => placeHolder.Content = value;
        }

        public double AnimatedTextBoxFontSize
        {
            get => mainTextBox.FontSize;
            set
            {
                mainTextBox.FontSize = placeHolder.FontSize = viewPassLabel.FontSize = value;
                viewPassCanvas.Margin = new Thickness(0, 0, value * 2, value * 2);
            }
        }

        public Brush AnimatedTextBoxBackground
        {
            get => mainTextBox.Background;
            set => mainTextBox.Background = placeHolder.Background = value;
        }

        public Brush AnimatedTextBoxForeground
        {
            get => mainTextBox.Foreground;
            set
            {
                if (value != null)
                {
                    mainTextBox.Foreground =   viewPassLabel.Foreground = value;
                    viewPassLabelLine.Stroke = mainTextBox.CaretBrush = value;
                }
            }
        }

        public bool IsPasswordType
        {
            get
            {
                return isPasswordType;
            }
            set{

                isPasswordType = value;
                 if(isPasswordType){
                    mainTextBox.Padding = new Thickness(8, 0, 25, 0);
                 }
                 else{
                    mainTextBox.Padding = new Thickness(5, 0, 0, 0);
                }
            }
        }

        private char animatedPassChar = '●';
        private string password;
        private bool isPasswordViewOn = false;

        private void MainTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (mainTextBox.Text == "")
            {
                double top = -(ActualHeight);
                ThicknessAnimation animation1 = new ThicknessAnimation
                {
                    From = placeHolder.Margin,
                    To = new Thickness(20, top, 0, 0),
                    Duration = new Duration(TimeSpan.FromMilliseconds(300))
                };

                DoubleAnimation animation2 = new DoubleAnimation
                {
                    From = placeHolder.FontSize,
                    To = 10,
                    Duration = new Duration(TimeSpan.FromMilliseconds(300))
                };

                placeHolder.BeginAnimation(MarginProperty, animation1);
                placeHolder.BeginAnimation(FontSizeProperty, animation2);
            }
            roundedRectangle.Stroke = System.Windows.Media.Brushes.DodgerBlue;
            placeHolder.Foreground = System.Windows.Media.Brushes.DodgerBlue;
        }

        private void MainTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (mainTextBox.Text == "")
            {
                double top = (ActualHeight);
                ThicknessAnimation animation1 = new ThicknessAnimation
                {
                    From = placeHolder.Margin,
                    To = new Thickness(20, 0, 0, 0),
                    Duration = new Duration(TimeSpan.FromMilliseconds(300))
                };

                DoubleAnimation animation2 = new DoubleAnimation
                {
                    From = placeHolder.FontSize,
                    To = AnimatedTextBoxFontSize,
                    Duration = new Duration(TimeSpan.FromMilliseconds(300))
                };

                placeHolder.BeginAnimation(MarginProperty, animation1);
                placeHolder.BeginAnimation(FontSizeProperty, animation2);
            }
            if(mainTextBox.Text=="") {
                roundedRectangle.Stroke = System.Windows.Media.Brushes.Gray;
                placeHolder.Foreground = System.Windows.Media.Brushes.Gray;
            }
          
        }

        private void PlaceHolder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mainTextBox.Focus();
        }

        private void MainTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (IsPasswordType)
            {
                if (!isPasswordViewOn)
                {
                    password += e.Text;
                    mainTextBox.Text += animatedPassChar;
                    e.Handled = true;
                }
                else
                {
                    password += e.Text;
                    mainTextBox.Text = password;
                    // mainTextBox.CaretIndex = mainTextBox.Text.Length;
                    e.Handled = true;
                }

                if (mainTextBox.CaretIndex == 0)
                {
                    mainTextBox.CaretIndex = mainTextBox.Text.Length;
                }
            }


            if (IsPasswordType && password.Length > 0)
                viewPassLabel.Visibility = Visibility.Visible;
            else
                viewPassLabel.Visibility = Visibility.Hidden;
        }

        private void MainTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IsPasswordType && !isPasswordViewOn && e.Key == Key.Back && mainTextBox.Text.Length > 0)
            {
                password = password.Substring(0, password.Length - 1);
                mainTextBox.Text = mainTextBox.Text.Substring(0, mainTextBox.Text.Length - 1);
                mainTextBox.CaretIndex = mainTextBox.Text.Length;
                e.Handled = true;
            }

            if (IsPasswordType && mainTextBox.CaretIndex == 0)
            {
                mainTextBox.CaretIndex = mainTextBox.Text.Length;
            }
        }


        private void InitializeControl()
        {
            viewPassLabel.Visibility = IsPasswordType ? Visibility.Visible : Visibility.Hidden;
            viewPassLabelLine.Visibility = Visibility.Hidden;
        }

        private void ViewPassLabelClicked(object sender, MouseButtonEventArgs e)
        {
            isPasswordViewOn = !isPasswordViewOn;
            if (isPasswordViewOn)
            {
                viewPassLabelLine.Visibility = Visibility.Visible;
                mainTextBox.Text = password;
            }
            else
            {
                viewPassLabelLine.Visibility = Visibility.Hidden;
                mainTextBox.Text = new string(animatedPassChar, mainTextBox.Text.Length);
            }
            mainTextBox.CaretIndex = mainTextBox.Text.Length;
        }

        private void mainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mainTextBox.Text.Length == 0)
                password = "";
        }
    }
}
