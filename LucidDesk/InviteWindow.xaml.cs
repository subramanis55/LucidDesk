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
using System.Windows.Shapes;

namespace LucidDesk
{
    /// <summary>
    /// Interaction logic for InviteWindow.xaml
    /// </summary>
    public partial class InviteWindow : Window
    {
        public InviteWindow()
        {
            InitializeComponent();
            AccessTypeCombobox.ItemsSource = new List<string>() { "Default", "bbb", "BDCSEW" };
            AccessTypeCombobox.SelectedItem = "Default";
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {

        }

      

        private void KeyboardAccessCheckBoxClick(object sender, RoutedEventArgs e)
        {

        }

    

        private void MouseAccessCheckBoxClick(object sender, RoutedEventArgs e)
        {

        }

       

    

        private void FileAccessCheckBoxClick(object sender, RoutedEventArgs e)
        {

        }

        private void ClipboardAccessCheckBoxClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
