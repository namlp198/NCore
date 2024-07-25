using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using ReadCodeGUI.Commons;
using System.Windows;
using ReadCodeGUI.Models;

namespace ReadCodeGUI.Manager.Class
{
    public class SQLite_Manager
    {
        #region variables
        public bool m_bSqliteConnected = false;
        public bool m_bCheckconnect = false;
        SQLiteConnection m_sqliteConn = new SQLiteConnection("Datasource=" + Defines.DBFilePath + ";Version=3;Compress=True;");
        #endregion

        #region SingleTon
        private static SQLite_Manager m_instance;
        public static SQLite_Manager Instance
        {
            get
            {
                if(m_instance == null)
                    m_instance = new SQLite_Manager();
                return m_instance;
            }
            private set { }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Do check have see connection?
        /// </summary>
        public void CheckConnect()
        {
            try
            {
                m_sqliteConn.Open();
                if (m_sqliteConn.State == ConnectionState.Open)
                {
                    m_bCheckconnect = true;
                    m_sqliteConn.Close();
                }
                else
                {
                    m_bCheckconnect = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                m_bCheckconnect = false;
            }
        }

        /// <summary>
        /// Function Open Connect to Database sqlite
        /// </summary>
        public void OpenConnectionToSqlite()
        {
            try
            {
                if (m_sqliteConn.State != ConnectionState.Open)
                {
                    m_sqliteConn.Open();
                    m_bSqliteConnected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Function Close Connect to Database sqlite
        /// </summary>
        public void CloseConnectionToSqlite()
        {
            try
            {
                if (m_sqliteConn.State == ConnectionState.Open)
                {
                    m_sqliteConn.Close();
                    m_bSqliteConnected = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void InsertData(ExcelTemplateModel excelModel, string tableName)
        {
            OpenConnectionToSqlite();

            if(m_bSqliteConnected)
            {
                if(excelModel != null)
                {
                    string query = "INSERT INTO " + tableName + " (Id,ProductName,ProductCode,Date,Judgement,Note) VALUES" + 
                                    "(@Id,@ProductName,@ProductCode,@Date,@Judgement,@Note)";

                    try
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(query, m_sqliteConn))
                        {
                            cmd.Parameters.AddWithValue("Id", excelModel.Id);
                            cmd.Parameters.AddWithValue("ProductName", excelModel.ProductName);
                            cmd.Parameters.AddWithValue("ProductCode", excelModel.ProductCode);
                            cmd.Parameters.AddWithValue("Date", excelModel.Date);
                            cmd.Parameters.AddWithValue("Judgement", excelModel.Judgement);
                            cmd.Parameters.AddWithValue("Note", excelModel.Note);

                            cmd.ExecuteNonQuery();
                        }
                        CloseConnectionToSqlite();
                    }
                    catch (Exception ex)
                    {
                        CloseConnectionToSqlite();
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
        public List<ExcelTemplateModel> SelectAllData(string tableName)
        {
            //string[] temp = new string[6];
            OpenConnectionToSqlite();
            if (m_bSqliteConnected)
            {
                List<ExcelTemplateModel> listExcelModel = new List<ExcelTemplateModel>();
                string query = "SELECT * FROM " + tableName;

                using (SQLiteCommand cmd = new SQLiteCommand(query, m_sqliteConn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                ExcelTemplateModel excelModel = new ExcelTemplateModel();
                                excelModel.Id = int.Parse(reader.GetValue(0).ToString());
                                excelModel.ProductName = reader.GetValue(1).ToString();
                                excelModel.ProductCode = reader.GetValue(2).ToString();
                                excelModel.Date = reader.GetValue(3).ToString();
                                excelModel.Judgement = reader.GetValue(4).ToString();
                                excelModel.Note = reader.GetValue(5).ToString();

                                listExcelModel.Add(excelModel);
                            }
                        }
                        catch (Exception ex)
                        {
                            CloseConnectionToSqlite();
                            MessageBox.Show(ex.Message);
                            return null;
                        }
                    }
                }
                CloseConnectionToSqlite();
                return listExcelModel;
            }
            else return null;
        }
        #endregion
    }
}
