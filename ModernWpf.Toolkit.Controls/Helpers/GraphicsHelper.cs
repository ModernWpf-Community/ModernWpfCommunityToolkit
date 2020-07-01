using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModernWpf.Toolkit.Controls.Helpers
{
    internal static class GraphicsHelper
    {
        public static Color GetPixelColor(this BitmapFrame bitmapFrame, double x, double y)
        {
            if (x <= bitmapFrame.PixelWidth && y <= bitmapFrame.PixelHeight)
            {
                var croppedBitmap = new CroppedBitmap(bitmapFrame, new Int32Rect((int)x, (int)y, 1, 1));
                var pixels = new byte[4];
                croppedBitmap.CopyPixels(pixels, 4, 0);
                return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
            }
            return Colors.Transparent;
        }
    }
}
