using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LucidDesk.Manager.Classes.DataSchema
{
    public class DeskImageData
    {
        private BitmapImage profileImage;

        public byte[] ImageData { set; get; }

        [JsonIgnore]
        public BitmapImage Image
        {
            get
            {
                return profileImage == null ? getBitImage() : profileImage;
            }
        }
        private BitmapImage getBitImage()
        {
            if (ImageData == null)
                return null;
            using (MemoryStream memoryStream = new MemoryStream(ImageData))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = memoryStream;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }
    }
}
