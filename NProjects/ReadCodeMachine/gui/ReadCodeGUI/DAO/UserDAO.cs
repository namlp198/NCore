using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReadCodeGUI.Models;

namespace ReadCodeGUI.DAO
{
    public class UserDAO : IDisposable
    {
        DbTestContext db = null;
        public UserDAO()
        {
            db = new DbTestContext();
        }

        public void AddNewExcelModel(ExcelTemplateModel excelTemplateModel)
        {
            try
            {
                db.Add(excelTemplateModel);
                db.SaveChanges();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
