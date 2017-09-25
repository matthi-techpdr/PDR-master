using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;

namespace PDR.Domain.Services.XLS
{
    internal class MatrixToXLSConverter
    {
        public byte[] Create(Matrix matrix)
        {
            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add(matrix.Name);
            ws.Cell(1, 1).Value = matrix.Name;

            this.RenderTemplate(ws);
            this.RenderData(ws, matrix);
            this.RenderFooter(ws, matrix);


            this.DefineStyle(ws);
            var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        }

        private void RenderData(IXLWorksheet ws, Matrix matrix)
        {
            var allParts = matrix.MatrixPrices.Select(x => x.PartOfBody)
                .Except(new[] { PartOfBody.RearBumper, PartOfBody.FrontBumper })
                .Distinct();
            int i = 3;
            foreach (var part in allParts)
            {
                var partPrices =
                    matrix.MatrixPrices.Where(x => x.PartOfBody == part).GroupBy(x => x.TotalDents).Select(
                            x => new OrderHelper
                            {
                                DIME = x.SingleOrDefault(s => s.AverageSize == AverageSize.DIME),
                                NKL = x.SingleOrDefault(s => s.AverageSize == AverageSize.NKL),
                                QTR = x.SingleOrDefault(s => s.AverageSize == AverageSize.QTR),
                                HALF = x.SingleOrDefault(s => s.AverageSize == AverageSize.HALF)
                            }.ToOrderedArray()).SelectMany(x => x).ToList();

                var row = new RowViewModel { Part = part.ToString(), Prices = partPrices };

                ws.Cell(i, 1).Value = row.Part;
                var j = 2;
                int i1 = i;
                row.Prices.ForEach(
                    x =>
                    {
                        ws.Cell(i1, j).Value = x;
                        j++;
                    });
                i++;
            }
        }

        private void RenderTemplate(IXLWorksheet ws)
        {
            ws.Cell(1, 1);

            ws.Cell(2, 1).Value = string.Empty;

            int j = 2;
            int z = 0;
            while (z < 10)
            {
                foreach (var size in Enum.GetValues(typeof(AverageSize)))
                {
                    ws.Cell(2, j).Value = size;
                    j++;
                }

                z++;
            }

            ws.Cell(1, 2).Value = "VERY LIGHT[1...5]";
            ws.Range(1, 2, 1, 5).Merge();

            ws.Cell(1, 6).Value = "LIGHT[6...15]";
            ws.Range(1, 6, 1, 9).Merge();

            ws.Cell(1, 10).Value = "MODERATE[16...30]";
            ws.Range(1, 10, 1, 13).Merge();

            ws.Cell(1, 14).Value = "MEDIUM[31...50]";
            ws.Range(1, 14, 1, 17).Merge();

            ws.Cell(1, 18).Value = "HEAVY[51...75]";
            ws.Range(1, 18, 1, 21).Merge();

            ws.Cell(1, 22).Value = "SEVERE[76...100]";
            ws.Range(1, 22, 1, 25).Merge();

            ws.Cell(1, 26).Value = "EXTREME[101...150]";
            ws.Range(1, 26, 1, 29).Merge();

            ws.Cell(1, 30).Value = "LIMIT[151...200]";
            ws.Range(1, 30, 1, 33).Merge();

            ws.Cell(1, 34).Value = "CEILING[201...250]";
            ws.Range(1, 34, 1, 37).Merge();

            ws.Cell(1, 38).Value = "MAX[251...300]";
            ws.Range(1, 38, 1, 41).Merge();

        }

        private void DefineStyle(IXLWorksheet ws)
        {
            var table = ws.Range("A1:AO19");
            this.SetTable(table);

            var footer = ws.Range("A21:H25");
            this.SetTable(footer);

            var tableHeader = ws.Range("A1:AO1");
            this.SetHeader(tableHeader);

            var footerHeader = ws.Range("A21:H21");
            this.SetHeader(footerHeader);

            ws.Columns().AdjustToContents();
        }

        private void RenderFooter(IXLWorksheet ws, Matrix matrix)
        {
            ws.Cell(21, 1).Value = "Up-charges";
            ws.Range(21, 1, 21, 4).Merge();

            ws.Cell(22, 1).Value = "Aluminium panels, %";
            ws.Cell(22, 4).Value = matrix.AluminiumPanel;
            ws.Range(22, 1, 22, 3).Merge();

            ws.Cell(23, 1).Value = "Double layered panels, %";
            ws.Cell(23, 4).Value = matrix.DoubleLayeredPanels;
            ws.Range(23, 1, 23, 3).Merge();

            ws.Cell(24, 1).Value = "SUV(oversized roof), %";
            ws.Cell(24, 4).Value = matrix.OversizedRoof;
            ws.Range(24, 1, 24, 3).Merge();

            ws.Cell(25, 1).Value = "Maximum, %";
            ws.Cell(25, 4).Value = matrix.Maximum;
            ws.Range(25, 1, 25, 3).Merge();

            ws.Cell(21, 5).Value = "Corrosion protection";
            ws.Range(21, 5, 21, 8).Merge();

            ws.Cell(22, 5).Value = "Per body part, $";
            ws.Cell(22, 8).Value = matrix.CorrosionProtectionPart;
            ws.Range(22, 5, 22, 7).Merge();

            ws.Cell(23, 5).Value = "Per whole car, $";
            ws.Cell(23, 8).Value = matrix.MaxCorrosionProtection;
            ws.Range(23, 5, 23, 7).Merge();

            ws.Cell(24, 5).Value = "Oversized dents, $";
            ws.Cell(24, 8).Value = matrix.OversizedDents;
            ws.Range(24, 5, 24, 7).Merge();
        }

        private void SetTable(IXLRange table)
        {
            table.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            table.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            table.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        private void SetHeader(IXLRange header)
        {
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;
            header.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            header.Style.Border.InsideBorderColor = XLColor.Black;
        }

        private class RowViewModel
        {
            public string Part { get; set; }

            public IEnumerable<string> Prices { get; set; }
        }
    }
}
