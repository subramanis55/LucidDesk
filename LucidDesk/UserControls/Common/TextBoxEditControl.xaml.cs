using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LucidDesk.UserControls.Common
{
    /// <summary>
    /// Interaction logic for TextBoxEditControl.xaml
    /// </summary>
    public partial class TextBoxEditControl : UserControl
    {
        public event EventHandler<string> DoneButtonClicked;
        private bool ispassword;
        public bool IsPassword
        {
            get
            {
                return ispassword;
            }
            set
            {
                ispassword = value;
                textBox.IsPasswordType = ispassword;
            }
        }

        private string originalText;
        public string TextBoxText
        {
            get
            {
                if (!ispassword)
                    return textBox.Text;
                else
                    return textBox.Password;
            }
            set
            {
                if (!ispassword)
                    textBox.Text = value;
                else
                    textBox.Password = value;
                originalText = value;
            }
        }



        public bool IsEditable
        {
            get
            {
                return textBox.IsEnabled;
            }
            set
            {

                if (value)
                {
                    textBox.IsEnabled = true;
                    editButtonIcon.Data = Application.Current.Resources["Tick"] as Geometry;
                }
                else
                {
                    textBox.IsEnabled = false;
                    editButtonIcon.Data = Application.Current.Resources["Edit"] as Geometry;
                }
            }
        }



        public int MyProperty
        {
            get { return (int)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("MyProperty", typeof(int), typeof(TextBoxEditControl), new PropertyMetadata(0));



        public static readonly DependencyProperty IsPasswordPropertyProperty =
            DependencyProperty.Register("IsPassword", typeof(bool), typeof(TextBoxEditControl), new PropertyMetadata(false));


        public TextBoxEditControl()
        {
            InitializeComponent();

        }


        private void EditButtonClick(object sender, RoutedEventArgs e)
        {

            if (IsEditable)
            {
                originalText = TextBoxText;
                DoneButtonClicked?.Invoke(this, originalText);
                if (IsEditable != false)
                    IsEditable = false;
            }
            else
            {
                if (IsEditable != true)
                    IsEditable = true;
            }

        }

        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (!IsEditable)
                return;
            EditButtonClick(null, e);
            TextBoxText = originalText;
        }

        private void EditControlPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && IsEditable)
            {
                EditButtonClick(null, e);
            }
        }
    }
}
