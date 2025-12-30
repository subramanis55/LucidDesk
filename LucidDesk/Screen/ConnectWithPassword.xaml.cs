using LucidDesk.Manager.Classes;
using LucidDesk.Manager;
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

namespace LucidDesk.Screen
{

    public partial class ConnectWithPassword : Window
    {
        public event EventHandler<Desk> OnClickConnectButton;
        private Desk desk;
        public Desk Desk
        {
            set
            {
                desk = value;
                Textbox.Text = desk.DisplayID;
            }
            get
            {
                return desk;
            }
        }

        public ConnectWithPassword(Desk desk)
        {
            InitializeComponent();
            Desk = desk;
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void TopPanelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
            e.Handled = true;
        }

        private void ConnectButtonClick(object sender, RoutedEventArgs e)
        {
            Desk.Password = passwordLabelTextblock.Text;
            OnClickConnectButton?.Invoke(this, Desk);
        }
    }
}
