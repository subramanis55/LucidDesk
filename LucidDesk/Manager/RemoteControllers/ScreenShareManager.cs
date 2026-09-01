using LucidDesk.DS.Enum;
using LucidDesk.Manager.Settings;
using LucidDesK.DS.DataSchema;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LucidDesk.Manager.RemoteControllers
{
    public static class ScreenShareManager
    {
        public static Action<Data> NewImageGenerated;

        private static readonly object selectedScreenLock = new object();
        public static System.Windows.Forms.Screen SelectedScreen { private set; get; } = null;

        private static CancellationTokenSource cancellationTokenSource;

        public static void Start()
        {
            cancellationTokenSource = new CancellationTokenSource();
            ScreenShareForClients(cancellationTokenSource);
        }

        public static void Stop()
        {
            cancellationTokenSource.Cancel();
            IsScreenShareON = false;
            SelectedScreen = null;
        }


        static bool IsScreenShareON = false;
        private static async Task ScreenShareForClients(CancellationTokenSource token)
        {
            try
            {
                IsScreenShareON = true;
                while (IsScreenShareON)
                {
                    await Task.Delay(1000 / SettingsManager.Settings.FrameSharePerSecond);
                    var screenImage = GetScreenShareImage();
                    var screenData = new DeskImageData() { ImageData = screenImage };
                    Data data = new Data(ReponseAndReqType.ScreenShareImageData, screenData);
                    NewImageGenerated?.Invoke(data);
                }
                IsScreenShareON = false;

            }
            catch (Exception ex)
            {
                IsScreenShareON = false;
            }
        }

        public static void MoniterScreenSwitch(int screenIndex)
        {
            lock (selectedScreenLock)
            {
                if (screenIndex < 0)
                {
                    SelectedScreen = null;
                    return;
                }
                if (screenIndex < System.Windows.Forms.Screen.AllScreens.Length)
                    SelectedScreen = System.Windows.Forms.Screen.AllScreens[screenIndex];
            }
        }

        private static byte[] GetScreenShareImage()
        {
            System.Drawing.Rectangle bounds;
            lock (selectedScreenLock)
            {
                if (SelectedScreen == null)
                    bounds = SystemInformation.VirtualScreen;
                else bounds = SelectedScreen.Bounds;
            }
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
                }
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    var encoder = ImageCodecInfo.GetImageEncoders().First(e => e.FormatID == ImageFormat.Jpeg.Guid);
                    using (EncoderParameters encoderParams = new EncoderParameters(1))
                    {
                        encoderParams.Param[0] =
                            new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 60L);
                        bitmap.Save(memoryStream, encoder, encoderParams);
                    }
                    return memoryStream.ToArray();
                }
            }
        }
    }
}
