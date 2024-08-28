using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using NVisionInspectGUI.Models;

namespace NVisionInspectGUI.Manager.Class
{
    public class Csv_Manager
    {
        private string m_currentDailyDatafilePath;
        public Csv_Manager() 
        {
           
        }
        public void Initialize()
        {
            m_currentDailyDatafilePath = Environment.CurrentDirectory + "\\VisionSettings\\Report\\" + DateTime.Now.ToString("dd-MM-yyyy") + "_Detail_Report" + ".csv";
            if (!File.Exists(m_currentDailyDatafilePath))
            {
                using (File.Create(m_currentDailyDatafilePath)) { }
            }
        }
        private static Csv_Manager m_instance;
        public static Csv_Manager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new Csv_Manager();
                return m_instance;
            }
            private set { }
        }

        public void WriteNewModelToCsv(List<ExcelTemplateModel> excelTemplateModels)
        {
            try
            {
                if (!File.Exists(m_currentDailyDatafilePath))
                {
                    using (File.Create(m_currentDailyDatafilePath)) { }
                }

                var configExcelTempModel = new CsvConfiguration(CultureInfo.InvariantCulture);
                {
                    configExcelTempModel.HasHeaderRecord = false;
                };

                using (StreamWriter streamWriter = new StreamWriter(m_currentDailyDatafilePath, true))
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, configExcelTempModel))
                {
                    csvWriter.WriteRecordsAsync(excelTemplateModels);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public List<ExcelTemplateModel> ReadExcelTemplateModelFromCsv()
        {
            try
            {
                if(File.Exists(m_currentDailyDatafilePath))
                {
                    List<ExcelTemplateModel> excelTemplateModels = new List<ExcelTemplateModel>();
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture);
                    {
                        config.HasHeaderRecord = false;
                    }
                    using (StreamReader streamReader = new StreamReader(m_currentDailyDatafilePath))
                    using (CsvReader csvReader = new CsvReader(streamReader, config))
                    {
                        // Read records from CSV file
                        IEnumerable<ExcelTemplateModel> records = csvReader.GetRecords<ExcelTemplateModel>();
                        excelTemplateModels = records.ToList();
                    }
                    return excelTemplateModels;
                }
                return null;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
