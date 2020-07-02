using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModernWpf.Toolkit.UI.Controls.Helpers
{
    internal static class GraphicsHelper
    {
        internal static Color GetPixelColor(this BitmapFrame bitmapFrame, double x, double y)
        {
            if (IsPositionOutsideBitmapBounds(bitmapFrame, x, y))
            {
                return Colors.Transparent;
            }

            var croppedBitmap = new CroppedBitmap(bitmapFrame, new Int32Rect((int)x, (int)y, 1, 1));
            var pixels = new byte[4];
            croppedBitmap.CopyPixels(pixels, 4, 0);
            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
        }

        internal static Brush GetPixelBrush(this BitmapFrame bitmapFrame, double x, double y)
        {
            return new SolidColorBrush(GetPixelColor(bitmapFrame, x, y));
        }

        private static bool IsPositionOutsideBitmapBounds(this BitmapFrame bitmapFrame, double x, double y)
        {
            return x < 0 || y < 0 || x >= bitmapFrame.PixelWidth || y >= bitmapFrame.PixelHeight;
        }
    }
}
