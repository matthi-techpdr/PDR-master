using System;
using System.Collections.Generic;
using System.Configuration;

using NHibernate;

using PDR.Domain.Model.Enums;

namespace PDR.WeeklyReports
{
    public class ReportsStoredProcedureHelper
    {
        private readonly ISession session;

        private IList<string> Parameters { get; set; }

        //public IList<EmployeeInfo> Employees { get; set; }

        //public IList<CustomerInfo> Customers { get; set; }

        //public IList<String> TextSPCustomers { get; set; }

        //public IList<String> TextSPEmployees { get; set; }

        //private const string SPEmployee = "[dbo].[WeeklyReportsByEmployees]";

        //private const string SPCustomer = "[dbo].[WeeklyReportsByCustomers]";

        private DateTime DateFrom { get; set; }

        private DateTime DateTo { get; set; }

        //public string SPEmployeeName
        //{
        //    get
        //    {
        //        return "WeeklyReportsByEmployees";
        //    }
        //}

        //public string SPCustomerName 
        //{
        //    get
        //    {
        //        return "WeeklyReportsByCustomers";
        //    }
        //}

        private const string SPbyEmployee = "exec [dbo].[ReportsByEmployees]";

        private const string SPbyCustomer = "exec [dbo].[ReportsByCustomers]";

        //private const string TextSPForCustomers = "exec sp_helptext N'" + SPCustomer + "'";

        //private const string TextSPForEmployees = "exec sp_helptext N'" + SPEmployee + "'";

        public ReportsStoredProcedureHelper(ISession session, DateTime dateFrom, DateTime dateTo)
        {
            this.session = session;
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            //this.Employees = new List<EmployeeInfo>();
            //this.Customers = new List<CustomerInfo>();
            //this.TextSPCustomers = new List<string>();
            //this.TextSPEmployees = new List<string>();

            int companyId;
            Int32.TryParse(ConfigurationManager.AppSettings["companyId"], out companyId);

            #region Set input parameters
            this.Parameters = new List<string>();
            this.Parameters.Add(String.Format("@companyId = {0}", companyId));
            this.Parameters.Add(String.Format(", @DateFrom = '{0}'", this.DateFrom));
            this.Parameters.Add(String.Format(", @DateTo = '{0}'", this.DateTo));
            #endregion
            //this.ExecutStoredProcedureByEmployee();
            //this.ExecutStoredProcedureByCustomer();
            //GetStoredProcedureText();
        }

        public List<EmployeeInfo> ExecutStoredProcedureByEmployee()
        {
            List<EmployeeInfo> Employees = new List<EmployeeInfo>();
            var query = SPbyEmployee;
            for (int i = 0; i < this.Parameters.Count; i++)
            {
                query += this.Parameters[i];
            }

            try
            {
                var objects =
                    this.session.CreateSQLQuery(query)
                        .AddScalar("EmployeeName", NHibernateUtil.String)
                        .AddScalar("EmployeeRole", NHibernateUtil.String)
                        .AddScalar("TotalInvoices", NHibernateUtil.Int32)
                        .AddScalar("TotalAmount", NHibernateUtil.Double)
                        .List();

                for (int i = 0; i < objects.Count; i++)
                {
                    var item = (object[])objects[i];
                    var employee = new EmployeeInfo();
                    employee.EmployeeName = item[0] == null ? "" : item[0].ToString();
                    employee.EmployeeRole = item[1] == null ? "" : item[1].ToString();
                    employee.TotalInvoices = item[2] == null ? 0 : Convert.ToInt32(item[2]);
                    employee.TotalAmount = item[3] == null ? 0 : Convert.ToDouble(item[3]);
                    Employees.Add(employee);
                }
            }
            catch (Exception ex)
            {
            }

            return Employees;

        }

        public List<CustomerInfo> ExecutStoredProcedureByCustomer(StatesOfUSA[] customerStates = null)
        {
            List<CustomerInfo> Customers = new List<CustomerInfo>();
            var query = SPbyCustomer;
            for (int i = 0; i < this.Parameters.Count; i++)
            {
                query += this.Parameters[i];
            }
             if (customerStates != null)
            {
                string states = "@States= ';";
                //states.Columns.Add("State");
                foreach (StatesOfUSA state in customerStates)
                {
                    //states.Rows.Add((int)state);
                    states += String.Format("{0};", (int)state);
                }
                states += "'";
                query += ","+states;
            }
            try
            {
                var objects =
                    this.session.CreateSQLQuery(query)
                        .AddScalar("CustomerName", NHibernateUtil.String)
                        .AddScalar("State", NHibernateUtil.String)
                        .AddScalar("City", NHibernateUtil.String)
                        .AddScalar("TotalInvoices", NHibernateUtil.Int32)
                        .AddScalar("TotalAmount", NHibernateUtil.Double)
                        .List();

                for (int i = 0; i < objects.Count; i++)
                {
                    var item = (object[])objects[i];
                    var customer = new CustomerInfo();
                    customer.CustomerName = item[0] == null ? "" : item[0].ToString();
                    customer.State = item[1] == null ? "" : ((StatesOfUSA)Convert.ToInt32(item[1].ToString())).ToString();
                    customer.City = item[2] == null ? "" : item[2].ToString();
                    customer.TotalInvoices = item[3] == null ? 0 : Convert.ToInt32(item[3]);
                    customer.TotalAmount = item[4] == null ? 0 : Convert.ToDouble(item[4]);
                    Customers.Add(customer);
                }
            }
            catch (Exception ex)
            {
            }
            return Customers;
        }

        //private void GetStoredProcedureText()
        //{
        //    try
        //    {
        //        var customers = session.CreateSQLQuery(TextSPForCustomers).AddScalar("Text", NHibernateUtil.String).List();
        //        var employees = session.CreateSQLQuery(TextSPForEmployees).AddScalar("Text", NHibernateUtil.String).List();
        //        for (int i = 0; i < customers.Count; i++)
        //        {
        //            var item = customers[i].ToString();
        //            this.TextSPCustomers.Add(item);
        //        }
        //        for (int i = 0; i < employees.Count; i++)
        //        {
        //            var item = employees[i].ToString();
        //            this.TextSPEmployees.Add(item);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //}
    }
}
