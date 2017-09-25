using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Win32;

using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Attributes;
using PDR.Domain.Model.CustomLines;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Photos;

using PDR.Resources.PDR.Domain.Services.PDF;

using SmartArch.Data.Proxy;

namespace PDR.Domain.Services.PDFConverters
{
    internal class EstimateToPdfConverter
    {
        #region Private Fields

        private static int counter;

        private static int counterRow;

        private static int counterSupplements;

        private readonly Document document;

        private bool breakFooter;

        private bool IsFooterStart;

        private bool IsFooter;

        private Estimate estimate;

        private readonly RepairOrder repairOrder;

        private readonly Invoice invoice;

        private Section section;

        private Table bodyPartTable;

        private IList<Estimate> estimates;

        private Company company;

        private Affiliate affiliate;

        private int countRow;

        #endregion

        #region _ctor

        public EstimateToPdfConverter(Estimate estimate, RepairOrder repairOrder = null)
        {
            this.estimate = estimate;
            this.document = new Document();
            this.repairOrder = repairOrder;
            this.company = this.estimate.Company;
            this.affiliate = this.estimate.Affiliate;
        }

        public EstimateToPdfConverter(Estimate estimate, RepairOrder repairOrder, Invoice invoice = null)
            : this(estimate, repairOrder)
        {
            this.invoice = invoice;
        }

        public EstimateToPdfConverter(List<Estimate> estimates)
        {
            this.document = new Document();
            this.estimates = estimates;
            this.company = this.estimate.Company;
            this.affiliate = this.estimate.Affiliate;
        }

        #endregion

        private int GetCountTextLines(XGraphics pdfGfx, XFont font, string text)
        {
            Unit max = 11;
            var maxWidth = Convert.ToInt32(Math.Round(max.Point, 0));
            var words = text.Split(' ');
            var countLines = 0;
            int lengthLine = 0;
            foreach (var word in words)
            {
                var currentWordLength = Convert.ToInt32(Math.Round(pdfGfx.MeasureString(word + ' ', font).Width, 0));
                lengthLine += currentWordLength;
                if (lengthLine >= maxWidth)
                {
                    countLines++;
                    lengthLine = currentWordLength >= maxWidth ? 0 : currentWordLength;
                }
            }
            if (lengthLine > 0)
            {
                countLines++;
            }
            return countLines;
        }

        private int GetCountRow(string companyName)
        {
            XFont xFont = new XFont("Times New Roman", 12);
            PdfDocument pdfDocument = new PdfDocument();
            PdfPage pdfPpage = pdfDocument.AddPage();
            var pdfGfx = XGraphics.FromPdfPage(pdfPpage);
            var companyNameCountLines = GetCountTextLines(pdfGfx, xFont, companyName);

            var countRow = this.invoice != null ? 38 : 40;
            countRow -= companyNameCountLines * 2;
            return countRow;
        }

        private bool IsDetailed { get; set; }

        public Document CreateDocument(bool detailed = false)
        {
            this.document.Info.Title = string.Format("Estimate #{0} report", this.estimate.Id);
            this.document.Info.Subject = "Show estimate information";
            this.document.Info.Author = this.estimate.Employee.SignatureName ?? this.estimate.Employee.Name;
            this.document.DefaultPageSetup.LeftMargin = "2cm";
            this.document.DefaultPageSetup.RightMargin = "1cm";
            this.IsDetailed = detailed;
            
            this.CreatePage();
            
            if (this.CheckNotes())
            {
                this.RenderNotes();
            }
            
            this.DefineStyles();

            return this.document;
        }

        #region Create Page

        private void CreatePage()
        {
            this.section = this.document.AddSection();
            var pageLabel = this.CreateHeaderDocument();

            //Paragraph paragraph2 = this.section.Headers.Primary.AddParagraph();
            //paragraph2.AddText(
            //    this.repairOrder == null
            //        ? (this.estimate.Employee.SignatureName ?? this.estimate.Employee.Name)
            //        : string.Join("; ", this.repairOrder.TeamEmployeePercents.Select(x => x.TeamEmployee.SignatureName ?? x.TeamEmployee.Name)));
            //paragraph2.Format.Font.Size = 10.5;
            //paragraph2.Format.Font.Name = "Times New Roman";
            //paragraph2.Format.Alignment = ParagraphAlignment.Left;

            //paragraph2.Format.LeftIndent = "9.85cm";

            this.RenderGeneralInfo();
            
            this.RenderPartsTable();

            // render page numbers
            if (this.document.Sections.Count == 1 && !this.CheckNotes())
            {
                return;
            }

            var count = this.CheckNotes()
                            ? this.document.Sections.Count + 1
                            : this.document.Sections.Count;
            pageLabel.AddPageField();
            //pageLabel.Format.RightIndent = "0.5cm";
            pageLabel.AddText(string.Format(" of {0}", count));
            pageLabel.Format.Font.Name = "Times New Roman";
        }

        #region Render Header
        
        private Paragraph CreateHeaderDocument()
        {
            var header = this.section.Headers.Primary;
            var pageLabel = header.AddParagraph();
            pageLabel.Format.Alignment = ParagraphAlignment.Right;
            var logoFrame = header.AddTextFrame();
            var logoParagraph = logoFrame.AddParagraph();
            logoParagraph.Format.SpaceBefore = "-0.45cm";
            Image logo = logoParagraph.AddImage(this.GetTemporaryFile(this.estimate.Company.Logo.PhotoFull, this.estimate.Company.Logo.ContentType));
            logo.Height = "3cm";
            logo.Width = "3cm";
            
            logo.LockAspectRatio = true;
            logo.RelativeVertical = RelativeVertical.Line;
            logo.RelativeHorizontal = RelativeHorizontal.Margin;
            logo.Top = ShapePosition.Top;
            logo.Left = ShapePosition.Left;
            logo.WrapFormat.Style = WrapStyle.Through;

            this.AddAddressHeader();

            Paragraph paragraph = this.section.Headers.Primary.AddParagraph();
            paragraph.Format.Font.Name = "Times New Roman";

            paragraph.AddText(
                string.Format(
                "{0} #{1}",
                this.repairOrder == null ? "Estimate" : this.invoice == null ? "Repair order" : "Invoice",
                this.repairOrder == null ? this.estimate.Id : this.invoice == null ? this.repairOrder.Id : this.invoice.Id));
            paragraph.Format.Font.Size = 12;
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.SpaceBefore = "-2.75cm";
            paragraph.Format.SpaceAfter = 5;
            paragraph.Format.LeftIndent = "10.25cm";

            Paragraph paragraph1 = this.section.Headers.Primary.AddParagraph();
            var employee = this.repairOrder == null
                ? !string.IsNullOrWhiteSpace(this.estimate.Employee.SignatureName) ? this.estimate.Employee.SignatureName : this.estimate.Employee.Name
                : string.Join("; ", this.repairOrder.TeamEmployeePercents.Select(x => x.TeamEmployee.SignatureName ?? x.TeamEmployee.Name));
            paragraph1.AddText(this.repairOrder == null
                ? "Estimate Prepared By: " + employee
                : "Repairs performed by: " + employee);
            paragraph1.Format.Font.Size = 10.5;
            paragraph1.Format.Font.Name = "Times New Roman";
            paragraph1.Format.Alignment = ParagraphAlignment.Left;

            paragraph1.Format.LeftIndent = "10.25cm";

            return pageLabel;
        }

        private void AddSeparator()
        {
            var t = this.section.AddTextFrame();
            t.MarginTop = 0;
            t.MarginBottom = 0;
            t.Height = String.Format("{0}cm", 0.4);
        }

        public void AddAddressHeader()
        {
            // Create the text frame for the address
            var addressFrame = this.section.AddTextFrame();
            addressFrame.Height = "3.0cm";
            addressFrame.Width = "7.0cm";
            addressFrame.Left = ShapePosition.Left;
            addressFrame.Top = ShapePosition.Top;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            addressFrame.Left = "3.25cm";
            addressFrame.Top = "1cm";
            addressFrame.RelativeVertical = RelativeVertical.Page;

            var affiliate = this.affiliate;
            
            var cityCompany = string.IsNullOrWhiteSpace(this.company.City)
                           ? string.Empty
                           : this.company.City + ",";
            var stateCompany = this.company.State.ToString();
            var zipCompany = string.IsNullOrWhiteSpace(this.company.Zip) ? string.Empty : this.company.Zip;
            var companyname = this.company.Name;
            var companyAddress1 = this.company.Address1;
            var companyAddress2 = this.company.Address2;
            var phoneNumber = this.company.PhoneNumber;
            var email = this.company.Email;
            
            if(affiliate != null)
            {
                cityCompany = string.IsNullOrWhiteSpace(affiliate.City)
                           ? string.Empty
                           : affiliate.City + ",";
                stateCompany = affiliate.State.HasValue ? affiliate.State.Value.ToString() : string.Empty;
                zipCompany = string.IsNullOrWhiteSpace(affiliate.Zip) ? string.Empty : affiliate.Zip;
                companyname = affiliate.Name;
                companyAddress1 = affiliate.Address1;
                companyAddress2 = affiliate.Address2;
                phoneNumber = affiliate.Phone;
                email = affiliate.Email;
            }

            var addressParagraph = addressFrame.AddParagraph();
            addressParagraph.Format.Font.Name = "Times New Roman";
            addressParagraph.Format.Font.Size = 10.5;
            addressParagraph.Format.SpaceAfter = 5;
            var companyName = addressParagraph.AddFormattedText(companyname);
            companyName.Font.Size = 20;
            companyName.Font.Bold = true;

            addressParagraph.AddLineBreak();
            addressParagraph.AddText(string.IsNullOrWhiteSpace(companyAddress1) ? string.Empty : companyAddress1);
            addressParagraph.AddLineBreak();
            if (!string.IsNullOrWhiteSpace(companyAddress2))
            {
                addressParagraph.AddText(companyAddress2);
                addressParagraph.AddLineBreak();
            }

            addressParagraph.AddText(string.Format("{0} {1} {2}", cityCompany, stateCompany, zipCompany));
            addressParagraph.AddLineBreak();
            addressParagraph.AddText(string.Format("Phone: {0}", phoneNumber.ToPhoneFormat()));
            addressParagraph.AddLineBreak();
            addressParagraph.AddText(string.Format("E-mail: {0}", email));
            addressParagraph.AddLineBreak();

            this.countRow = GetCountRow(companyname);

            this.AddSeparator();
        }
        #endregion

        #region Render General Info

        private void RenderGeneralInfo()
        {
            var tableframe = this.section.AddTextFrame();
            tableframe.Top ="1.5cm";
            var table = tableframe.AddTable();

            table.Style = "Table";
            table.Rows.LeftIndent = 0;
            table.RightPadding = 0;
            table.LeftPadding = 0;
            table.TopPadding = 0;
            table.BottomPadding = 0;
            // table.Borders.Width = 0.2;

            // Before you can add a row, you must define the columns
            Column column = table.AddColumn("9cm");
            //column.Format.Font.Bold = true;

            column = table.AddColumn("0.1cm");
            //column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn("9cm");
            //column.Format.Alignment = ParagraphAlignment.Left;
            column.Format.PageBreakBefore = true;

            var mainRow = table.AddRow();
            var rightPart = mainRow.Cells[0];
            var leftPart = mainRow.Cells[2];
            
            AddDocumentInfo(rightPart);
            
            AddCarInfo(leftPart);
            
            //if(this.invoice != null)
            //{
            //    return;
            //}

            this.AddGuaranteeInfo();
        }

        #region Document Info (Left Part)

        private void AddDocumentInfo(Cell cell)
        {
            var rightFrame = cell.AddTextFrame();
            //rightFrame.MarginTop = "0.5cm";
            var rightTable = rightFrame.AddTable();
            rightTable.Rows.LeftIndent = 0;
            rightTable.RightPadding = 0;
            rightTable.LeftPadding = 0;
            rightTable.TopPadding = 0;
            rightTable.BottomPadding = 0;
            rightTable.Borders.ClearAll();

            var column = rightTable.AddColumn("3.5cm");
            column.Format.Font.Bold = true;
            column.Format.Alignment = ParagraphAlignment.Left;

            column = rightTable.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            var city = string.IsNullOrWhiteSpace(this.estimate.Customer.City)
                           ? string.Empty
                           : this.estimate.Customer.City + ",";

            var state = this.estimate.Customer.State.HasValue
                ? this.estimate.Customer.State.Value != StatesOfUSA.None
                    ? this.estimate.Customer.State.Value.ToString()
                    : string.Empty
                : string.Empty;
            var zip = string.IsNullOrWhiteSpace(this.estimate.Customer.Zip) ? string.Empty : this.estimate.Customer.Zip;
            rightTable.Rows.AddRow();
            //rightTable.Rows.AddRow();
            //rightTable.Rows.AddRow();

            var rows = new Dictionary<string, string>();
            rows["Name"] = this.estimate.Customer.GetCustomerName();
            rows["Address 1"] = this.estimate.Customer.Address1;
            rows["Address 2"] = this.estimate.Customer.Address2;
            rows["City, State ZIP"] = string.Format("{0} {1} {2}", city, state, zip);
            rows["Date"] = this.invoice == null ?
                            this.repairOrder == null
                                ? this.estimate.CreationDate.ToString("MM-dd-yyyy")
                                : this.repairOrder.CreationDate.ToString("MM-dd-yyyy")
                            : this.invoice.CreationDate.ToString("MM-dd-yyyy");
            rows["Phone"] = this.estimate.Customer.Phone;
            rows["E-mail"] = this.estimate.Customer.Email;
            rows["Insured name"] = this.estimate.Insurance.InsuredName ?? string.Empty;
            rows["Company"] = this.estimate.Insurance.CompanyName ?? string.Empty;
            rows["Policy #"] = this.estimate.Insurance.Policy ?? string.Empty;
            rows["Claim #"] = this.estimate.Insurance.Claim ?? string.Empty;
            rows["Accident date"] = this.estimate.Insurance.AccidentDate.HasValue ? this.estimate.Insurance.AccidentDate.Value.ToString("MM-dd-yyyy") : string.Empty;
            rows["Claim date"] = this.estimate.Insurance.ClaimDate.HasValue ? this.estimate.Insurance.ClaimDate.Value.ToString("MM-dd-yyyy") : string.Empty;

            RenderZebraTable(rightTable, rows);
            //rightTable.Rows.AddRow();

            //if (this.invoice != null)
            //{
            //    var row = rightTable.Rows.AddRow();
            //    var guarantee = row.Cells[0];
            //    this.RenderGuarantee(guarantee);
            //    return;
            //}

            this.RenderSignatureText(rightTable);
            //rightTable.Rows.AddRow();
            this.RenderSignature(rightTable);

            this.RenderDate(rightTable);
            this.RenderTextInBox(rightTable);

        }

        private static void RenderZebraTable(Table table, Dictionary<string, string> rows)
        {
            int z = 2;
            foreach (var row in rows)
            {
                RenderRow(table, row.Key, row.Value, z % 2 == 0);
                z++;
            }
        }

        private static void RenderRow(Table table, string name, string value, bool isShading)
        {
            var dateRow = table.AddRow();
            var nameCol = dateRow.Cells[0];
            nameCol.AddParagraph(string.Format("{0}",
                name + ":" + (string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Length > 32
                    ? new string('\t', 3)
                    : string.Empty)));
            var valueCol = dateRow.Cells[1];
            
            if (!string.IsNullOrEmpty(value))
            {
                if (name != "Name")
                {
                    if (value.Length > 24)
                    {
                        value = value.Insert(24, " ");
                    }
                }
            }
            else
            {
                value = string.Empty;
            }

            valueCol.AddParagraph(value);

            if (!isShading)
            {
                dateRow.Format.Shading.Color = Colors.LightGray;
                nameCol.Format.Shading.Color = Colors.LightGray;
                //valueCol.Format.Shading.Color = Colors.LightGray;
            }
        }

        #region Signature Block

        public void RenderSignatureText(Table table)
        {
            var signatureText = table.Rows.AddRow().Cells[0];
            var p = signatureText.AddParagraph(
                this.repairOrder == null ?
                string.Format(PDFRes.EstimateSignatureMessage, this.estimate.Company.Name) 
                : PDFRes.RepairOrderSignatureMessage);

            signatureText.MergeRight = 1;
            signatureText.Format.Font.Name = "Times New Roman";
            signatureText.Format.Font.Size = 11;
            signatureText.Format.Font.Bold = false;
            p.Format.Borders.DistanceFromLeft = "2mm";
            p.Format.Borders.DistanceFromRight = "2mm";
            p.Format.Borders.DistanceFromTop = "0mm";
            p.Format.Borders.DistanceFromBottom = "0mm";
        }

        public void RenderSignature(Table table)
        {
            var row = table.Rows.AddRow();
            var signatureTitle = row.Cells[0];
            var caps = signatureTitle.AddParagraph("Customer signature");
            caps.Format.SpaceBefore = "2mm";
            signatureTitle.MergeRight = 1;
            signatureTitle.Format.Shading.Color = Colors.LightGray;
            caps.Format.SpaceBefore = "1mm";
            var documentSignature = this.repairOrder == null ? (Photo)this.estimate.CustomerSignature : this.repairOrder.RoCustomerSignature;

            var image = documentSignature != null
                            ? this.GetTemporaryFile(documentSignature.PhotoFull, documentSignature.ContentType)
                            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "content", "images", "blank_signature.png");

            var signature = signatureTitle.AddParagraph();
            signature.Format.Shading.Color = Colors.White;
            signature.Format.Alignment = ParagraphAlignment.Center;
            signature.Format.Borders.Width = 0.2;
            signature.Format.Borders.Color = Colors.Gray;
            
            var sigImg = signature.AddImage(image);
            sigImg.Width = "3cm";
            sigImg.Height = "1.5cm";
        }

        #endregion

        #endregion

        #region Car Info (Right Part)

        private void AddCarInfo(Cell cell)
        {
            var leftFrame = cell.AddTextFrame();
            leftFrame.Top = "0.4cm";
            var table = leftFrame.AddTable();
            //table.Format.LeftIndent = "-1cm";
            table.Rows.LeftIndent = 0;
            table.RightPadding = 0;
            table.LeftPadding = 0;
            table.TopPadding = 0;
            table.BottomPadding = 0;
            table.Borders.ClearAll();

            var column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Left;
            column.Format.Font.Bold = true;

            column = table.AddColumn("6cm");
            column.Format.Alignment = ParagraphAlignment.Left;

            var rows1 = new Dictionary<string, string>();
            rows1["Year"] = this.estimate.Car.Year.ToString();
            rows1["Make"] = this.estimate.Car.Make;
            rows1["Model"] = this.estimate.Car.Model;
            rows1["Miles"] = this.estimate.Car.Mileage.ToString();
            rows1["Color"] = this.estimate.Car.Color;
            rows1["R.O. #"] = this.estimate.Car.CustRO;
            rows1["Stock #"] = this.estimate.Car.Stock;
            rows1["VIN#"] = this.estimate.Car.VIN;
            rows1["Status"] = this.invoice == null
                                 ? this.repairOrder == null
                                       ? this.estimate.EstimateStatus.ToString()
                                       : this.repairOrder.RepairOrderStatus.ToString()
                                 : this.invoice.Status.ToString();

            RenderZebraTable(table, rows1);

            var carImageArea = table.Rows.AddRow().Cells[0];

            carImageArea.MergeRight = 1;
            //carImageArea.MergeDown = 6;
            var carInsp = carImageArea.AddParagraph();

            carInsp.Format.SpaceBefore = "0.5cm";
            carInsp.Format.Alignment = ParagraphAlignment.Center;

            if (this.estimate.CarImage == null)
            {
                return;
            }

            var carInspImage = carInsp.AddImage(this.GetTemporaryFile(this.estimate.CarImage.PhotoFull, this.estimate.Company.Logo.ContentType));
            carInspImage.Height = "8cm";

            //if (this.invoice != null)
            //{
            //    return;
            //}

            //var guar = table.Rows.AddRow().Cells[0];
            //guar.MergeRight = 1;

            //this.RenderGuarantee(guar);
        }

        #endregion

        #region Date and Guarantee Block

        private void AddGuaranteeInfo()
        {
            var tableframe = this.section.AddTextFrame();
            if (this.estimate.Car.Type == VehicleTypes.Truck)
            {
                tableframe.Top = "11cm";
            }
            else
            {
                tableframe.Top = "10cm";
            }
            var table = tableframe.AddTable();

            table.Style = "Table";
            table.Rows.LeftIndent = 0;
            table.RightPadding = 0;
            table.LeftPadding = 0;
            table.TopPadding = 0;
            table.BottomPadding = 0;
            // table.Borders.Width = 0.2;

            // Before you can add a row, you must define the columns
            table.AddColumn("18cm");
            //table.AddColumn("0.5cm");
            //var column = table.AddColumn("9cm");
            //column.Format.PageBreakBefore = true;

            //table.AddRow();

            //if (this.invoice != null)
            //{
            //    return;
            //}

            //this.RenderDate(table);
            //this.RenderTextInBox(table);
            table.AddRow();
            var guarantee = table.Rows[0].Cells[0];
            //guarantee.MergeDown = 2;
            this.RenderGuarantee(guarantee);
        }
        
        public void RenderGuarantee(Cell gearanteeArea)
        {
            var textF = gearanteeArea.AddTextFrame();
            textF.Width = "18cm";
            //textF.MarginLeft = "4mm";
            //textF.MarginRight = "4mm";

            //if (this.invoice == null)
            //{
            //}
            //else
            //{
            //    textF.MarginTop = "4mm";
            //    textF.Width = "8.5cm";
            //    textF.MarginLeft = "1mm";
            //    textF.MarginBottom = "2mm";
            //}

            var p = textF.AddParagraph();
            //p.Format.LeftIndent = "-0.5cm";
            p.Format.RightIndent = "0cm";
            p.Format.Font.Name = "Times New Roman";
            p.Format.Font.Size = 9;
            p.AddTab();
            p.AddTab();
            p.AddSpace(70);

            p.AddFormattedText("GUARANTEE");
            p.AddLineBreak();
            p.AddFormattedText(string.Format(PDFRes.RepairOrderTextInTheBoxGuarantee, this.estimate.Company.Name)).Font.Size = 9;
            //gearanteeArea.MergeDown = 1;
            ParagraphBorderStyle(p);
        }

        public void RenderDate(Table table)
        {
            var dateRow = table.AddRow();
            //dateRow.Format.SpaceBefore = "0.5";
            //dateRow.Cells[0].AddParagraph();
            var dtT = dateRow.Cells[0].AddTextFrame();
            var t = dtT.AddTable();
            //t.Format.SpaceBefore = this.repairOrder != null ? "1.5cm" : "0.5cm";
            
            var col = t.AddColumn("1cm");
            t.AddColumn("7.7cm");
            var p = t.AddRow().Cells[0].AddParagraph("Date: ");
            p.Format.Font.Name = "Times New Roman";
            p.Format.Font.Size = 12;
            

            //dateRow.Cells[1].AddParagraph();
            var dtBorder = t.Rows[0].Cells[1].AddParagraph();
            dtBorder.Format.LeftIndent = "-1";
            
            dtBorder.Format.Borders.Bottom = new Border { Width = 0.2, Color = Colors.Black };
        }

        public void RenderTextInBox(Table table)
        {
            var row = table.AddRow();
            var textInBox = row.Cells[0];
            
            if (this.repairOrder == null)
            {
                //textInBox.AddParagraph().AddLineBreak();
                var quote = textInBox.AddParagraph();
                textInBox.MergeRight = 1;
                quote.Format.SpaceBefore = "-1.45cm";
                quote.AddText(PDFRes.EstimateTextInTheBox);
                ParagraphBorderStyle(quote);
            }

            //textInBox.AddParagraph().AddLineBreak();
            //textInBox.MergeRight = 1;
            textInBox.Format.Font.Bold = false;
        }
        
        private static void ParagraphBorderStyle(Paragraph p)
        {
            p.Format.Borders.Width = 1;
            p.Format.Borders.DistanceFromLeft = "1mm";
            p.Format.Borders.DistanceFromRight = "1mm";
            p.Format.Borders.DistanceFromTop = "2mm";
            p.Format.Borders.DistanceFromBottom = "2mm";
            p.Format.Alignment = ParagraphAlignment.Justify;
        }

        #endregion

        #endregion

        #region Render Parts Table

        private int GetRowToBreak()
        {
            var breakRow = 13;
            if (this.repairOrder != null)
            {
                breakRow = 13;
            }

            if (this.invoice != null)
            {
                breakRow = 13;
            }

            return breakRow;
        }

        private void SetBodyPartTableMargin(TextFrame frame)
        {
            frame.Top = "1cm";
            if (this.repairOrder != null)
            {
                frame.Top = "0.75cm";
            }

            if (this.invoice != null)
            {
                frame.Top = "9.5cm";
            }
        }

        private string GetFormattedSectionName(PartOfBody part)
        {
            var name = part.ToString().ToUpper();
            name = name.Replace("RRD", "RR D").Replace("RFD", "RF D");
            name = name.Replace("RFEN", "R FEN").Replace("LFEN", "L FEN");
            name = name.Replace("LFD", "LF D").Replace("LRD", "LR D");
            name = name.Replace("RQ", "R Q").Replace("LQ", "L Q");
            name = name.Replace("RCABCOR", "R CAB COR").Replace("LCABCOR", "L CAB COR");
            name = name.Replace("LTROOFRAIL", "LT ROOF RAIL").Replace("RTROOFRAIL", "RT ROOF RAIL");
            name = name.Replace("REARBUMPER", "REAR BUMPER").Replace("FRONTBUMPER", "FRONT BUMPER");
            name = name.Replace("METALSUNROOF", "METAL SUNROOF").Replace("DECKLID", "DECK LID");

            return name;
        }

        private string GetFormattedCarInspectionRowName(TotalDents totalDents)
        {
            string nameTotalDents = String.Empty;
            if (totalDents == TotalDents.NoDamage)
            {
                nameTotalDents = "No Damage";
            }
            else if (totalDents == TotalDents.Conventional)
            {
                nameTotalDents = "Conventional Repairs";
            }
            return nameTotalDents;
        }

        private void GenerateCarInspectionTextRow(CarInspection carInspection, string name)
        {
            if (carInspection.DentsAmount == TotalDents.NoDamage || carInspection.DentsAmount == TotalDents.Conventional)
            {
                string nameTotalDents = GetFormattedCarInspectionRowName(carInspection.DentsAmount);
                this.AddSimpleRow(true, true, counter.ToString(), "", nameTotalDents, string.Empty, carInspection.DentsCost.ToString("0.00"));
            }
            else if (IsDetailed)
            {
                var approxSize = string.Format("Approx. Size: {0}, Total # Dents: {1}",
                    carInspection.AverageSize.GetAverageSizeName(), carInspection.DentsAmount.GetTotalAmountName());
                this.AddSimpleRow(true, false, counter.ToString(), "PDR", approxSize, "add fields", carInspection.DentsCost.ToString("0.00"));
            }
            else
            {
                this.AddSimpleRow(true, false, counter.ToString(), "PDR", name, string.Empty, carInspection.DentsCost.ToString("0.00"));
            }
        }

        private void RenderPartsTable()
        {
            counter = 0;
            counterRow = 0;
            TextFrame bodyPartsTableFrame = this.section.AddTextFrame();
            bodyPartsTableFrame.MarginTop = "-3mm";
            //this.SetBodyPartTableMargin(bodyPartsTableFrame);
            //if (this.invoice != null)
            //{
            //    bodyPartsTableFrame.MarginTop = "10mm";
            //}
            this.bodyPartTable = bodyPartsTableFrame.AddTable();
            this.SetBodyPartTableStyles();

            this.AddRowWithBorder(true, "Line", "Operation", "Description", "Labor", "Extended Price $");

            foreach (var carInspection in this.estimate.CarInspections)
            {
                var name = GetFormattedSectionName(carInspection.Name);
                
                this.AddSimpleRow(false, false, counter.ToString(), string.Empty, name, string.Empty, string.Empty);

               this.GenerateCarInspectionTextRow(carInspection, name);

                foreach (var effort in carInspection.ChosenEffortItems)
                {
                    this.AddSimpleRow(
                        true,
                        false,
                        counter.ToString(),
                        effort.Operations.ToString().Replace('_', '&'),
                        effort.EffortItem.Name,
                        effort.Hours + " hrs",
                        string.Empty);
                }

                var max = this.CheckMaxPercents(carInspection);

                this.AddSimpleRow(
                        true,
                        false,
                        counter.ToString(),
                        string.Empty,
                        "Aluminium",
                        max ? string.Empty : this.estimate.Matrix.AluminiumPanel + "%",
                        max ? string.Empty : carInspection.AluminiumAmount.ToString("0.00"));


                this.AddSimpleRow(
                    true,
                    false,
                    counter.ToString(),
                    string.Empty,
                    "Double metal",
                    max ? this.estimate.EstMaxPercent + "%" : this.estimate.Matrix.DoubleLayeredPanels + "%",
                    max ? carInspection.AdditionalPercentsAmount.ToString("0.00") : carInspection.DoubleMetallAmount.ToString("0.00"));
                
                if (carInspection.Name == PartOfBody.Roof)
                {
                    this.AddSimpleRow(
                        true,
                        false,
                        counter.ToString(),
                        string.Empty,
                        "Oversized roof",
                        max ? string.Empty : this.estimate.Matrix.OversizedRoof + "%",
                        max ? string.Empty : carInspection.OversizedRoofAmount.ToString("0.00"));
                }

                this.AddSimpleRow(
                    true,
                    false,
                    counter.ToString(),
                    string.Empty,
                    "Corrosion protection",
                    string.Empty,
                    carInspection.CorrosionProtectionCost.ToString("0.00"));

                var oversizeDent = carInspection.CustomLines.SingleOrDefault(x => x is OversizedDentsLine);
                
                if (oversizeDent != null)
                {
                    this.AddSimpleRow(true, false, counter.ToString(), string.Empty, "Oversized dents", oversizeDent.Name, oversizeDent.Cost.ToString("0.00"));

                    foreach (var line in carInspection.CustomLines.Where(x => x is EffortLine))
                    {
                        this.AddSimpleRow(true, false, counter.ToString(), string.Empty, line.Name, string.Empty, line.Cost.ToString("0.00"));
                    }
                }

                var customLines = carInspection.CustomLines.Where(x => x is CustomCarInspectionLine).ToList();
                
                if (customLines.Count <= 0)
                {
                    continue;
                }

                //this.AddSimpleRow(true, counter.ToString(), string.Empty, string.Format("{0} custom fields", carInspection.Name).ToUpper(), string.Empty, string.Empty);
                foreach (var customLine in customLines)
                {
                    this.AddSimpleRow(
                        true,
                        true,
                        counter.ToString(),
                        string.Empty,
                        customLine.Name,
                        "add fields",
                        customLine.Cost.ToString("0.00"));
                }
            }

            var estimateCustomLines = this.estimate.CustomEstimateLines;
            if (estimateCustomLines.Count > 0)
            {
                this.AddSimpleRow(false, false, counter.ToString(), string.Empty, "Other", string.Empty, string.Empty);
                foreach (var customLine in estimateCustomLines)
                {
                    this.AddSimpleRow(
                        true,
                        true,
                        counter.ToString(),
                        string.Empty,
                        customLine.Name,
                        "add fields",
                        customLine.Cost.ToString("0.00"));
                }
            }

            if (this.repairOrder != null)
            {
                var suplemets = this.repairOrder.Supplements;
                counterSupplements = 0;
                if (!suplemets.IsEmpty)
                {
                    this.AddSimpleRow(false, false, string.Empty, string.Empty, "SUPPLEMENTS", string.Empty, string.Empty);

                    foreach (var supl in this.repairOrder.Supplements)
                    {
                        counterSupplements++;
                        this.AddSimpleRow(
                            true,
                            true,
                            counterSupplements.ToString(),
                            string.Empty,
                            supl.Description,
                            "add fields",
                            supl.Sum.ToString("0.00"));
                    }
                }
            }

            this.RenderFooter();
        }
        
        private bool CheckMaxPercents(CarInspection inspection)
        {
            var percents = 0.0;
            if (inspection.Aluminium)
            {
                percents += this.estimate.EstAluminiumPer;
            }

            if (inspection.DoubleMetal)
            {
                percents += this.estimate.EstDoubleMetalPer;
            }

            if (inspection.OversizedRoof)
            {
                percents += this.estimate.EstOversizedRoofPer;
            }

            return this.estimate.EstMaxPercent < percents;
        }
        
        private void SetBodyPartTableStyles()
        {
            this.bodyPartTable.Style = "PartTable";
            this.bodyPartTable.Format.Alignment = ParagraphAlignment.Center;
            this.bodyPartTable.Rows.LeftIndent = 0;
            this.bodyPartTable.TopPadding = 2;
            this.bodyPartTable.BottomPadding = 2;
            this.bodyPartTable.Rows.LeftIndent = 0;
            this.bodyPartTable.AddColumn();
            this.bodyPartTable.AddColumn();
            this.bodyPartTable.AddColumn().Width = "6cm";
            this.bodyPartTable.AddColumn().Width = "4cm";
            this.bodyPartTable.AddColumn();
        }
        
        public Row AddSimpleRow(bool leftAlign, bool allowWithNullableSum, params string[] colValues)
        {
            Regex regCustomFields = new Regex(@"(add fields)$");
            var rowToBreak = this.GetRowToBreak();
            var nullSum = allowWithNullableSum || colValues.Last() != "0.00";
            
            if (this.breakFooter && IsFooter)
            {
                AddSection(regCustomFields);
                this.IsFooterStart = false;
            }
            else if ((counterRow == rowToBreak && nullSum) || (counterRow - rowToBreak) % this.countRow == 0 && nullSum)
            {
                if (!this.IsFooter)
                {
                    AddSection(regCustomFields);
                }
            }

            return this.AddRow(leftAlign, allowWithNullableSum, regCustomFields, colValues);
        }

        private void AddSection(Regex regCustomFields)
        {
            this.section = this.document.AddSection();
            this.AddAddressHeader();
            var bodyPartsTableFrame = this.section.AddTextFrame();
            bodyPartsTableFrame.Top = "2cm";
            this.bodyPartTable = bodyPartsTableFrame.AddTable();
            this.SetBodyPartTableStyles();
            var header = this.AddRow(true, false, regCustomFields, "Line", "Operation", "Description", "Labor",
                                     "Extended Price $");
            counterRow--;
            counter--;
            SetBordersForPartHeader(header);
            this.breakFooter = false;
        }

        public Row AddRow(bool leftAlign, bool allowWitsNullableSum, Regex regCustomFields, params string[] colValues)
        {
            var nullSum = allowWitsNullableSum || colValues.Last() != "0.00";
            if (nullSum || colValues[2].ToLower() == "Labor".ToLower())
            {
                var row = this.bodyPartTable.AddRow();
                for (var i = 0; i < colValues.Length; i++)
                {
                    var cell = row.Cells[i].AddParagraph(colValues[i]);
                    
                    
                    if (regCustomFields.IsMatch(colValues[i]))
                    {
                        if(colValues[i - 1].Length > 50)
                        {
                            row.Cells[i - 1].Elements.Clear();
                            var count = (int)Math.Ceiling((double) colValues[i - 1].Length/50);
                            var start = 0;

                            for (var j = 0; j < count; j++)
                            {
                                var end = 50*(j + 1);
                                if(end >= colValues[i - 1].Length)
                                {
                                    end = end >= 100 ? colValues[i - 1].Length - 1 - 50 : colValues[i - 1].Length;
                                }
                                var str = colValues[i - 1].Substring(start, end);
                                row.Cells[i - 1].AddParagraph(str);
                                start = 50*(j + 1);
                            }
                            counterRow++;
                        }

                        row.Cells[i-1].MergeRight = 1;
                        row.Cells[i-1].Format.Alignment = ParagraphAlignment.Left;
                        cell = row.Cells[i].AddParagraph(string.Empty);
                    }

                    if (i == 2 && leftAlign)
                    {
                        cell.Format.Alignment = ParagraphAlignment.Left;
                    }
                }

                counterRow++;
                counter++;
                if(counter >= 17 && counter < 20)
                {
                    var d = 3;
                }
                if (this.breakFooter)
                {
                    counterRow++;
                    counter++;
                }

                return row;
            }

            return null;
        }

        public void AddRowWithBorder(bool leftAlign, params string[] colValues)
        {
            var row = this.AddSimpleRow(leftAlign, false, colValues);
            if (row != null)
            {
                SetBordersForPartHeader(row);
            }
        }

        private void RenderFooter()
        {
            this.IsFooter = true;
            this.IsFooterStart = true;
            string subtotalsSum = this.repairOrder == null
                ? this.estimate.Subtotal.ToString("0.00")
                : this.repairOrder.RoSubtotal.ToString("0.00");
            string laborSum = this.estimate.GetDocumentLaborSum().ToString("0.00");
            string adddiscount = "0.00";
            string discount = "0.00";
            string discountPercent = string.Empty;
            if (this.repairOrder != null)
            {
                adddiscount = Math.Round(this.repairOrder.AdditionalDiscount, 2, MidpointRounding.AwayFromZero).ToString("0.00");
                discount = this.repairOrder.DocumentDiscountSum.ToString("0.00");
                laborSum = this.repairOrder.WorkByThemselve ? "0.00" : this.estimate.GetDocumentLaborSum().ToString("0.00");

               discountPercent = this.estimate.Customer.ToPersist<RetailCustomer>() != null
                                      ? this.repairOrder.RetailDiscount.ToString()
                                      : this.estimate.EstDiscount.ToString();
            }

            string tax = this.repairOrder == null ? this.estimate.TaxSum.ToString("0.00") : this.repairOrder.TaxSum.ToString("0.00");
            string grand = this.repairOrder == null
                               ? this.estimate.TotalAmount.ToString("0.00")
                               : this.repairOrder.TotalAmount.ToString("0.00");

            var footerLength = new List<string> { subtotalsSum, laborSum, discount, tax, grand, adddiscount }.Count(x => x != "0.00") + 1;
            this.CalculateFooterBreak(footerLength);

            this.AddRowWithBorder(
                true,
                string.Empty,
                string.Empty,
                "SUBTOTAL PDR",
                //this.estimate.TotalLaborHours.ToString(),
                string.Empty,
                subtotalsSum);

            this.AddRowWithBorder(
                true,
                string.Empty,
                string.Empty,
                "LABOR",
                string.Format("{0} hrs x {1} /hr", this.estimate.TotalLaborHours, (this.estimate.NewLaborRate.HasValue ? (double)this.estimate.NewLaborRate : this.estimate.EstHourlyRate)),
                laborSum);

            if (this.repairOrder != null)
            {
                this.AddRowWithBorder(true, string.Empty, string.Empty, "ADDITIONAL DISCOUNT", string.Empty, adddiscount);
                this.AddRowWithBorder(true, string.Empty, string.Empty, "DISCOUNT", discountPercent + "%", discount);
            }

            this.AddRowWithBorder(
                true,
                string.Empty,
                string.Empty,
                "TAX",
                Convert.ToDecimal(this.estimate.CurrentLaborTax * 100) + "%",
                tax);

            this.AddRowWithBorder(
                true,
                string.Empty,
                string.Empty,
                "GRAND TOTAL",
                string.Empty,
                grand);
        }

        private void CalculateFooterBreak(int footerLength)
        {
            var rowToBreak = this.GetRowToBreak();
            var page = counterRow < rowToBreak ? 1 : Convert.ToInt32(Math.Floor(Convert.ToDouble((counterRow + footerLength - rowToBreak) / this.countRow)) + 2);
            var countRowOnFirstPage = this.invoice != null ? 21 : 14;

            if (page == 1)
            {
                if ((countRowOnFirstPage - counterRow) < footerLength)
                {
                    this.breakFooter = true;
                }
            }
            else if(page == 2 && (counterRow - rowToBreak) == 0)
            {
                this.breakFooter = true;
            }
            else
            {
                if (((page*this.countRow) - counterRow) < footerLength)
                {
                    this.breakFooter = true;
                }
                else if (this.document.Sections.Count < page && ((counterRow - rowToBreak)/(page - 2)) + footerLength >= this.countRow )
                {
                    this.breakFooter = true;
                }
            }
        }

        #endregion

        #endregion

        #region Render Notes

        private bool CheckNotes()
        {
            return this.repairOrder == null
                   && this.invoice == null
                   && !string.IsNullOrWhiteSpace(this.estimate.Company.Notes);
        }

        private void RenderNotes()
        {
            this.section = this.document.AddSection();
            var pageLabel = this.CreateHeaderDocument();

            var company = this.estimate.Company;
            var notes = company.Notes.Split(new[] { '\n' });
            var flag = true;
            foreach (Paragraph notesparagraph in notes.Select(note => this.section.AddParagraph(note)))
            {
                //notesparagraph.AddTab();
                //notesparagraph.Format.FirstLineIndent = "0.7cm";
                notesparagraph.Format.Alignment = ParagraphAlignment.Justify;
                notesparagraph.Format.Font.Name = "Times New Roman";
                notesparagraph.Format.Font.Size = 11;
                if (!flag) continue;
                notesparagraph.Format.SpaceBefore = "2.5cm";
                flag = false;
            }

            if (this.document.Sections.Count != 1)
            {
                pageLabel.AddPageField();
                //pageLabel.Format.RightIndent = "0.5cm";
                pageLabel.AddText(string.Format(" of {0}", this.document.Sections.Count));
                pageLabel.Format.Font.Name = "Times New Roman";
            }
        }

        #endregion

        private void DefineStyles()
        {
            Style style = this.document.Styles["Normal"];

            //style.Font.Name = "Verdana";

            style = this.document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);
            //style.ParagraphFormat.RightIndent = "0cm";

            style = this.document.Styles.AddStyle("Table", "Normal");
            //style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 11;

            style = this.document.Styles.AddStyle("PartTable", "Table");
            //style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 11;
        }

        private static void SetBordersForPartHeader(Row row)
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
    }
}
