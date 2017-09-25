using Microsoft.Practices.ServiceLocation;
using System.Linq;

using NLog;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Reports;
using PDR.Domain.Services.ObjectsComparer;

using SmartArch.Data;

namespace PDR.Web.Core.NLog.FileLoggers
{
    public static class ReportLogger
    {
        private static readonly Logger Loggger = LogManager.GetCurrentClassLogger();

        public static void New(Report report)
        {
            var type = LogHelper.SplitPropertyName(report.ReportType.ToString());
            var title = report.Title;
            var date = BuildDate(report.StartDate, report.EndDate);
            var adminOrManager = report.Employee.Role == UserRoles.Admin || report.Employee.Role == UserRoles.Manager;

            var customer = report.CustomerId.HasValue
                               ? ServiceLocator.Current.GetInstance<ICompanyRepository<Customer>>().Get(report.CustomerId.Value).GetCustomerName()
                               : "All customers";

            var team = report.TeamId.HasValue ? ServiceLocator.Current.GetInstance<ICompanyRepository<Team>>().Get(report.TeamId.Value) : null;

            var msg = string.Format("Generate {0} - {1}; {2}; {3}; ", type, title, date, customer);
            
            if (report.TeamId == 0)
            {
                msg += "Personal activities only;";
            }
            else if (team == null && adminOrManager)
            {
                msg += "All teams";
            }
            else if (team != null)
            {
                msg += team.Title;
            }

            Loggger.Info(msg);
        }

        private static string BuildDate(string startDate, string endDate)
        {
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                return string.Format("{0} - {1}", startDate, endDate);
            }

            if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
            {
                return string.Format("From {0}", startDate);
            }

            if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                return string.Format("To {0}", endDate);
            }
            
            return "Date are not specified";
        }

        public static void Print(Report report)
        {
            var type = LogHelper.SplitPropertyName(report.ReportType.ToString());
            var msg = string.Format("Export/Print PDF {0} - {1}.", type, report.Title);
            Loggger.Info(msg);
        }

        public static void PrintNonSaved(string type, string startDate, string endDate, string customerName, string teamName = null)
        {
            var date = BuildDate(startDate, endDate);
            var msg = string.Format("Export/Print PDF {0} report - {1}; {2}; ", type, date, customerName);
            if (teamName != null)
            {
                msg += teamName;
            }

            Loggger.Info(msg);
        }

        public static void Delete(Report report)
        {
            var type = LogHelper.SplitPropertyName(report.ReportType.ToString());
            var msg = string.Format("Delete {0} report - {1}.", type, report.Title);
            Loggger.Info(msg);
        }
    }
}