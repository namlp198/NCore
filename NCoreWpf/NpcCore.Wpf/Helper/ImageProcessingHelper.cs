﻿using System.IO;
using System.Windows.Media.Imaging;

namespace NpcCore.Wpf.Helpers
{
    public static class ImageProcessingHelper
    {
        /// <summary>
        /// Convert from Byte array to Bitmap image
        /// </summary>
        /// <param name="imageData"></param>
        /// <returns></returns>
        public static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;

            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }


    }
}