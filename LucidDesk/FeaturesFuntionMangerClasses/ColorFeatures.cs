using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LucidDesk.FeaturesFuntionMangerClasses
{
    public static class ColorFeatures
    {
        public static BitmapImage CreateBitmapImageWithCharacter(int width, int height, Color backgroundColor, char character, string fontFamily, double fontSize, Color fontColor)
        {
            // Create a DrawingVisual for rendering
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                // Draw the background
                drawingContext.DrawRectangle(new SolidColorBrush(backgroundColor), null, new Rect(0, 0, width, height));

                // Set up the text format
                FormattedText formattedText = new FormattedText(
                    character.ToString(),
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(fontFamily),
                    fontSize,
                    new SolidColorBrush(fontColor),
                    new NumberSubstitution(),
                    TextFormattingMode.Display);

                // Draw the character in the center of the bitmap
                drawingContext.DrawText(formattedText, new Point((width - formattedText.Width) / 2, (height - formattedText.Height) / 2));
            }

            // Render the DrawingVisual to a RenderTargetBitmap
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(drawingVisual);

            // Convert RenderTargetBitmap to BitmapImage
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            using (var memoryStream = new System.IO.MemoryStream())
            {
                encoder.Save(memoryStream);
                memoryStream.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
        public static Color GetColorBasedOnFirstChar(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return Colors.Transparent; // Default or error color
            }

            char firstChar = char.ToUpper(input[0]); // Handle case-insensitivity

            Color color;

            switch (firstChar)
            {
                case 'A': color = (Color)ColorConverter.ConvertFromString("#007ACC"); break;
                case 'B': color = Colors.Blue; break;
                case 'C': color = Colors.Green; break;
                case 'D': color = Colors.Yellow; break;
                case 'E': color = Colors.Orange; break;
                case 'F': color = Colors.Purple; break;
                case 'G': color = Colors.Teal; break;
                case 'H': color = Colors.Pink; break;
                case 'I': color = Colors.Brown; break;
                case 'J': color = Colors.Gray; break;
                case 'K': color = (Color)ColorConverter.ConvertFromString("#00DACC"); break;
                case 'L': color = Colors.Magenta; break;
                case 'M': color = Colors.Lime; break;
                case 'N': color = Colors.Indigo; break;                                   
                case 'O': color = Colors.Maroon; break;
                case 'P': color = Colors.Olive; break;
                case 'Q': color = Colors.Silver; break;
                case 'R': color = Colors.Gold; break;
                case 'S': color = (Color)ColorConverter.ConvertFromString("#00B9CC"); break;
                case 'T': color = Colors.Violet; break;
                case 'U': color = (Color)ColorConverter.ConvertFromString("#41A5FF"); break;
                case 'V': color = Colors.Honeydew; break;
                case 'W': color = Colors.Lavender; break;
                case 'X': color = Colors.Salmon; break;
                case 'Y': color = Colors.Beige; break;
                case 'Z': color = Colors.Wheat; break;
                default:
                color = Colors.Gray;break; 
            }

            return color;
        }
    }
}
