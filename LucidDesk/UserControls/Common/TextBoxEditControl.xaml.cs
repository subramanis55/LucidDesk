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
    /// Interaction logic for TextBoxEditControl.xaml
    /// </summary>
    public partial class TextBoxEditControl : UserControl
    {
        public event EventHandler<string> DoneButtonClicked;

        private string originalText;
        public string TextBoxText
        {
            get
            {
                return textBox.Text;
            }
            set
            {
                textBox.Text = value;
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
            if(e.Key == Key.Enter&&IsEditable){
                EditButtonClick(null, e);
            }
        }
    }
}
