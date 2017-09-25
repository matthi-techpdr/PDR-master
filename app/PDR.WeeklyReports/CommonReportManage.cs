using System;
using System.Collections.Generic;
using System.IO;
using NHibernate;
using PDR.Domain.Model.Enums;

namespace PDR.WeeklyReports
{
    public class CommonReportManage
    {
        private readonly ISession session;

        public CommonReportManage(ISession session)
        {
            this.session = session;
            //delete all .xlsx files in current directory
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            var files = Directory.GetFiles(directory, "*.xlsx");
            foreach (var currentFile in files)
            {
                File.Delete(currentFile);
            }
        }

        private void CreateReports(DateTime DateFrom, DateTime DateTo, string reportName)
        {
             var helper = new ReportsStoredProcedureHelper(session, DateFrom, DateTo);
             List<EmployeeInfo> employees = helper.ExecutStoredProcedureByEmployee();
             List<CustomerInfo> customers = helper.ExecutStoredProcedureByCustomer();
             StatesOfUSA[] filteredStates1 = new[] { StatesOfUSA.CO, StatesOfUSA.WY };
             StatesOfUSA[] filteredStates2 = new[] { StatesOfUSA.OK, StatesOfUSA.TX, StatesOfUSA.LA, StatesOfUSA.NM };
             List<CustomerInfo> filteredCustomers1 = helper.ExecutStoredProcedureByCustomer(filteredStates1);
             List<CustomerInfo> filteredCustomers2 = helper.ExecutStoredProcedureByCustomer(filteredStates2);
             var reports = new ReportsToXlsConverter(employees, customers, filteredCustomers1, filteredCustomers2, filteredStates1, filteredStates2, DateFrom, DateTo, reportName);
        }
        //Reports monthly from previous week
        public void CreateWeeklyReports()
        {
            var DateFrom = DateTime.Now.StartOfCurrentWeek();
            var DateTo = DateFrom.EndOfCurrentWeek();
            CreateReports(DateFrom, DateTo, "Weekly Operations");
            //var helper = new ReportsStoredProcedureHelper(session, DateFrom, DateTo);
            //var reports = new ReportsToXlsConverter(helper.Employees, helper.Customers, DateFrom, DateTo, "Weekly Operations");
        }
        //Reports monthly from previous morth
        public void CreateMonthlyReports()
        {
            var DateFrom = DateTime.Now.StartOfPreviousMonth();
            var DateTo = DateTime.Now.EndOfPreviousMonth();
            CreateReports(DateFrom, DateTo, "Monthly Operations");
            //var helper = new ReportsStoredProcedureHelper(session, DateFrom, DateTo);
            //var reports = new ReportsToXlsConverter(helper.Employees, helper.Customers, DateFrom, DateTo, "Monthly Operations");
        }

        //Reports monthly from beginning of the year
        public void CreateSummingMonthlyReports()
        {
            DateTime DateFrom;
            if (DateTime.Now.Month > 1)
            {
                DateFrom = DateTime.Now.StartOfCurrentYear();
            }
            else
            {
                DateFrom = DateTime.Now.StartOfPreviousYear();
            }
            var DateTo = DateTime.Now.EndOfPreviousMonth();
            if(DateTo > DateFrom){}
            CreateReports(DateFrom, DateTo, "YTD Operations");
            //var helper = new ReportsStoredProcedureHelper(session, DateFrom, DateTo);
            //var reports = new ReportsToXlsConverter(helper.Employees, helper.Customers, DateFrom, DateTo, "YTD Operations");
        }

        //Reports from previous year
        public void CreateAnnualReports()
        {
            var DateFrom = DateTime.Now.StartOfPreviousYear();
            var DateTo = DateTime.Now.EndOfPreviousYear();
            CreateReports(DateFrom, DateTo, "Annual Operations");
            //var helper = new ReportsStoredProcedureHelper(session, DateFrom, DateTo);
            //var reports = new ReportsToXlsConverter(helper.Employees, helper.Customers, DateFrom, DateTo, "Annual Operations");
        }
    }
}
