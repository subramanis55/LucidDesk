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
        private BitmapImage deskImage;

        public byte[] ImageData { set; get; }

        [JsonIgnore]
        public BitmapImage Image
        {
            get
            {
                return deskImage == null ? getBitMapImage() : deskImage;
            }
        }
        private BitmapImage getBitMapImage()
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
      public System.Drawing.Bitmap GetBitMap()
        {
            if (ImageData == null)
                return null;
            using (MemoryStream memoryStream = new MemoryStream(ImageData))
            {
              
                return new System.Drawing.Bitmap(memoryStream);
            }
        }
    }
}
