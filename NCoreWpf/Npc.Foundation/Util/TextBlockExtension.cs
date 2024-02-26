using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Npc.Foundation.Util
{
    /// <summary>
    /// TextBlock Element Extension
    /// </summary>
    public static class TextBlockExtension
    {
        /// <summary>
        /// Find TextRange
        /// </summary>
        /// <param name="ele"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static TextRange FindTextRange(this TextBlock ele, string text)
        {
            try
            {
                var startIndex = ele.Text.ToUpper().IndexOf(text.ToUpper());
                if (startIndex > -1)
                {
                    var startPosition = ele.ContentStart.GetPositionAtOffset(startIndex + 1);
                    var endPosition = ele.ContentStart.GetPositionAtOffset(startIndex + text.Length + 1);
                    return new TextRange(startPosition, endPosition);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Clear Added TextRange Property
        /// </summary>
        /// <param name="ele"></param>
        public static void ClearAddedTextRangeProperty(this TextBlock ele)
        {
            var textRange = new TextRange(ele.ContentStart, ele.ContentEnd);
            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, ele.Background);
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, ele.Foreground);
        }
    }
}
