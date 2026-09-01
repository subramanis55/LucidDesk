using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
namespace LucidDesk.UserControls.Common
{
    /// <summary>
    /// Interaction logic for PasswordTextBox.xaml
    /// </summary>
    public partial class PasswordTextBox : UserControl
    {
        public event RoutedEventHandler LostFocus;
        public event RoutedEventHandler GotFocus;
        public event TextChangedEventHandler TextChanged;
        public event RoutedEventHandler PasswordChanged;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(PasswordTextBox), new PropertyMetadata(""));
        public static readonly DependencyProperty TextPaddingProperty = DependencyProperty.Register("TextPadding", typeof(Thickness), typeof(PasswordTextBox), new PropertyMetadata(new Thickness(10, 5, 10, 5)));
        public static readonly DependencyProperty TextboxBorderThicknessProperty =
         DependencyProperty.Register("TextboxBorderThickness", typeof(Thickness), typeof(PasswordTextBox), new PropertyMetadata(new Thickness(0, 0, 0, 2.2)));

        public static readonly DependencyProperty TextboxBorderBrushProperty =
         DependencyProperty.Register("TextboxBorderBrush", typeof(Brush), typeof(PasswordTextBox), new PropertyMetadata(new SolidColorBrush(Colors.Gainsboro)));

        public static readonly DependencyProperty CornerRadiusProperty =
          DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(PasswordTextBox), new PropertyMetadata(new CornerRadius(4, 4, 4, 4)));

        public static readonly DependencyProperty TextboxBackgroundProperty =
            DependencyProperty.Register("TextboxBackground", typeof(Brush), typeof(PasswordTextBox), new PropertyMetadata(new SolidColorBrush(Colors.Gainsboro)));

        public Brush TextboxBorderBrush
        {
            get { return (Brush)GetValue(TextboxBorderBrushProperty); }
            set { SetValue(TextboxBorderBrushProperty, value); }
        }

        public Brush TextboxBackground
        {
            get { return (Brush)GetValue(TextboxBackgroundProperty); }
            set { SetValue(TextboxBackgroundProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
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
        public PasswordTextBox()
        {
            InitializeComponent();

            // expose properties to bindings in XAML
            DataContext = this;

            // handle placeholder click to focus the appropriate input
            PlaceholderLabel.MouseDown += PlaceholderLabel_MouseDown;

            // handle focus changes to show/hide placeholder
            PART_PasswordBox.GotFocus += Input_GotFocus;
            PART_PasswordBox.LostFocus += Input_LostFocus;
            PART_VisibleTextBox.GotFocus += Input_GotFocus;
            PART_VisibleTextBox.LostFocus += Input_LostFocus;

            UpdatePlaceholderVisibility();
            // set default TextboxBackground from theme resource if not explicitly set

        }


        private bool _isSyncing;

        // Placeholder dependency property
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(PasswordTextBox), new PropertyMetadata(""));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly DependencyProperty PlaceholderColorProperty =
            DependencyProperty.Register("PlaceholderColor", typeof(Brush), typeof(PasswordTextBox), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDD"))));

        public Brush PlaceholderColor
        {
            get => (Brush)GetValue(PlaceholderColorProperty);
            set => SetValue(PlaceholderColorProperty, value);
        }


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
            UpdatePlaceholderVisibility();
        }

        // ---- Sync PasswordBox -> Password property ----
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isSyncing) return;
            _isSyncing = true;
            Password = PART_PasswordBox.Password;
            _isSyncing = false;
            UpdatePlaceholderVisibility();
        }

        // ---- Sync TextBox -> Password property ----
        private void VisibleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSyncing) return;
            _isSyncing = true;
            Password = PART_VisibleTextBox.Text;
            _isSyncing = false;
            UpdatePlaceholderVisibility();
        }

        private void PlaceholderLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // focus the currently visible input control
            if (PART_VisibleTextBox.Visibility == Visibility.Visible)
                PART_VisibleTextBox.Focus();
            else
                PART_PasswordBox.Focus();

            e.Handled = true;
        }

        private void Input_GotFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderLabel.Visibility = Visibility.Hidden;
        }

        private void Input_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdatePlaceholderVisibility();
        }

        private void UpdatePlaceholderVisibility()
        {
            // Show placeholder when there is no password and neither input has focus
            bool hasText = !string.IsNullOrEmpty(Password);
            bool hasFocus = PART_PasswordBox.IsFocused || PART_VisibleTextBox.IsFocused;
            PlaceholderLabel.Visibility = (!hasText && !hasFocus) ? Visibility.Visible : Visibility.Hidden;
        }
    }
}

