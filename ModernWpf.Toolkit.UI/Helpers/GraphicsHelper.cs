using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModernWpf.Toolkit.UI.Helpers
{
    public static class GraphicsHelper
    {
        public static Color GetPixelColor(this BitmapSource bitmapSource, double x, double y)
        {
            if (IsPositionOutsideBitmapBounds(bitmapSource, x, y))
            {
                return Colors.Transparent;
            }

            var croppedBitmap = new CroppedBitmap(bitmapSource, new Int32Rect((int)x, (int)y, 1, 1));
            var pixels = new byte[4];
            croppedBitmap.CopyPixels(pixels, 4, 0);
            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
        }

        public static Brush GetPixelBrush(this BitmapSource bitmapSource, double x, double y)
        {
            return new SolidColorBrush(GetPixelColor(bitmapSource, x, y));
        }

        private static bool IsPositionOutsideBitmapBounds(this BitmapSource bitmapSource, double x, double y)
        {
            return x < 0 || y < 0 || x >= bitmapSource.PixelWidth || y >= bitmapSource.PixelHeight;
        }
    }
}
