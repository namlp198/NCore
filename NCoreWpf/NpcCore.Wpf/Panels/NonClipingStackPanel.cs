using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NpcCore.Wpf.Panels
{
    public class NonClipingGrid : Grid
    {
        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return null;
        }
    }

    public class NonClipingStackPanel : StackPanel
    {
        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return null;
        }
    }
}
