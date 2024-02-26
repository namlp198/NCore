using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Npc.Foundation.Util.Bitmap
{
    public class BitmapImageLoader
    {
        private BackgroundWorker _loader = null;

        private Action<BitmapImageEx> _fileAsyncCallback = null;


        public void LoadFromFileAsync(string path, Action<BitmapImageEx> callback)
        {
            FileInfo fi = new FileInfo(path);

            if (_loader == null)
            {
                _loader = new BackgroundWorker();
                _loader.DoWork += Loader_DoWork;
                _loader.RunWorkerCompleted += Loader_RunWorkerCompleted;
            }

            _fileAsyncCallback = callback;
            _loader.RunWorkerAsync(fi);
        }

        private void Loader_DoWork(object sender, DoWorkEventArgs e)
        {
            BitmapImageEx result = null;

            FileInfo fi = e.Argument as FileInfo;
            // [NCS-1252] [Coverity : 128562]
            //if (fi.Exists == true)
            if (fi != null && fi.Exists == true)
            {
                var bitmapFrame = BitmapFrame.Create(new Uri(fi.FullName, UriKind.Absolute), BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                var width = bitmapFrame.PixelWidth;
                var height = bitmapFrame.PixelHeight;

                MemoryStream imageStream = new MemoryStream();
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(new Uri(fi.FullName, UriKind.Absolute)));
                encoder.Save(imageStream);

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = imageStream;
                bi.EndInit();
                bi.Freeze();

                result = new BitmapImageEx(bi);
                result.OriginWidth = width;
                result.OriginHeight = height;
            }
            else
            {
                // go errror
            }

            e.Result = result;
        }

        private void Loader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_loader != null)
            {
                _loader.DoWork -= Loader_DoWork;
                _loader.RunWorkerCompleted -= Loader_RunWorkerCompleted;
                _loader = null;
            }

            if (_fileAsyncCallback != null)
            {
                _fileAsyncCallback(e.Result as BitmapImageEx);
            }

            _fileAsyncCallback = null;
        }
    }
}
