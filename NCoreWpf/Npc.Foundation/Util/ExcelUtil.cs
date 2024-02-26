using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace Npc.Foundation.Util
{
    public static class ExcelUtil
    {
        /// <summary>
        /// Create Excel Application.
        /// Must be Call ReleaseExcelObject().
        /// </summary>
        /// <param name="displayAlerts"></param>
        /// <returns></returns>
        public static Excel.Application CreateApp(bool visible = false, bool displayAlerts = false)
        {
            Excel.Application excelApp = new Excel.Application();
            if (null != excelApp)
            {
                excelApp.DisplayAlerts = displayAlerts; // true일 경우 메시지 발생.
            }

            return excelApp;
        }

        /// <summary>
        /// Load Excel File.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Excel.Workbook LoadExcel(Excel.Application app, string filePath)
        {
            // [NCS-2695] CID 171193 Dereference null return (stat)
            //return app.Workbooks.Open(Filename: filePath);
            return app?.Workbooks?.Open(Filename: filePath);
        }

        /// <summary>
        /// Add WorkBook.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static Excel.Workbook AddBook(Excel.Application app)
        {
            return app?.Workbooks?.Add();
        }

        /// <summary>
        /// Add Worksheet.
        /// </summary>
        /// <param name="book"></param>
        /// <param name="name"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public static Excel.Worksheet AddSheet(Excel.Workbook book, string name, Excel.Worksheet after)
        {
            var sheet = book?.Worksheets?.Add(After: after);
            if (null != sheet)
            {
                sheet.Name = name;
            }

            return sheet;
        }

        /// <summary>
        /// Excel Object Release
        /// </summary>
        /// <param name="app"></param>
        /// <param name="book"></param>
        /// <param name="objs"></param>
        public static void Release(Excel.Application app, Excel.Workbook book, object[] objs)
        {
            foreach (var obj in objs)
            {
                ExcelUtil.ReleaseExcelObject(obj);
            }

            ExcelUtil.CloseWorkBook(book);
            ExcelUtil.ReleaseExcelObject(book);
            ExcelUtil.QuitApplication(app);
            ExcelUtil.ReleaseExcelObject(app);
        }

        /// <summary>
        /// Close WorkBook.
        /// </summary>
        /// <param name="book"></param>
        public static void CloseWorkBook(Excel.Workbook book)
        {
            book?.Close(0);
        }

        /// <summary>
        /// Quit Excel Application.
        /// </summary>
        /// <param name="app"></param>
        public static void QuitApplication(Excel.Application app)
        {
            app?.Quit();
        }

        /// <summary>
        /// Release Object.
        /// All of Excel Object are must be Released.
        /// </summary>
        /// <param name="obj"></param>
        public static void ReleaseExcelObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
