using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO.Packaging;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace OpenXMLParser
{
    public class ExcelParser
    {
        private SpreadsheetDocument m_excelDocument;
        private WorkbookPart m_workbookPart;

        public bool OpenFile(string sExcelFilePath, bool editable = false)
        {
            try 
	        {
                m_excelDocument = SpreadsheetDocument.Open(sExcelFilePath, editable);
                m_workbookPart = m_excelDocument.WorkbookPart;

                return true;
	        }
	        catch
	        {
                return false;
	        }
        }

        public void Close()
        {
            if ( m_excelDocument != null )
                m_excelDocument.Close();
        }

        public string GetCellValue( string sheetName, string addressName )
        {
            string value = null;

            Cell theCell = GetCell(sheetName, addressName);
            if (theCell == null)
                return value = "";
            // If the cell does not exist, return an empty string:
            if ( theCell != null )
            {
                value = theCell.InnerText;

                // If the cell represents a numeric value, you are done. 
                // For dates, this code returns the serialized value that 
                // represents the date. The code handles strings and Booleans
                // individually. For shared strings, the code looks up the 
                // corresponding value in the shared string table. For Booleans, 
                // the code converts the value into the words TRUE or FALSE.
                if ( theCell.DataType != null )
                {
                    switch ( theCell.DataType.Value )
                    {
                        case CellValues.SharedString:
                            // For shared strings, look up the value in the shared 
                            // strings table.
                            var stringTable = m_workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                            // If the shared string table is missing, something is 
                            // wrong. Return the index that you found in the cell.
                            // Otherwise, look up the correct text in the table.
                            if ( stringTable != null )
                            {
                                value = stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                            }
                            break;

                        case CellValues.Boolean:
                            switch ( value )
                            {
                                case "0":
                                    value = "FALSE";
                                    break;
                                default:
                                    value = "TRUE";
                                    break;
                            }
                            break;
                    }
                }                
            }
            return value;
        }

        public bool SetCellValue( string sheetName, string addressName, string value )
        {
            Cell theCell = GetCell(sheetName, addressName);
            // MLE : Case of you insert text in cell into an already row 
            if (theCell == null)
            {
                Sheet theSheet = m_workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).FirstOrDefault();               
                WorksheetPart wsPart = (WorksheetPart) (m_workbookPart.GetPartById(theSheet.Id));
                uint newRowIndex = uint.Parse(addressName.Substring(1));
                string columName = addressName.Substring(0,1);
                theCell = InsertCellInWorksheet(columName, newRowIndex, wsPart);
            }

            // If the cell does not exist, return an empty string:
            if ( theCell != null )
            {
                SetCellValue(theCell, value);
                return true;
            }
            else
            {
                return false;
            }
        }        

        public void AddCell(string sheetName, string addressName, string value )
        {
            // Find the sheet with the supplied name, and then use that Sheet
            // object to retrieve a reference to the appropriate worksheet.
            Sheet theSheet = m_workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).FirstOrDefault();

            // Retrieve a reference to the worksheet part, and then use its 
            // Worksheet property to get a reference to the cell whose 
            // address matches the address you supplied:
            WorksheetPart wsPart = (WorksheetPart) (m_workbookPart.GetPartById(theSheet.Id));


            // Get the sheetData cell table.
            SheetData sheetData = wsPart.Worksheet.GetFirstChild<SheetData>();

            int newRowIndex = int.Parse(addressName.Substring(1));

            // Add a row to the cell table.
            Row row;
            row = new Row()
            {
                RowIndex = Convert.ToUInt32(newRowIndex)
            };
            sheetData.Append(row);

            // In the new row, find the column location to insert a cell in A1.  
            Cell refCell = null;
            foreach ( Cell cell in row.Elements<Cell>() )
            {
                if ( string.Compare(cell.CellReference.Value, addressName, true) > 0 )
                {
                    refCell = cell;
                    break;
                }
            }

            Cell newCell = new Cell()
            {
                CellReference = addressName
            };

            SetCellValue(newCell, value);

            row.InsertBefore(newCell, refCell);            
        }

        private Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }

        public int GetLastEmptyRowIndex(string sheetName)
        {
            // Find the sheet with the supplied name, and then use that Sheet
            // object to retrieve a reference to the appropriate worksheet.
            Sheet theSheet = m_workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).FirstOrDefault();

            // Retrieve a reference to the worksheet part, and then use its 
            // Worksheet property to get a reference to the cell whose 
            // address matches the address you supplied:
            WorksheetPart wsPart = (WorksheetPart) (m_workbookPart.GetPartById(theSheet.Id));

            // Get the sheetData cell table.
            SheetData sheetData = wsPart.Worksheet.GetFirstChild<SheetData>();

            Row lastRow = (Row) sheetData.ChildElements.Last();            

            int cellIndex = Convert.ToInt32(lastRow.RowIndex.ToString());

            string cellValue = GetCellValue(sheetName, "A" + cellIndex);
            
            while( String.IsNullOrEmpty(cellValue) )
            {               
                cellIndex--;
                cellValue = GetCellValue(sheetName, "A" + cellIndex);
            }

            return cellIndex + 1;           
        }

        public void Save()
        {
            foreach ( WorksheetPart wsPart in m_workbookPart.WorksheetParts )
            {
                try
                {
                    wsPart.Worksheet.Save();
                }
                catch ( Exception e )
                {
                }
            }
        }

        private Cell GetCell(string sheetName, string addressName)
        {
            // Find the sheet with the supplied name, and then use that Sheet
            // object to retrieve a reference to the appropriate worksheet.
            Sheet theSheet = m_workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).FirstOrDefault();

            if ( theSheet == null )            
                return null;            

            // Retrieve a reference to the worksheet part, and then use its 
            // Worksheet property to get a reference to the cell whose 
            // address matches the address you supplied:            
            WorksheetPart wsPart = (WorksheetPart) (m_workbookPart.GetPartById(theSheet.Id));
            return wsPart.Worksheet.Descendants<Cell>().Where(c => c.CellReference == addressName).FirstOrDefault();
        }

        private int InsertSharedStringItem( string text, SharedStringTablePart shareStringPart )
        {
            // If the part does not contain a SharedStringTable, create one.
            if ( shareStringPart.SharedStringTable == null )
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach ( SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>() )
            {
                if ( item.InnerText == text )
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        private void SetCellValue( Cell cell, string value )
        {
            var stringTable = m_workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

            // Insert the text into the SharedStringTablePart.
            int index = InsertSharedStringItem(value, stringTable);

            cell.CellValue = new CellValue(index.ToString());
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
        }
    }
}
