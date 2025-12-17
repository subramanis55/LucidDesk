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
    /// Interaction logic for SectionTabControl.xaml
    /// </summary>
    public partial class SessionHeaderControl : UserControl
    {
        public event EventHandler OnClickClose;
         
        private bool isCloseButtonVisible;

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public bool IsCloseButtonVisible
        {
            get
            {
                return isCloseButtonVisible;

            }

            set
            {
                isCloseButtonVisible = value;
                if (isCloseButtonVisible)
                    CloseButton.Visibility = Visibility.Visible;
                else
                {
                    CloseButton.Visibility = Visibility.Hidden;
                }
            }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(SessionHeaderControl), new PropertyMetadata("New Session"));

        public SessionHeaderControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void CancelMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CancelMouseDown(object sender, RoutedEventArgs e)
        {
        try{
                OnClickClose?.Invoke(this, EventArgs.Empty);
            }
            catch{

            }
            
        }
    }
}
