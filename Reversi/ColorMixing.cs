using System.Windows.Media;

namespace Reversi
{
    static class ColorMixing
    {
        public static Color Lerp(this Color color, Color othercolor, double weight)
        {
            byte r = (byte)(weight * color.R + (1 - weight) * othercolor.R);
            byte g = (byte)(weight * color.G + (1 - weight) * othercolor.G);
            byte b = (byte)(weight * color.B + (1 - weight) * othercolor.B);
            return Color.FromRgb(r, g, b);
        }

        public static SolidColorBrush Lerp(this SolidColorBrush brush, SolidColorBrush otherBrush, double weight)
        {
            return new SolidColorBrush(Lerp(brush.Color, otherBrush.Color, weight));
        }
    }
}