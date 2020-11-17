using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UserScript.CamRAC;
using UserScript.SystemService;

namespace UserScript
{
    partial class APAS_UserScript
    {
        #region Variables

        #endregion

        #region User Process

        /// <summary>
        /// The section of the user process.
        /// 用户自定义流程函数。
        /// 
        /// Please write your process in the following method.
        /// 请在以下函数中定义您的工艺流程。
        /// 
        /// </summary>
        /// <param name="Apas"></param>
        /// <returns></returns>
        static void UserProc(SystemServiceClient Apas, CamRemoteAccessContractClient Camera = null, IOptions opts = null)
        {
            var var1 = $"{opts.PrefixVarRead}CH0";
            var var2 = $"{opts.PrefixVarRead}CH3";
            object strch0, strch3;

            try
            {
                strch0 = Apas.__SSC_ReadVariable(var1);
            }
            catch(NullReferenceException)
            {
                var err = $"无法找到变量[{var1}]";
                Apas?.__SSC_LogError(err);
                throw new Exception(err);
            }

            try
            {
                strch3 = Apas.__SSC_ReadVariable(var2);
            }
            catch (NullReferenceException)
            {
                var err = $"无法找到变量[{var2}]";
                Apas?.__SSC_LogError(err);
                throw new Exception(err);
            }

            if (double.TryParse(strch0.ToString(), out double ch0) == false)
                throw new Exception($"读取变量[{var1}]时发生错误。");
            else if (double.TryParse(strch3.ToString(), out double ch3) == false)
                throw new Exception($"读取变量[{var2}]时发生错误。");
            else
            {
                var angle = (ch0 - ch3) / 3 / (opts.Coeff * opts.Pitch);
                Apas.__SSC_WriteVariable(opts.PrefixVarWrite, angle);
            }
        }
        #endregion

        #region Private Methods

        // Given a document, a worksheet name, a column name, and a one-based row index,
        // deletes the text from the cell at the specified column and row on the specified worksheet.
        public static double CalculateAngleFromExcel(string docName, string sheetName, double X1, double X4)
        {
            // Open the document for editing.
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(docName, true))
            {
                IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == sheetName);
                if (sheets.Count() == 0)
                {
                    throw new System.Exception("The specified worksheet does not exist.");
                }
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(relationshipId);

                // Get the cell at the specified column and row.
                string colName = "C";
                uint rowIndex = 6;
                Cell cell = GetSpreadsheetCell(worksheetPart.Worksheet, colName, rowIndex);
                if (cell == null)
                {
                    throw new System.Exception($"The specified cell [{colName}{rowIndex}] does not exist.");
                }

                cell.CellValue = new CellValue(X1.ToString());

                rowIndex = 9;
                cell = GetSpreadsheetCell(worksheetPart.Worksheet, colName, rowIndex);
                if (cell == null)
                {
                    throw new System.Exception($"The specified cell [{colName}{rowIndex}] does not exist.");
                }
                cell.CellValue = new CellValue(X4.ToString());

                worksheetPart.Worksheet.Save();

                // return the angle.
                colName = "G";
                rowIndex = 6;
                cell = GetSpreadsheetCell(worksheetPart.Worksheet, colName, rowIndex);
                if (cell == null)
                {
                    throw new System.Exception($"The specified cell [{colName}{rowIndex}] does not exist.");
                }

                var cellval = cell.CellValue.Text;

                if (double.TryParse(cellval.ToString(), out double angle) == false)
                    throw new System.Exception($"unable to convert the cell {colName}{rowIndex} value {cellval} to angle.");
                else
                {
                    Debug.WriteLine($"Predicated Angle: {angle}°");
                    return angle;
                }

                
            }
        }

        // Given a worksheet, a column name, and a row index, gets the cell at the specified column and row.
        private static Cell GetSpreadsheetCell(Worksheet worksheet, string columnName, uint rowIndex)
        {
            IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Elements<Row>().Where(r => r.RowIndex == rowIndex);
            if (rows.Count() == 0)
            {
                // A cell does not exist at the specified row.
                return null;
            }

            IEnumerable<Cell> cells = rows.First().Elements<Cell>().Where(c => string.Compare(c.CellReference.Value, columnName + rowIndex, true) == 0);
            if (cells.Count() == 0)
            {
                // A cell does not exist at the specified column, in the specified row.
                return null;
            }

            return cells.First();
        }

        #endregion

    }
}
