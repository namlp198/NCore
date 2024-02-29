using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfTest
{
    internal class ImageProcessorManager
    {
        public ImageProcessorDll m_imageProcessor = new ImageProcessorDll();

        public void Initialize()
        {
            m_imageProcessor.Initialize();
        }

        public void Release()
        {
            m_imageProcessor.DeleteImageProcessor();
        }
    }
}
