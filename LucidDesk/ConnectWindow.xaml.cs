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
using System.Windows.Threading;

namespace LucidDesk
{
    /// <summary>
    /// Interaction logic for ConnectWindow.xaml
    /// </summary>
    public partial class ConnectWindow : Window
    {
        private BitmapImage _gifImage;
        private DispatcherTimer _timer;
        private int _currentFrame = 0;
        private BitmapFrame[] _frames;

        public ConnectWindow()
        {
            InitializeComponent();
            LoadAnimatedGif("YourGifFile.gif");
        }

        private void LoadAnimatedGif(string fileName)
        {
            var uri = new Uri($"D:\\loadingGif.gif", UriKind.RelativeOrAbsolute);
            var decoder = new GifBitmapDecoder(uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            _frames = decoder.Frames.ToArray();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(50); // Adjust the interval to control the animation speed
            _timer.Tick += (s, e) =>
            {
                _currentFrame = (_currentFrame + 1) % _frames.Length;
                AnimatedGifImage.Source=_frames[_currentFrame];
            };
            _timer.Start();
        }
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
