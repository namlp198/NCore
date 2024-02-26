using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Npc.Foundation.Util.Bitmap
{
    public class BitmapImageEx : IDisposable
    {
        public BitmapImageEx(BitmapImage bi)
        {
            OnePixelToUnitByWidth = 1.0d;
            OneUnitToPixelByWidth = 1.0d;

            this.Image = bi;

            OriginWidth = bi.PixelWidth;
            OriginHeight = bi.PixelHeight;

            //Koh.Wpf.Foundation.Test.InstanceLifeCycleTester.CreatedObject("BitmapImageEx-" + this.GetHashCode());
        }

        ~BitmapImageEx()
        {
            //Koh.Wpf.Foundation.Test.InstanceLifeCycleTester.DestroiedObject("BitmapImageEx-" + this.GetHashCode());
        }

        public bool IsError { get; set; }
        public BitmapImage Image { get; set; }

        public int OriginWidth { get; set; }
        public int OriginHeight { get; set; }

        public double UnitWidth { get; private set; }
        public double UnitHeight { get; private set; }
        public string UnitName { get; set; }

        public bool HasUnitInfo { get; set; }

        public object Data { get; set; }

        public double OnePixelToUnitByWidth { get; set; }
        public double OneUnitToPixelByWidth { get; set; }
        public double OnePixelToUnitByHeight { get; set; }
        public double OneUnitToPixelByHeight { get; set; }

        public void SetSizeInfo(int originWidth, int originHeight, double unitWidth, double unitHeight, string unitName)
        {
            this.OriginWidth = originWidth;
            this.OriginHeight = originHeight;
            this.UnitWidth = unitWidth;
            this.UnitHeight = unitHeight;
            this.UnitName = unitName;
            this.HasUnitInfo = true;

            //this.PixelPerUnit = this.OriginWidth / this.UnitWidth;
            //this.UnitPerPixel = this.UnitWidth / this.OriginWidth;

            this.OnePixelToUnitByWidth = this.UnitWidth / this.OriginWidth;
            this.OneUnitToPixelByWidth = this.OriginWidth / this.UnitWidth;
            this.OnePixelToUnitByHeight = this.UnitHeight / this.OriginHeight;
            this.OneUnitToPixelByHeight = this.OriginHeight / this.UnitHeight;
        }



        public void Dispose()
        {
            if (Image != null)
            {
                if (Image.StreamSource != null)
                {
                    Image.StreamSource.Dispose();
                    GC.Collect();
                }
            }
            Image = null;

            //Koh.Wpf.Foundation.Test.InstanceLifeCycleTester.ChangeStatus("BitmapImageEx-" + this.GetHashCode(), "Disposed");
        }
    }
}
