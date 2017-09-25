using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using ClosedXML.Excel;
using PDR.Domain.Helpers;
using PDR.Domain.Model;

namespace PDR.Domain.Services.XLS
{
    public class InvoiceToXLSConverter
    {
        public InvoiceToXLSConverter()
        {
        }

        public InvoiceToXLSConverter(IEnumerable<Invoice> invoices)
        {
            this.Invoices = invoices;
        }

        public IEnumerable<Invoice> Invoices { get; set; }

        public string Create()
        {
            var newFile = new FileInfo(HostingEnvironment.MapPath("~/Content/invoice.xlsx"));
         
            var files = Directory.GetFiles(newFile.DirectoryName, "*M.xlsx");
            foreach (var file in files)
            {
                File.Delete(file);
            }

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("Invoices");

            this.RenderHeader(ws);
            this.RenderBodyTable(ws, this.Invoices);

            ws.Columns().AdjustToContents();
            wb.SaveAs(newFile.FullName);
            var name = newFile.FullName;
            
            var startIndex = name.IndexOf(".xlsx");
            var tempfile = name.Insert(startIndex, DateTime.Now.ToString("MM-dd-yyyy_hh-mm-ss_tt"));
            File.Copy(newFile.FullName, tempfile);
            var filename = tempfile.Substring(tempfile.LastIndexOf('\\') + 1);
            return filename;
        }

        private void RenderHeader(IXLWorksheet ws)
        {
            ws.Cell(1, 1).Value = "ACCOUNT";
            ws.Cell(1, 2).Value = "ACCOUNT RECEIVABLE";
            ws.Cell(1, 3).Value = "NAME";
            ws.Cell(1, 4).Value = "NUMBER";
            ws.Cell(1, 5).Value = "ITEM";
            ws.Cell(1, 6).Value = "DESCRIPTION";
            ws.Cell(1, 7).Value = "RATE";
            ws.Cell(1, 8).Value = "AMOUNT";
            ws.Cell(1, 9).Value = "INVOICEDATE";
            ws.Cell(1, 10).Value = "MEMO";
            
            ws.Cell(1, 10).AsRange().AddToNamed("Titles");
            ws.Range(1, 1, 1, 9).Style.Fill.BackgroundColor = XLColor.LightGreen;
            ws.Cell(1, 10).Style.Fill.BackgroundColor = XLColor.LightYellow;
            ws.Column(7).Style.NumberFormat.NumberFormatId = 2;
            ws.Column(8).Style.NumberFormat.NumberFormatId = 2;
        }

        private void RenderBodyTable(IXLWorksheet ws, IEnumerable<Invoice> invoices)
        {
            const int START_ROW = 2;

            ws.Cell(START_ROW, 1).Value = invoices.Select(x => new
                                                           {
                                                               ACCOUNT = "SALES",
                                                               ACCOUNT_RECEIVABLE = "ACCOUNTS RECEIVABLE",
                                                               NAME = x.Customer.GetCustomerName().ToUpper(),
                                                               NUMBER = x.Id.ToString().ToUpper(),
                                                               ITEM = "PAINTLESS DENT REPAIR",
                                                               DESCRIPTION = string.Format(
                                                                    "{0} {1} {2} - {3}",
                                                                    x.RepairOrder.Estimate.Car.Year,
                                                                    x.RepairOrder.Estimate.Car.Make,
                                                                    x.RepairOrder.Estimate.Car.Model,
                                                                    x.RepairOrder.Estimate.Car.VIN).ToUpper(),
                                                               RATE = x.InvoiceSum,
                                                               AMOUNT = x.InvoiceSum,
                                                               INVOICEDATE = x.CreationDate.ToString("MM/dd/yyyy"),
                                                               MEMO = string.Format("RO: {0}", x.RepairOrder.Estimate.Car.CustRO != null ? x.RepairOrder.Estimate.Car.CustRO.ToUpper() : string.Empty)
                                                           }).AsEnumerable();
        }
    }
}
