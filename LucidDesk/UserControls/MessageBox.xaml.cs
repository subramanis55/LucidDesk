using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LucidDesk.UserControls
{
    public enum MessageBoxType
    {
        Ok, YesOrNo
    }
    public partial class MessageBox : Window
    {
        public MessageBoxType messageBoxType;
        public DialogResult DialogResult;
        public MessageBoxType MessageBoxType
        {
            set
            {
                messageBoxType = value;
                if(messageBoxType==MessageBoxType.Ok){
                    MainContainer.RowDefinitions[2].Height = new GridLength(0);
                }
                else if(messageBoxType == MessageBoxType.YesOrNo) {
                    MainContainer.RowDefinitions[3].Height = new GridLength(0);
                }
            }
            get
            {
                return messageBoxType;
            }
        }
        public MessageBox()
        {

            InitializeComponent();
        
        }

        private void YesButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = DialogResult.Yes;
           Close();
        }

        private void NoButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

    

        private void TopPanelMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        public DialogResult ShowMessageBox(string message, string title, MessageBoxType messageBoxType)
        {
            HeaderText.Text = title;
            MessageText.Text = message;
            MessageBoxType = messageBoxType;
            this.ShowDialog();
            return DialogResult;
        }
    }
}
