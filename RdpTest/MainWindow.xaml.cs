using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
namespace RdpTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdpControl;
        private DispatcherTimer captureTimer;

        public MainWindow()
        {
            InitializeComponent();

            //// Initialize RDP control
            //rdpControl = new AxMSTSCLib.AxMsRdpClient9NotSafeForScripting();
            //rdpControl.BeginInit();
            //WindowsFormsHost.Child = rdpControl;
            //rdpControl.EndInit();

            //// Configure RDP settings
            //rdpControl.Server = "your-rdp-server";
            //rdpControl.UserName = "your-username";
            //rdpControl.AdvancedSettings8.ClearTextPassword = "your-password";
            //rdpControl.AdvancedSettings8.EnableCredSspSupport = true;
            //rdpControl.Connect();

            //// Set up capture timer
            //captureTimer = new DispatcherTimer();
            //captureTimer.Interval = TimeSpan.FromSeconds(1); // Capture every second
            //captureTimer.Tick += CaptureTimer_Tick;
            //captureTimer.Start();
        }

        //private void CaptureTimer_Tick(object sender, EventArgs e)
        //{
        //    CaptureRdpScreen();
        //}

        //private void CaptureRdpScreen()
        //{
        //    try
        //    {
        //        Bitmap bitmap = new Bitmap(rdpControl.Width, rdpControl.Height);
        //        Graphics graphics = Graphics.FromImage(bitmap);
        //        graphics.CopyFromScreen(rdpControl.PointToScreen(System.Drawing.Point.Empty), System.Drawing.Point.Empty, bitmap.Size);

        //        // Convert Bitmap to BitmapImage
        //        BitmapImage bitmapImage = new BitmapImage();
        //        using (MemoryStream memoryStream = new MemoryStream())
        //        {
        //            bitmap.Save(memoryStream, ImageFormat.Bmp);
        //            memoryStream.Position = 0;
        //            bitmapImage.BeginInit();
        //            bitmapImage.StreamSource = memoryStream;
        //            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //            bitmapImage.EndInit();
        //        }

        //        // Display the captured image (assuming you have an Image control named 'CapturedImage' in your XAML)
        //        CapturedImage.Source = bitmapImage;
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.MessageBox.Show("Error capturing RDP screen: " + ex.Message);
        //    }
        //}
    }
}


