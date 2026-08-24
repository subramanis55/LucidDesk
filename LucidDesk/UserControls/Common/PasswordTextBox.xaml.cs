using System.Windows;
using System.Windows.Controls;
namespace LucidDesk.UserControls.Common
{
    /// <summary>
    /// Interaction logic for PasswordTextBox.xaml
    /// </summary>
    public partial class PasswordTextBox : UserControl
    {
        public PasswordTextBox()
        {
            InitializeComponent();
        }


        private bool _isSyncing;


        // ---- Password dependency property (the actual bound value) ----
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                nameof(Password),
                typeof(string),
                typeof(PasswordTextBox),
                new FrameworkPropertyMetadata(string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnPasswordPropertyChanged));

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PasswordTextBox)d;
            if (control._isSyncing) return;

            control._isSyncing = true;
            control.PART_PasswordBox.Password = e.NewValue as string ?? string.Empty;
            control.PART_VisibleTextBox.Text = e.NewValue as string ?? string.Empty;
            control._isSyncing = false;
        }

        // ---- IsPassword property: controls whether masking is enabled at all ----
        public static readonly DependencyProperty IsPasswordProperty =
            DependencyProperty.Register(
                nameof(IsPassword),
                typeof(bool),
                typeof(PasswordTextBox),
                new PropertyMetadata(true, OnIsPasswordChanged));

        public bool IsPassword
        {
            get => (bool)GetValue(IsPasswordProperty);
            set => SetValue(IsPasswordProperty, value);
        }

        private static void OnIsPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PasswordTextBox)d;
            bool isPassword = (bool)e.NewValue;

            // If IsPassword = false, always show as plain text and hide the toggle button
            control.PART_ToggleButton.Visibility = isPassword ? Visibility.Visible : Visibility.Collapsed;

            if (!isPassword)
            {
                control.PART_ToggleButton.IsChecked = true; // force "visible" state
            }
            control.UpdateVisibility();
        }

        // ---- Toggle handling ----
        private void ToggleButton_CheckedChanged(object sender, RoutedEventArgs e)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            bool showText = PART_ToggleButton.IsChecked == true;

            PART_VisibleTextBox.Visibility = showText ? Visibility.Visible : Visibility.Collapsed;
            PART_PasswordBox.Visibility = showText ? Visibility.Collapsed : Visibility.Visible;

            if (showText)
                PART_VisibleTextBox.Text = Password;
            else
                PART_PasswordBox.Password = Password;
        }

        // ---- Sync PasswordBox -> Password property ----
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isSyncing) return;
            _isSyncing = true;
            Password = PART_PasswordBox.Password;
            _isSyncing = false;
        }

        // ---- Sync TextBox -> Password property ----
        private void VisibleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSyncing) return;
            _isSyncing = true;
            Password = PART_VisibleTextBox.Text;
            _isSyncing = false;
        }
    }
}

