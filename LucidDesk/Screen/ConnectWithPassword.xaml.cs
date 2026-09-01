using LucidDesk.DS.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            Desk.Password = passwordTextbox.Password;
            OnClickConnectButton?.Invoke(this, Desk);
        }

        private void TextboxTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PasswordTextboxPasswordChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
