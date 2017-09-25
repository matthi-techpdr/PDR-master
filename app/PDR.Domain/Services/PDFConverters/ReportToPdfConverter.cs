using System;
using System.IO;
using System.Linq;

using Microsoft.Win32;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;

namespace PDR.Domain.Services.PDFConverters
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;

    public class ReportToPdfConverter
    {
        private readonly Document document;

        private readonly DataForReports data;

        private Table table;

        public ReportToPdfConverter(DataForReports data) : this()
        {
            this.data = data;
        }
        
        protected ReportToPdfConverter()
        {
            this.document = new Document();
        }

        public int TotalAmountRows { get; private set; }

        public int CurrentIndexRow { get; private set; }

        public int CurrentAmountRowsAtLeast { get; private set; }

        public double TotalAmount { get; private set; }

        public double PaidAmount { get; private set; }

        public double Commission { get; private set; }

        public bool IsLastPage { get; private set; }

        public double HeightRows { get; private set; }

        public Document CreateDocument(Company company)
        {
            this.DocumentInitialize();

            //switch (this.data.EntityType)
            //{
            //    case "Estimate":
            //        this.EstimateDocumentInitialize();
            //        break;
            //    case "RepairOrder":
            //        this.RepairOrderDocumentInitialize();
            //        break;
            //    case "Invoice":
            //        this.InvoiceDocumentInitialize();
            //        break;
            //}

            this.DefineStyles();

            this.TotalAmountRows = this.data.Entities.Count();
            this.CurrentIndexRow = 0;
            this.IsLastPage = false;

            while (this.CurrentIndexRow < this.TotalAmountRows)
            {
                this.CreatePage(company);
            }

            if (this.TotalAmountRows == 0)
            {
                this.CreatePage(company);
            }
            
            this.NumerationPages();

            return this.document;
        }

        private void DocumentInitialize()
        {
            this.document.Info.Title = this.data.EntityType + " report";
            this.document.Info.Subject = "Show " + this.data.EntityType + " report information";
            this.document.Info.Author = this.data.Employee.Name;
        }

        private string GetTemporaryFile(byte[] file, string contentType)
        {
            var temp = Path.GetTempPath();
            var path = temp + Guid.NewGuid() + this.GetDefaultExtension(contentType);
            using (var fileStream = File.Create(path))
            {
                fileStream.Write(file, 0, file.Length);
            }

            return path;
        }

        private string GetDefaultExtension(string mimeType)
        {
            var key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
            var value = key != null ? key.GetValue("Extension", null) : null;
            var result = value != null ? value.ToString() : string.Empty;

            return result;
        }

        #region Create Header

        private void CreateLogoFrame(Section section, Company company)
        {
            Image logo = section.Headers.Primary.AddImage(this.GetTemporaryFile(company.Logo.PhotoFull, company.Logo.ContentType));
            logo.Height = "2.5cm";
            logo.LockAspectRatio = true;
            logo.RelativeVertical = RelativeVertical.Line;
            logo.RelativeHorizontal = RelativeHorizontal.Margin;
            logo.Top = ShapePosition.Top;
            logo.Left = ShapePosition.Left;
            logo.WrapFormat.Style = WrapStyle.Through;

            this.CreateCompanyFrame(section, company);
        }

        private void CreateCompanyFrame(Section section, Company company)
        {
            var addressFrame = section.AddTextFrame();
            addressFrame.Height = "3.0cm";
            addressFrame.Width = "7.0cm";
            addressFrame.Left = ShapePosition.Left;
            addressFrame.Top = ShapePosition.Top;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            addressFrame.Left = "3.5cm";
            addressFrame.Top = "1.25cm";
            addressFrame.RelativeVertical = RelativeVertical.Page;

            var cityCompany = string.IsNullOrWhiteSpace(company.City)
                           ? string.Empty
                           : company.City + ",";
            var stateCompany = company.State;
            var zipCompany = string.IsNullOrWhiteSpace(company.Zip) ? string.Empty : company.Zip;

            var addressParagraph = addressFrame.AddParagraph();
            addressParagraph.Format.Font.Name = "Times New Roman";
            addressParagraph.Format.Font.Size = 10.5;
            addressParagraph.Format.SpaceAfter = 5;
            var companyName = addressParagraph.AddFormattedText(company.Name);
            companyName.Font.Size = 11;
            companyName.Font.Bold = true;

            addressParagraph.AddLineBreak();
            addressParagraph.AddText(string.IsNullOrWhiteSpace(company.Address1) ? string.Empty : company.Address1);
            addressParagraph.AddLineBreak();
            if (!string.IsNullOrWhiteSpace(company.Address2))
            {
                addressParagraph.AddText(company.Address2);
                addressParagraph.AddLineBreak();
            }

            addressParagraph.AddText(string.Format("{0} {1} {2}", cityCompany, stateCompany, zipCompany));
            addressParagraph.AddLineBreak();
            addressParagraph.AddText(string.Format("Phone: {0}", company.PhoneNumber.ToPhoneFormat()));
            addressParagraph.AddLineBreak();
            addressParagraph.AddText(string.Format("E-mail: {0}", company.Email));
            addressParagraph.AddLineBreak();
        }

        private void CreateFilterFrame(Section section)
        {
            var filterParagraph = section.AddParagraph();
            filterParagraph.Format.LeftIndent = "12cm";
            filterParagraph.Format.SpaceBefore = "-0.4cm";

            filterParagraph.Format.Font.Size = 10.5;
            filterParagraph.Format.Font.Name = "Times New Roman";
            filterParagraph.AddText(string.Format("From: {0}", this.data.DateFrom));
            filterParagraph.AddLineBreak();
            filterParagraph.AddText(string.Format("To: {0}", this.data.DateTo));

            filterParagraph.Format.Alignment = ParagraphAlignment.Left;
            filterParagraph.AddLineBreak();
            filterParagraph.AddText(string.Format("Customers: {0}", this.data.Customer));

            if (this.data.Employee.Role == UserRoles.Manager
                || this.data.Employee.Role == UserRoles.Admin)
            {
                filterParagraph.AddLineBreak();
                filterParagraph.AddText(string.Format("Teams: {0}", this.data.Team));
            }
        }

        private void NumerationPages()
        {
            if (this.document.Sections.Count != 1)
            {
                for (var i = 0; i < this.document.Sections.Count; i++)
                {
                    var header = this.document.Sections[i].Headers.Primary;
                    header.Format.SpaceAfter = "-1cm";
                    var pageLabel = header.AddParagraph();
                    pageLabel.Format.Font.Name = "Times New Roman";
                    pageLabel.Format.Alignment = ParagraphAlignment.Right;
                    pageLabel.AddPageField();
                    pageLabel.AddText(string.Format(" of {0}", header.Document.Sections.Count));
                }
            }
        }

        #endregion

        private void CreatePage(Company company)
        {
            Section section = this.document.AddSection();
            section.PageSetup.RightMargin = "1cm";
            section.PageSetup.FooterDistance = "1cm";
            
            this.CreateLogoFrame(section, company);

            Paragraph paragraph = section.Headers.Primary.AddParagraph();
            paragraph.Format.Font.Name = "Times New Roman";
            paragraph.Format.LeftIndent = "9cm";
            paragraph.AddText(this.data.EntityType != "RepairOrder" ? this.data.EntityType + " report" : "Repair order report");

            paragraph.Format.Font.Size = 12;
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            this.CreateFilterFrame(section);

            var bodyPartsTableFrame = section.AddTextFrame();
            bodyPartsTableFrame.Top = "1.5cm";
            this.table = bodyPartsTableFrame.AddTable();
            
            switch (this.data.EntityType)
            {
                case "Estimate":
                    this.EstimateRenderPartsTable();
                    break;
                case "RepairOrder":
                    this.RepairOrderRenderPartsTable();
                    break;
                case "Invoice":
                    this.InvoiceRenderPartsTable();
                    break;
            }
        }

        #region Invoice Entity

        private void InvoiceRenderPartsTable()
        {
            table.Style = "PartTable";
            table.Format.Alignment = ParagraphAlignment.Center;
            table.Rows.LeftIndent = 0;
            table.TopPadding = 2;
            table.BottomPadding = 2;
            table.Rows.LeftIndent = 0;

            table.AddColumn().Width = "0.5cm";
            table.AddColumn().Width = "2.5cm";
            table.AddColumn().Width = "1.75cm";
            table.AddColumn().Width = "3.5cm";
            if (this.data.Employee.Role == UserRoles.Manager)
            {
                table.AddColumn().Width = "2cm";
                table.AddColumn().Width = "2cm";
                table.AddColumn().Width = "1.5cm";
                if (this.data.Commission)
                {
                    table.AddColumn().Width = "2cm";
                }
            }
            else
            {
                table.AddColumn().Width = "2cm";
                table.AddColumn().Width = "2cm";
                if (this.data.Commission)
                {
                    table.AddColumn().Width = "2cm";
                }
            }

            this.table.AddColumn();

            Row headerRow = this.table.AddRow();
            headerRow.Cells[0].AddParagraph(string.Empty);
            headerRow.Cells[1].AddParagraph("Creation date");
            headerRow.Cells[2].AddParagraph("Invoice ID");
            headerRow.Cells[3].AddParagraph("Customer name");
            headerRow.Cells[4].AddParagraph("Invoice amount");
            headerRow.Cells[5].AddParagraph("Paid amount");
            headerRow.Cells[6].AddParagraph("Status");
            if (this.data.Employee.Role == UserRoles.Manager)
            {
                headerRow.Cells[7].AddParagraph("Employee");
                if (this.data.Commission)
                {
                    headerRow.Cells[8].AddParagraph("Commission");
                }
            }
            else
            {
                if (this.data.Commission)
                {
                    headerRow.Cells[7].AddParagraph("Commission");
                }
            }

            this.SetBordersForPartHeader(headerRow);

            this.InvoiceFillContent();
        }

        private void InvoiceFillContent()
        {
            double invoiceSum = 0;
            double paidSum = 0;
            double comission = 0;
            int count = this.CurrentIndexRow == 0 ? 1 : this.CurrentIndexRow + 1;
            this.CurrentAmountRowsAtLeast = 0;

            var list =
                this.data.Entities.Select(companyEntity => companyEntity as Invoice).ToList()
                .OrderByDescending(x => x.CreationDate).ToList();

            XFont font = new XFont("Times New Roman", 12);
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            var pdfGfx = XGraphics.FromPdfPage(page);

            while (this.CurrentAmountRowsAtLeast < 36)
            {
                if (this.CurrentIndexRow == this.TotalAmountRows)
                {
                    this.IsLastPage = true;
                    break;
                }

                var row = this.table.AddRow();

                var invoice = list[this.CurrentIndexRow];
                if (this.CurrentIndexRow % 2 == 0)
                {
                    row.Shading.Color = Colors.LightGray;
                }

                row.Cells[0].AddParagraph(count ++.ToString());
                row.Cells[1].AddParagraph(invoice.CreationDate.ToString("MM/dd/yyyy"));
                row.Cells[2].AddParagraph(invoice.Id.ToString());
                row.Cells[3].AddParagraph(invoice.Customer == null ? string.Empty : invoice.Customer.GetCustomerName());
                row.Cells[4].AddParagraph(string.Format("${0:F2}", Math.Round(invoice.InvoiceSum, 2)));
                row.Cells[5].AddParagraph(string.Format("${0:F2}", Math.Round(invoice.PaidSum, 2)));
                row.Cells[6].AddParagraph(invoice.Status.ToString());

                var countLinesForCustomer = GetCountTextLines(pdfGfx, font, row.Cells[3], invoice.Customer.GetCustomerName());
                var countLines = countLinesForCustomer;

                if (this.data.Employee.Role == UserRoles.Manager)
                {
                    row.Cells[7].AddParagraph(string.Join(",\n", invoice.RepairOrder.TeamEmployeePercents.Select(x => x.TeamEmployee.Name)));
                    if (this.data.Commission)
                    {
                        row.Cells[8].AddParagraph(string.Format("${0:F2}", invoice.GetCommission()));
                        comission += invoice.GetCommission();
                    }

                    var teamEmployees = invoice.RepairOrder.TeamEmployeePercents.Select(x => x.TeamEmployee.Name).ToList();
                    var countLinesForEmployees = 0;
                    foreach (var teamEmployee in teamEmployees)
                    {
                        countLinesForEmployees += GetCountTextLines(pdfGfx, font, row.Cells[7], teamEmployee);
                    }

                    countLines = Math.Max(countLinesForCustomer, countLinesForEmployees);
                }
                else
                {
                    if (this.data.Commission)
                    {
                        row.Cells[7].AddParagraph(string.Format("${0:F2}", invoice.GetCommission()));
                        comission += invoice.GetCommission();
                    }
                }
                this.CurrentAmountRowsAtLeast += countLines;

                invoiceSum += invoice.InvoiceSum;
                paidSum += invoice.PaidSum;
                this.CurrentIndexRow ++;
            }

            this.TotalAmount += invoiceSum;
            this.PaidAmount += paidSum;
            this.Commission += comission;

            if (!this.IsLastPage)
            {
                Row grandTotal = this.table.AddRow();
                grandTotal.Cells[0].MergeRight = 3;
                grandTotal.Cells[0].AddParagraph("Total page").Format.Alignment = ParagraphAlignment.Center;
                grandTotal.Cells[4].AddParagraph(string.Format("${0:F2}", Math.Round(invoiceSum, 2)));
                grandTotal.Cells[5].AddParagraph(string.Format("${0:F2}", Math.Round(paidSum, 2)));
                grandTotal.Cells[6].AddParagraph(string.Empty);
                if (this.data.Employee.Role == UserRoles.Manager)
                {
                    grandTotal.Cells[7].AddParagraph(string.Empty);
                    if (this.data.Commission)
                    {
                        grandTotal.Cells[8].AddParagraph(string.Format("${0:F2}", comission));
                    }
                }
                else
                {
                    if (this.data.Commission)
                    {
                        grandTotal.Cells[7].AddParagraph(string.Format("${0:F2}", comission));
                    }
                }

                this.SetBordersForPartHeader(grandTotal);
            }

            if (this.IsLastPage)
            {
                this.SetTotalInvoice();
            }
        }

        private void SetTotalInvoice()
        {
            Row grandTotal = this.table.AddRow();
            grandTotal.Cells[0].MergeRight = 3;
            grandTotal.Cells[0].AddParagraph("TOTAL").Format.Alignment = ParagraphAlignment.Center;
            grandTotal.Cells[4].AddParagraph(string.Format("${0:F2}", Math.Round(this.TotalAmount, 2)));
            grandTotal.Cells[5].AddParagraph(string.Format("${0:F2}", Math.Round(this.PaidAmount, 2)));
            grandTotal.Cells[6].AddParagraph(string.Empty);
            if (this.data.Employee.Role == UserRoles.Manager)
            {
                grandTotal.Cells[7].AddParagraph(string.Empty);
                if (this.data.Commission)
                {
                    grandTotal.Cells[8].AddParagraph(string.Format("${0:F2}", this.Commission));
                }
            }
            else
            {
                if (this.data.Commission)
                {
                    grandTotal.Cells[7].AddParagraph(string.Format("${0:F2}", this.Commission));
                }
            }

            this.SetBordersForPartHeader(grandTotal);
        }

        #endregion

        #region Repair Order Entity

        private void RepairOrderRenderPartsTable()
        {
            this.table.Style = "PartTable";
            this.table.Format.Alignment = ParagraphAlignment.Center;
            this.table.Rows.LeftIndent = 0;
            this.table.TopPadding = 2;
            this.table.BottomPadding = 2;
            this.table.Rows.LeftIndent = 0;

            this.table.AddColumn().Width = "0.6cm";
            this.table.AddColumn().Width = "3cm";
            this.table.AddColumn().Width = "3cm";
            this.table.AddColumn().Width = "4cm";
            this.table.AddColumn().Width = "3cm";
            
            if (this.data.Employee.Role == UserRoles.Manager)
            {
                this.table.AddColumn().Width = "2cm";
            }

            this.table.AddColumn();

            Row headerRow = this.table.AddRow();

            headerRow.Cells[0].AddParagraph(string.Empty);
            headerRow.Cells[1].AddParagraph("Creation date");
            headerRow.Cells[2].AddParagraph("Repair order ID");
            headerRow.Cells[3].AddParagraph("Customer name");
            headerRow.Cells[4].AddParagraph("Total amount");
            headerRow.Cells[5].AddParagraph("Status");
            if (this.data.Employee.Role == UserRoles.Manager)
            {
                headerRow.Cells[6].AddParagraph("Employee");
            }

            this.SetBordersForPartHeader(headerRow);

            this.RepairOrderFillContent();
        }

        private void RepairOrderFillContent()
        {
            double totalSum = 0;
            int count = this.CurrentIndexRow == 0 ? 1 : this.CurrentIndexRow + 1;
            this.CurrentAmountRowsAtLeast = 0;
            

            var list =
                this.data.Entities.Select(companyEntity => companyEntity as RepairOrder).ToList().OrderByDescending(
                    x => x.CreationDate).ToList();

            XFont font = new XFont("Times New Roman", 12);
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            var pdfGfx = XGraphics.FromPdfPage(page);

            while (this.CurrentAmountRowsAtLeast <= 36)
            {
                if (this.CurrentIndexRow == this.TotalAmountRows)
                {
                    this.IsLastPage = true;
                    break;
                }

                var row = this.table.AddRow();
                var ro = list[this.CurrentIndexRow];
                if (this.CurrentIndexRow % 2 == 0)
                {
                    row.Shading.Color = Colors.LightGray;
                }

                row.Cells[0].AddParagraph(count ++.ToString());
                row.Cells[1].AddParagraph(ro.CreationDate.ToString("MM/dd/yyyy"));
                row.Cells[2].AddParagraph(ro.Id.ToString());
                row.Cells[3].AddParagraph(ro.Customer == null ? string.Empty : ro.Customer.GetCustomerName());
                row.Cells[4].AddParagraph(string.Format("${0:F2}", ro.TotalAmount));
                row.Cells[5].AddParagraph(ro.RepairOrderStatus.ToString());

                var countLinesForCustomer = GetCountTextLines(pdfGfx, font, row.Cells[3], ro.Customer.GetCustomerName());
                var countLines = countLinesForCustomer;
                
                if (this.data.Employee.Role == UserRoles.Manager)
                {
                    row.Cells[6].AddParagraph(string.Join(",\n", ro.TeamEmployeePercents.Select(x => x.TeamEmployee.Name)));
                    var teamEmployees = ro.TeamEmployeePercents.Select(x => x.TeamEmployee.Name).ToList();
                    var countLinesForEmployees = 0;
                    foreach (var teamEmployee in teamEmployees)
                    {
                        countLinesForEmployees += GetCountTextLines(pdfGfx, font, row.Cells[6], teamEmployee);
                    }

                    countLines = Math.Max(countLinesForCustomer, countLinesForEmployees);
                }
                this.CurrentAmountRowsAtLeast += countLines;

                totalSum += ro.TotalAmount;
                this.CurrentIndexRow++;
            }

            this.TotalAmount += totalSum;

            if (!this.IsLastPage)
            {
                Row grandTotal = this.table.AddRow();
                grandTotal.Cells[0].AddParagraph(string.Empty);
                grandTotal.Cells[1].AddParagraph(string.Empty);
                grandTotal.Cells[2].AddParagraph("Total page").Format.Alignment = ParagraphAlignment.Center;
                grandTotal.Cells[3].AddParagraph(string.Empty);
                grandTotal.Cells[4].AddParagraph(string.Format("${0:F2}", totalSum));
                grandTotal.Cells[5].AddParagraph(string.Empty);
                if (this.data.Employee.Role == UserRoles.Manager)
                {
                    grandTotal.Cells[6].AddParagraph(string.Empty);
                }

                this.SetBordersForPartHeader(grandTotal);
            }

            if (this.IsLastPage)
            {
                this.SetTotalRepairOrder();
            }
        }

        private void SetTotalRepairOrder()
        {
            Row grandTotal = this.table.AddRow();
            grandTotal.Cells[0].AddParagraph(string.Empty);
            grandTotal.Cells[1].AddParagraph(string.Empty);
            grandTotal.Cells[2].AddParagraph("TOTAL").Format.Alignment = ParagraphAlignment.Center;
            grandTotal.Cells[3].AddParagraph(string.Empty);
            grandTotal.Cells[4].AddParagraph(string.Format("${0:F2}", this.TotalAmount));
            grandTotal.Cells[5].AddParagraph(string.Empty);
            if (this.data.Employee.Role == UserRoles.Manager)
            {
                grandTotal.Cells[6].AddParagraph(string.Empty);
            }

            SetBordersForPartHeader(grandTotal);
        }

        #endregion

        #region Estimate entity

        private void EstimateRenderPartsTable()
        {
            table.Style = "PartTable";
            table.Format.Alignment = ParagraphAlignment.Center;
            table.Rows.LeftIndent = 0;
            table.TopPadding = 2;
            table.BottomPadding = 2;
            table.RightPadding = 1;
            table.Rows.LeftIndent = 0;
            
            this.table.AddColumn().Width = "0.7cm";
            this.table.AddColumn().Width = "3cm";
            this.table.AddColumn().Width = "2.5cm";
            this.table.AddColumn().Width = "4cm";
            this.table.AddColumn().Width = "2.5cm";
            this.table.AddColumn().Width = "2.5cm";

            if (this.data.Employee.Role == UserRoles.Manager)
            {
                this.table.AddColumn().Width = "2cm";
            }

            this.table.AddColumn().Width = "0.5cm";


            Row headerRow = this.table.AddRow();
            headerRow.Height = "0.5cm";
            headerRow.Cells[0].AddParagraph(string.Empty);
            headerRow.Cells[1].AddParagraph("Creation date");
            headerRow.Cells[2].AddParagraph("Estimate ID");
            headerRow.Cells[3].AddParagraph("Customer name");
            headerRow.Cells[4].AddParagraph("Total amount");
            headerRow.Cells[5].AddParagraph("Status");
            if (this.data.Employee.Role == UserRoles.Manager)
            {
                headerRow.Cells[6].AddParagraph("Employee");
            }

            this.SetBordersForPartHeader(headerRow);

            this.HeightRows += headerRow.Height.Centimeter;

            this.EstimateFillContent();
        }

        private void EstimateFillContent()
        {
            double totalCalculatedSum = 0;
            int count = this.CurrentIndexRow == 0 ? 1 : this.CurrentIndexRow + 1;
            this.CurrentAmountRowsAtLeast = 0;

            var list =
                this.data.Entities.Select(companyEntity => companyEntity as Estimate).ToList()
                .OrderByDescending(x => x.CreationDate).ToList();

            XFont font = new XFont("Times New Roman", 12);
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            var pdfGfx = XGraphics.FromPdfPage(page);

            while (this.CurrentAmountRowsAtLeast <= 36)
            {
                if (this.CurrentIndexRow == this.TotalAmountRows)
                {
                    this.IsLastPage = true;
                    break;
                }

                var estimate = list[this.CurrentIndexRow];
                var row = this.table.AddRow();

                if (this.CurrentIndexRow % 2 == 0)
                {
                    row.Shading.Color = Colors.LightGray;
                }
                
                row.Cells[0].AddParagraph(count ++.ToString()).Format.RightIndent = 0;
                row.Cells[1].AddParagraph(estimate.CreationDate.ToString("MM/dd/yyyy"));
                row.Cells[2].AddParagraph(estimate.Id.ToString());
                row.Cells[3].AddParagraph(estimate.Customer.GetCustomerName());
                row.Cells[4].AddParagraph(string.Format("${0:F2}", estimate.TotalAmount));
                row.Cells[5].AddParagraph(estimate.EstimateStatus.ToString());

                var countLinesForCustomer = GetCountTextLines(pdfGfx, font, row.Cells[3], estimate.Customer.GetCustomerName());
                var countLines = countLinesForCustomer;
                if (this.data.Employee.Role == UserRoles.Manager)
                {
                    row.Cells[6].AddParagraph(estimate.Employee.Name);
                    var countLinesForEmployeeName = GetCountTextLines(pdfGfx, font, row.Cells[6], estimate.Employee.Name);
                    countLines = Math.Max(countLinesForCustomer, countLinesForEmployeeName);
                }
                
                this.CurrentIndexRow ++;

                totalCalculatedSum += estimate.TotalAmount;

                this.CurrentAmountRowsAtLeast += countLines;
            }

            this.TotalAmount += totalCalculatedSum;
            if (!this.IsLastPage)
            {
                Row grandTotal = this.table.AddRow();
                grandTotal.Height = "0.5cm";
                grandTotal.Cells[0].AddParagraph(string.Empty);
                grandTotal.Cells[1].AddParagraph(string.Empty);
                grandTotal.Cells[2].AddParagraph("Total page").Format.Alignment = ParagraphAlignment.Center;
                grandTotal.Cells[3].AddParagraph(string.Empty);
                grandTotal.Cells[4].AddParagraph(string.Format("${0:F2}", totalCalculatedSum));
                grandTotal.Cells[5].AddParagraph(string.Empty);
                if (this.data.Employee.Role == UserRoles.Manager)
                {
                    grandTotal.Cells[6].AddParagraph(string.Empty);
                }

                this.SetBordersForPartHeader(grandTotal);
            }

            if (this.CurrentIndexRow == this.TotalAmountRows)
            {
                this.IsLastPage = true;
            }

            if (this.IsLastPage)
            {
                this.SetTotalEstimate();
            }
        }

        private int GetCountTextLines(XGraphics pdfGfx, XFont font, Cell cell, string text)
        {
            Unit max = cell.Column.Width - (cell.Column.LeftPadding + cell.Column.RightPadding);
            // -6 I had to put it in the magic number, because for the text "Herb's #4 - Plano, TX" calculation was made as a single line, but the PDF file, it looked like two. 
            var maxWidth = Convert.ToInt32(Math.Round(max.Point, 0)) -6;
            var words = text.Split(' ');
            var countLines = 0;
            int lengthLine = 0;
            foreach (var word in words)
            {
                var currentWordLength = Convert.ToInt32(Math.Round(pdfGfx.MeasureString(word + ' ', font).Width, 0));
                lengthLine += currentWordLength;
                if (lengthLine >= maxWidth)
                {
                    countLines ++;
                    lengthLine = currentWordLength >= maxWidth ? 0 : currentWordLength;
                }
            }
            if (lengthLine > 0)
            {
                countLines++;
            }
            return countLines;
        }

        private void SetTotalEstimate()
        {
            Row grandTotal = this.table.AddRow();
            grandTotal.Cells[0].AddParagraph(string.Empty);
            grandTotal.Cells[1].AddParagraph(string.Empty);
            grandTotal.Cells[2].AddParagraph("TOTAL").Format.Alignment = ParagraphAlignment.Center;
            grandTotal.Cells[3].AddParagraph(string.Empty);
            grandTotal.Cells[4].AddParagraph(string.Format("${0:F2}", this.TotalAmount));
            grandTotal.Cells[5].AddParagraph(string.Empty);
            if (this.data.Employee.Role == UserRoles.Manager)
            {
                grandTotal.Cells[6].AddParagraph(string.Empty);
            }

            this.SetBordersForPartHeader(grandTotal);
        }

        #endregion

        //private void GetCurrentAmountRowsAtLeast(int customerLength)
        //{
        //    if (customerLength > 20 && customerLength <= 40)
        //    {
        //        this.CurrentAmountRowsAtLeast += 2;
        //    }
        //    else 
        //        if (customerLength > 40)
        //    {
        //        this.CurrentAmountRowsAtLeast += 3;
        //    }
        //    else
        //    {
        //        this.CurrentAmountRowsAtLeast += 1;
        //    }
        //}

        private void SetBordersForPartHeader(Row row)
        {
            row.Format.Font.Bold = true;
            row.HeadingFormat = true;
            row.Borders.Top.Color = Colors.Black;
            row.Borders.Bottom.Color = Colors.Black;
            row.Borders.Left.Color = Colors.White;
            row.Borders.Right.Color = Colors.White;
            row.Borders.Left.Width = 0.25;
            row.Borders.Right.Width = 0.25;
            row.Borders.Top.Width = 0.25;
            row.Borders.Bottom.Width = 0.25;
        }

        private void DefineStyles()
        {
            Style style = this.document.Styles["Normal"];

            style.Font.Name = "Verdana";

            style = this.document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = this.document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 11;

            style = this.document.Styles.AddStyle("PartTable", "Table");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 11;
        }
    }
}