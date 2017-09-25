using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using System.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using PDR.Domain.Model.Enums;
using PDR.Domain.Services.Mailing;
using System.IO.Packaging;
using System.Xml.Linq;

namespace PDR.WeeklyReports
{
    public class ReportsToXlsConverter
    {
        private IEnumerable<EmployeeInfo> Employees { get; set; }

        private IEnumerable<CustomerInfo> Customers { get; set; }

        private IEnumerable<CustomerInfo> FilteredCustomers1 { get; set; }

        private IEnumerable<CustomerInfo> FilteredCustomers2 { get; set; }

        //private IEnumerable<String> TextSPCustomers { get; set; }

        //private IEnumerable<String> TextSPEmployees { get; set; }

        private DateTime DateFrom { get; set; }

        private DateTime DateTo { get; set; }

        private int CountTech
        {
            get
            {
                return this.Employees.Count(x => x.EmployeeRole != "RITechnician");
            }
        }

        private int CountRiTech
        {
            get
            {
                return this.Employees.Count(x => x.EmployeeRole == "RITechnician");
            }
        }

        private string CustomerFileName { get; set; }

        private string FilteredCustomer1FileName { get; set; }

        private string FilteredCustomer2FileName { get; set; }

        private string EmloyeeFileName  { get; set; }

        private string TitleName { get; set; }

        private string FilteredCustomer1TitleName { get; set; }

        private string FilteredCustomer2TitleName { get; set; }

        private string EmailTextByEmloyee{ get; set; }

        private string EmailSubjectByEmployee { get; set; }

        private string EmailTextByCustomer { get; set; }

        private string EmailSubjectByCustomer { get; set; }

        private string EmailTextByFilteredCustomer1 { get; set; }

        private string EmailTextByFilteredCustomer2 { get; set; }

        private string EmailFrom { get; set; }

        public ReportsToXlsConverter(IEnumerable<EmployeeInfo> employees, IEnumerable<CustomerInfo> customers, IEnumerable<CustomerInfo> FilteredCustomers1, IEnumerable<CustomerInfo> FilteredCustomers2, StatesOfUSA[] FilterStates1, StatesOfUSA[] FilterStates2, DateTime dateFrom, DateTime dateTo, string reportName)
        {
            this.CustomerFileName = String.Format("{0} - Customer Report.xlsx", reportName);
            this.FilteredCustomer1FileName= String.Format("{0} - Customer Report ({1}).xlsx", reportName, String.Join(",", FilterStates1));
            this.FilteredCustomer2FileName = String.Format("{0} - Customer Report ({1}).xlsx", reportName, String.Join(",", FilterStates2));
            this.EmloyeeFileName = String.Format("{0} - Emloyee Report.xlsx", reportName);

            this.EmailTextByEmloyee = String.Format("Hi-Tech Paintless Dent Removal. {0} - Employee Report.", reportName);
            this.EmailTextByCustomer = String.Format("Hi-Tech Paintless Dent Removal. {0} - Customer Report.", reportName);
            this.EmailTextByFilteredCustomer1 = String.Format("Hi-Tech Paintless Dent Removal. {0} - Customer Report ({1}).", reportName, String.Join(",", FilterStates1));
            this.EmailTextByFilteredCustomer2 = String.Format("Hi-Tech Paintless Dent Removal. {0} - Customer Report ({1}).", reportName, String.Join(",", FilterStates2));
            this.TitleName = reportName;
            this.FilteredCustomer1TitleName = String.Format("{0} - Customer Report ({1})", reportName, String.Join(",", FilterStates1));
            this.FilteredCustomer2TitleName = String.Format("{0} - Customer Report ({1})", reportName, String.Join(",", FilterStates2));
            this.Employees = employees;
            this.Customers = customers;
            this.FilteredCustomers1 = FilteredCustomers1;
            this.FilteredCustomers2 = FilteredCustomers2;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            //this.TextSPEmployees = textSPEmployees;
            //this.TextSPCustomers = textSPCustomers;
            this.EmailFrom = ConfigurationManager.AppSettings["emailFrom"];
            //this.DateFrom = DateTime.Now.StartOfPreviousWeek(DayOfWeek.Monday);
            //this.DateTo = this.DateFrom.AddDays(7).AddSeconds(-1);
            this.EmailSubjectByEmployee = EmailTextByEmloyee.Substring(0, EmailTextByEmloyee.Length -1) + String.Format(" from {0} to {1}", DateFrom.ToString("MM/dd/yyyy"), DateTo.ToString("MM/dd/yyyy"));
            this.EmailSubjectByCustomer = EmailTextByCustomer.Substring(0, EmailTextByCustomer.Length - 1) + String.Format(" from {0} to {1}", DateFrom.ToString("MM/dd/yyyy"), DateTo.ToString("MM/dd/yyyy"));
            this.Create();
        }

        private void Create()
        {
            string emailTo = ConfigurationManager.AppSettings["emailTo"];
            string companyname = ConfigurationManager.AppSettings["companyName"];

            #region File For Employees
            using (FileStream file = new FileStream(EmloyeeFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Report");
                var messageDate = String.Format("From {0} to {1}", DateFrom.ToString("MM/dd/yyyy"), DateTo.ToString("MM/dd/yyyy"));
                var messageTech = String.Format("{0} - Technicians Report", TitleName);
                var messageRiTech = String.Format("{0} - RI Technicians Report", TitleName);

                //for Tech
                ws.Cell(1, 1).Value = companyname;
                this.RenderHeaderForEmloyee(ws, messageTech, messageDate, 2);
                this.RenderBodyTableForEmloyee(ws, false, 5);

                //for RiTech
                var startRowForRiTech = CountTech + 6;
                this.RenderHeaderForEmloyee(ws, messageRiTech, messageDate, startRowForRiTech);
                this.RenderBodyTableForEmloyee(ws, true, startRowForRiTech + 3);

                var rngTable = ws.Range(1, 1, Employees.Count() + 7, 4);
                rngTable.FirstCell().Style.Font.FontSize = 14;
                rngTable.Row(1).Merge();
                rngTable.Row(2).Merge();
                rngTable.Row(3).Merge();
                rngTable.Row(startRowForRiTech).Merge();
                rngTable.Row(startRowForRiTech+1).Merge();

                RenderGrandTotalForEmloyee(ws, Employees.Count() + 10);

                ws.Columns().AdjustToContents();

                //var wsSqlScript = wb.Worksheets.Add("SQL");
                //RenderBodyScriptSPForEmloyee(wsSqlScript);

                wb.SaveAs(file);
            }
            this.MakeRelativePaths(EmloyeeFileName);
            var attachment = new Attachment(EmloyeeFileName);
            attachment.ContentType = new ContentType("application/vnd.ms-excel");

            var attachments = new List<Attachment> { attachment };
            var mailService = new MailService();
            mailService.Send(EmailFrom, emailTo, EmailSubjectByEmployee, EmailTextByEmloyee, attachments);
            #endregion

            #region File For Customers
            using (FileStream file = new FileStream(CustomerFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Report");
                var messageDate = String.Format("From {0} to {1}", DateFrom.ToString("MM/dd/yyyy"), DateTo.ToString("MM/dd/yyyy"));
                var messageCust = String.Format("{0} - Customer Report", TitleName);

                ws.Cell(1, 1).Value = companyname;
                RenderHeaderForCustomer(ws, messageCust, messageDate, 2);
                RenderBodyTableForCustomer(ws, 5, this.Customers);

                var rngTable = ws.Range(1, 1, 3, 6);
                rngTable.FirstCell().Style.Font.FontSize = 14;

                rngTable.Row(1).Merge();
                rngTable.Row(2).Merge();
                rngTable.Row(3).Merge();

                RenderGrandTotalForCustomer(ws, this.Customers.Count() + 6, this.Customers);

                ws.Columns().AdjustToContents();

                //var wsSqlScript = wb.Worksheets.Add("SQL");
                //RenderBodyScriptSPForCustomer(wsSqlScript);
                wb.SaveAs(file);
            }
            this.MakeRelativePaths(CustomerFileName);
            attachment = new Attachment(CustomerFileName);
            attachments = new List<Attachment> { attachment };
            mailService.Send(EmailFrom, emailTo, EmailSubjectByCustomer, EmailTextByCustomer, attachments);
            #endregion

            #region File For FilteredCustomers1
            using (FileStream file = new FileStream(FilteredCustomer1FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Report");
                var messageDate = String.Format("From {0} to {1}", DateFrom.ToString("MM/dd/yyyy"), DateTo.ToString("MM/dd/yyyy"));
                var messageCust = String.Format("{0} - Customer Report", FilteredCustomer1TitleName);

                ws.Cell(1, 1).Value = companyname;
                RenderHeaderForCustomer(ws, messageCust, messageDate, 2);
                RenderBodyTableForCustomer(ws, 5, this.FilteredCustomers1);

                var rngTable = ws.Range(1, 1, 3, 6);
                rngTable.FirstCell().Style.Font.FontSize = 14;

                rngTable.Row(1).Merge();
                rngTable.Row(2).Merge();
                rngTable.Row(3).Merge();

                RenderGrandTotalForCustomer(ws, this.FilteredCustomers1.Count() + 6, this.FilteredCustomers1);

                ws.Columns().AdjustToContents();

                //var wsSqlScript = wb.Worksheets.Add("SQL");
                //RenderBodyScriptSPForCustomer(wsSqlScript);
                wb.SaveAs(file);
            }
            this.MakeRelativePaths(FilteredCustomer1FileName);
            attachment = new Attachment(FilteredCustomer1FileName);
            attachments = new List<Attachment> { attachment };
            mailService.Send(EmailFrom, emailTo, EmailSubjectByCustomer, EmailTextByFilteredCustomer1, attachments);
            #endregion

            #region File For FilteredCustomers2
            using (FileStream file = new FileStream(FilteredCustomer2FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Report");
                var messageDate = String.Format("From {0} to {1}", DateFrom.ToString("MM/dd/yyyy"), DateTo.ToString("MM/dd/yyyy"));
                var messageCust = String.Format("{0} - Customer Report", FilteredCustomer2TitleName);

                ws.Cell(1, 1).Value = companyname;
                RenderHeaderForCustomer(ws, messageCust, messageDate, 2);
                RenderBodyTableForCustomer(ws, 5, this.FilteredCustomers2);

                var rngTable = ws.Range(1, 1, 3, 6);
                rngTable.FirstCell().Style.Font.FontSize = 14;

                rngTable.Row(1).Merge();
                rngTable.Row(2).Merge();
                rngTable.Row(3).Merge();

                RenderGrandTotalForCustomer(ws, this.FilteredCustomers2.Count() + 6, this.FilteredCustomers2);

                ws.Columns().AdjustToContents();

                //var wsSqlScript = wb.Worksheets.Add("SQL");
                //RenderBodyScriptSPForCustomer(wsSqlScript);
                wb.SaveAs(file);
            }
            this.MakeRelativePaths(FilteredCustomer2FileName);
            attachment = new Attachment(FilteredCustomer2FileName);
            attachments = new List<Attachment> { attachment };
            mailService.Send(EmailFrom, emailTo, EmailSubjectByCustomer, EmailTextByFilteredCustomer2, attachments);
            #endregion
        }

        private void RenderHeaderForEmloyee(IXLWorksheet ws, string title, string date, int numberRow)
        {
            ws.Cell(numberRow, 1).Value = title;
            ws.Cell(numberRow + 1, 1).Value = date;
            ws.Cell(numberRow + 2, 1).Value = "#";
            ws.Cell(numberRow + 2, 2).Value = "EMPLOYEE NAME";
            ws.Cell(numberRow + 2, 3).Value = "TOTAL INVOICES";
            ws.Cell(numberRow + 2, 4).Value = "TOTAL AMOUNT";

            ws.Cell(numberRow, 1).Style.Font.FontSize = 12;
            ws.Cell(numberRow+1, 1).Style.Font.FontSize = 12;
            ws.Cell(numberRow + 1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            ws.Row(numberRow + 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rng = ws.Range(numberRow + 2, 1, numberRow + 2, 4);
            rng.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rng.RangeUsed().Style.Border.InsideBorderColor = XLColor.Gray;
            rng.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rng.RangeUsed().Style.Border.OutsideBorderColor = XLColor.Gray;
        }

        private void RenderBodyTableForEmloyee(IXLWorksheet ws, bool forRi, int startRow)
        {
            var counter = 1;
            if (forRi)
            {
                ws.Cell(startRow, 1).Value = Employees.Where(x => x.EmployeeRole == "RITechnician")
                                                        .Select(x => new
                                                        {
                                                            Id = counter++,
                                                            EMPLOYEE_NAME = x.EmployeeName,
                                                            TOTAL_INVOICES = x.TotalInvoices,
                                                            TOTAL_AMOUNT = x.TotalAmount
                                                        }).AsEnumerable();
            }
            else
            {
                ws.Cell(startRow, 1).Value = Employees.Where(x => x.EmployeeRole != "RITechnician")
                                                        .Select(x => new
                                                        {
                                                            Id = counter++,
                                                            EMPLOYEE_NAME = x.EmployeeName,
                                                            TOTAL_INVOICES = x.TotalInvoices,
                                                            TOTAL_AMOUNT = x.TotalAmount
                                                        }).AsEnumerable();
            }

            var countEmployees = forRi ? CountRiTech : CountTech;
            var rngNumbers = ws.Range(startRow, 3, startRow + countEmployees, 3);
            rngNumbers.Style.NumberFormat.Format = "#,###";
            rngNumbers.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            var rngMoney = ws.Range(startRow, 4, startRow + countEmployees, 4);
            rngMoney.Style.NumberFormat.Format = "$ #,##0.00";

            if (countEmployees > 0)
            {
                var rng = ws.Range(startRow, 1, startRow + countEmployees, 4);
                rng.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                rng.RangeUsed().Style.Border.InsideBorderColor = XLColor.Gray;
                rng.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rng.RangeUsed().Style.Border.OutsideBorderColor = XLColor.Gray;
            }
        }

        private void RenderGrandTotalForEmloyee(IXLWorksheet ws, int startRow)
        {
            ws.Cell(startRow, 1).Value = "1";
            ws.Cell(startRow, 1).Style.Font.SetFontColor(XLColor.White);
            ws.Cell(startRow, 2).Value = "Grand Total";
            //ws.Cell(startRow, 3).Value = Employees.Sum(x => x.TotalInvoices);
            ws.Cell(startRow, 4).Value = Employees.Sum(x => x.TotalAmount);

            //ws.Cell(startRow, 3).Style.NumberFormat.Format = "#,###";
            //ws.Cell(startRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell(startRow, 4).Style.NumberFormat.Format = "$ #,##0.00";

            var rng = ws.Range(startRow, 1, startRow, 4);
            rng.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rng.RangeUsed().Style.Border.InsideBorderColor = XLColor.Gray;
            rng.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rng.RangeUsed().Style.Border.OutsideBorderColor = XLColor.Gray;
        }

        private static void RenderHeaderForCustomer(IXLWorksheet ws, string title, string date, int numberRow)
        {
            ws.Cell(numberRow, 1).Value = title;
            ws.Cell(numberRow + 1, 1).Value = date;
            ws.Cell(numberRow + 2, 1).Value = "#";
            ws.Cell(numberRow + 2, 2).Value = "CUSTOMER NAME";
            ws.Cell(numberRow + 2, 3).Value = "STATE";
            ws.Cell(numberRow + 2, 4).Value = "CITY";
            ws.Cell(numberRow + 2, 5).Value = "TOTAL INVOICES";
            ws.Cell(numberRow + 2, 6).Value = "TOTAL AMOUNT";

            ws.Cell(numberRow, 1).Style.Font.FontSize = 12;
            ws.Cell(numberRow + 1, 1).Style.Font.FontSize = 12;
            ws.Cell(numberRow + 1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            ws.Row(numberRow + 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rng = ws.Range(numberRow + 2, 1, numberRow + 2, 6);
            rng.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rng.RangeUsed().Style.Border.InsideBorderColor = XLColor.Gray;
            rng.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rng.RangeUsed().Style.Border.OutsideBorderColor = XLColor.Gray;

        }

        private static void RenderBodyTableForCustomer(IXLWorksheet ws, int startRow, IEnumerable<CustomerInfo> customers)
        {
            var counter = 1;
            ws.Cell(startRow, 1).Value = customers.Select(x => new
            {   Id = counter++,
                EMPLOYEE_NAME = x.CustomerName,
                STATE = x.State,
                CITY = x.City,
                TOTAL_INVOICES = x.TotalInvoices,
                TOTAL_AMOUNT = x.TotalAmount
            }).AsEnumerable();

            var rngNumbers = ws.Range(startRow, 5, startRow + customers.Count(), 5);
            rngNumbers.Style.NumberFormat.Format = "#,###";
            rngNumbers.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngMoney = ws.Range(startRow, 6, startRow + customers.Count(), 6);
            rngMoney.Style.NumberFormat.Format = "$ #,##0.00";

            var rngCity = ws.Range(startRow, 4, startRow + customers.Count(), 4);
            rngCity.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            var rngState = ws.Range(startRow, 3, startRow + customers.Count(), 3);
            rngState.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            if (customers.Count() > 0)
            {
                var rng = ws.Range(startRow, 1, startRow + customers.Count(), 6);
                rng.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                rng.RangeUsed().Style.Border.InsideBorderColor = XLColor.Gray;
                rng.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                rng.RangeUsed().Style.Border.OutsideBorderColor = XLColor.Gray;
            }
        }

        private static void RenderGrandTotalForCustomer(IXLWorksheet ws, int startRow, IEnumerable<CustomerInfo> customers)
        {
            ws.Cell(startRow, 1).Value = "1";
            ws.Cell(startRow, 1).Style.Font.SetFontColor(XLColor.White);
            ws.Cell(startRow, 2).Value = "Grand Total";
            //ws.Cell(startRow, 5).Value = Customers.Sum(x => x.TotalInvoices);
            ws.Cell(startRow, 6).Value = customers.Sum(x => x.TotalAmount);

            //ws.Cell(startRow, 5).Style.NumberFormat.Format = "#,###";
            //ws.Cell(startRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Cell(startRow, 6).Style.NumberFormat.Format = "$ #,##0.00";

            var rng = ws.Range(startRow, 1, startRow, 6);
            rng.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rng.RangeUsed().Style.Border.InsideBorderColor = XLColor.Gray;
            rng.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rng.RangeUsed().Style.Border.OutsideBorderColor = XLColor.Gray;
        }

        //fix the workbook relations file for IOS
        private void MakeRelativePaths(string filepath)
        {
            // Get the namespace strings
            const string documentRelationshipType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";
            const string relationshipSchema = "http://schemas.openxmlformats.org/package/2006/relationships";

            string documentUri = null;
            string documentDirectory = null;
            string documentName = null;

            Uri relDocUri = null;

            XName targetAttributeName = null;
            string targetValue = null;

            //  Open the package
            using (Package xlPackage = Package.Open(filepath, FileMode.Open, FileAccess.ReadWrite))
            {
                // Get the directory and filename of the main document part (e.g. /xl/workbook.xml).
                foreach (System.IO.Packaging.PackageRelationship relationship in xlPackage.GetRelationshipsByType(documentRelationshipType))
                {
                    documentUri = relationship.TargetUri.ToString();

                    documentName = System.IO.Path.GetFileName(documentUri);
                    documentDirectory = documentUri.Substring(0, documentUri.Length - documentName.Length);

                    //  There should only be document part in the package, but break out anyway.
                    break;
                }

                // Load the relationship document
                relDocUri = new Uri(documentDirectory + "_rels/" + documentName + ".rels", UriKind.Relative);
                XDocument relDoc = XDocument.Load(xlPackage.GetPart(relDocUri).GetStream());

                // Loop through all of the relationship nodes
                targetAttributeName = XName.Get("Target");
                foreach (XElement relNode in relDoc.Elements(XName.Get("Relationships", relationshipSchema)).Elements(XName.Get("Relationship", relationshipSchema)))
                {
                    // Edit the value of the Target attribute
                    targetValue = relNode.Attribute(targetAttributeName).Value;

                    if (targetValue.StartsWith(documentDirectory))
                        targetValue = targetValue.Substring(documentDirectory.Length);

                    relNode.Attribute(targetAttributeName).Value = targetValue;
                }

                // Save the document
                relDoc.Save(xlPackage.GetPart(relDocUri).GetStream());
            }
        }

        //private void RenderBodyScriptSPForCustomer(IXLWorksheet ws)
        //{
        //    const int START_ROW = 1;
        //    var text = String.Join(String.Empty, TextSPCustomers);
        //    ws.Cell(START_ROW, 1).Value = text;

        //    ws.Rows().Style.Alignment.SetWrapText();
        //    ws.Columns().AdjustToContents();
        //}

        //private void RenderBodyScriptSPForEmloyee(IXLWorksheet ws)
        //{
        //    const int START_ROW = 1;
        //    var text = String.Join(String.Empty, TextSPEmployees);
        //    ws.Cell(START_ROW, 1).Value = text;

        //    ws.Rows().Style.Alignment.SetWrapText();
        //    ws.Columns().AdjustToContents();
        //}
    }
}
