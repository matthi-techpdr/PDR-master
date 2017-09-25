using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Services.XLS
{
    internal class XLSToMatrixConverter
    {
        private readonly Matrix matrix;

        private readonly SpreadsheetDocument document;

        private IEnumerable<Row> rows;

        private static TotalDents TotalDentsResolver(int column)
        {
            if (column >= 2 && column <= 5)
            {
                return TotalDents.To5;
            }

            if (column >= 6 && column <= 9)
            {
                return TotalDents.To15;
            }

            if (column >= 10 && column <= 13)
            {
                return TotalDents.To30;
            }

            if (column >= 14 && column <= 17)
            {
                return TotalDents.To50;
            }

            if (column >= 18 && column <= 21)
            {
                return TotalDents.To75;
            }

            if (column >= 22 && column <= 25)
            {
                return TotalDents.To100;
            }

            if (column >= 26 && column <= 29)
            {
                return TotalDents.To150;
            }

            if (column >= 30 && column <= 33)
            {
                return TotalDents.To200;
            }

            if (column >= 34 && column <= 37)
            {
                return TotalDents.To250;
            }

            if (column >= 38 && column <= 41)
            {
                return TotalDents.To300;
            }

            throw new Exception("Wrong table format");
        }

        private void Init()
        {
            IEnumerable<Sheet> sheets = this.document.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
            string relationshipId = sheets.First().Id.Value;
            var worksheetPart = (WorksheetPart)this.document.WorkbookPart.GetPartById(relationshipId);
            var workSheet = worksheetPart.Worksheet;
            var sheetData = workSheet.GetFirstChild<SheetData>();
            this.rows = sheetData.Descendants<Row>().ToList();
        }

        public XLSToMatrixConverter(string xlsFilePath, Matrix matrix)
        {
            this.document = SpreadsheetDocument.Open(xlsFilePath, false);
            this.Init();
            this.matrix = matrix;
        }

        public XLSToMatrixConverter(Stream stream, Matrix matrix)
        {
            this.document = SpreadsheetDocument.Open(stream, false);
            this.Init();
            this.matrix = matrix;
        }

        public Matrix Import(Admin newAdmin = null)
        {
            this.GetMatrixPrices(newAdmin);
            this.GetMatrixProperties();
            this.document.Dispose();
            return this.matrix;
        }

        private void GetMatrixPrices(Admin newAdmin = null)
        {
            for (var i = 2; i < 19; i++)
            {
                var currentRow = this.rows.ElementAt(i);
                var currentBodyPart = (PartOfBody)Enum.Parse(typeof(PartOfBody), this.GetCellValue(currentRow.ElementAt(0) as Cell), true);
                for (int j = 1; j < 41; j++)
                {
                    var currentCell = currentRow.ElementAt(j) as Cell;

                    if (currentCell != null && currentCell.CellValue != null)
                    {
                        var cost = Convert.ToDouble(this.GetDoubleCellValue(currentCell));
                        var averageSize =
                            (AverageSize)
                            Enum.Parse(typeof(AverageSize), this.GetCellValue(this.rows.ElementAt(1).ElementAt(j) as Cell));
                        var totalDents = TotalDentsResolver(j + 1);

                        var priceMatrix = new MatrixPrice(true, newAdmin)
                        {
                            Cost = cost,
                            PartOfBody = currentBodyPart,
                            AverageSize = averageSize,
                            TotalDents = totalDents
                        };

                        this.matrix.AddMatrixPrice(priceMatrix);
                    }
                }
            }
        }

        private void GetMatrixProperties()
        {
            var aluminiumCellValue = this.GetCellValue(this.rows.ElementAt(20).ElementAt(3) as Cell);
            this.matrix.AluminiumPanel = Convert.ToInt16(aluminiumCellValue);

            var doblePanelCellValue = this.GetCellValue(this.rows.ElementAt(21).ElementAt(3) as Cell);
            this.matrix.DoubleLayeredPanels = Convert.ToInt16(doblePanelCellValue);

            var suv = this.GetCellValue(this.rows.ElementAt(22).ElementAt(3) as Cell);
            this.matrix.OversizedRoof = Convert.ToInt16(suv);

            var max = this.GetCellValue(this.rows.ElementAt(23).ElementAt(3) as Cell);
            this.matrix.Maximum = Convert.ToInt16(max);

            var cpPart = this.GetCellValue(this.rows.ElementAt(20).ElementAt(7) as Cell);
            this.matrix.CorrosionProtectionPart = Convert.ToDouble(cpPart);

            var cpMax = this.GetCellValue(this.rows.ElementAt(21).ElementAt(7) as Cell);
            this.matrix.MaxCorrosionProtection = Convert.ToDouble(cpMax);

            var oversizedDents = this.GetCellValue(this.rows.ElementAt(22).ElementAt(7) as Cell);
            this.matrix.OversizedDents = Convert.ToDouble(oversizedDents);
            
            var name = this.GetCellValue(this.rows.ElementAt(0).ElementAt(0) as Cell);
            this.matrix.Name = name;
        }

        private string GetCellValue(Cell cell)
        {
            var stringTablePart = this.document.WorkbookPart.SharedStringTablePart;
            var value = cell.CellValue;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[int.Parse(value.InnerXml)].InnerText;
            }

            return value.InnerXml;
        }

        private double GetDoubleCellValue(Cell cell)
        {
            double doubleValue;
            var val = this.GetCellValue(cell);
            if (double.TryParse(val, out doubleValue))
            {
                return doubleValue;
            }

            throw new Exception();
        }
    }
}
