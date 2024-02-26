using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace NpcCore.Wpf.Core
{
    internal static class MaterialHelper
    {
        internal static Material Create(ImageSource image)
        {
            if (image == null)
                return null;

            ImageBrush brush = new ImageBrush(image);

            var material = new DiffuseMaterial(brush);
            return material;
        }

        internal static Material Create(Color color, double opacity = 1)
        {
            Brush brush = new SolidColorBrush(color)
            {
                Opacity = opacity
            };

            var material = new DiffuseMaterial(brush);
            return material;
        }
    }
}
