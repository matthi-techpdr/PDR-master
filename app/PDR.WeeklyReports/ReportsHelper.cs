using System;
using System.Collections.Generic;
using NHibernate;
using PDR.Domain.Model;

namespace PDR.WeeklyReports
{
    using System.Linq;

    public class ReportsHelper
    {
        private readonly ISession session;

        private CustomCompanyRepository<Invoice> invoicesRepository;

        private CustomCompanyRepository<RepairOrder> repairOrdersRepository;

        private DateTime DateFrom { get; set; }

        private DateTime DateTo { get; set; }

        public IList<EmployeeInfo> Employees { get; set; }

        public IList<CustomerInfo> Customers { get; set; }

        public ReportsHelper(ISession session, DateTime dateFrom, DateTime dateTo)
        {
            this.session = session;
            this.invoicesRepository = new CustomCompanyRepository<Invoice>(session);
            this.repairOrdersRepository = new CustomCompanyRepository<RepairOrder>(session);
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.Employees = new List<EmployeeInfo>();
            this.Customers = new List<CustomerInfo>();
            this.GetEmployeeInfo();
            this.GetCustomerInfo();
        }

        private void GetEmployeeInfo()
        {
            var invoicesFromPeriod = invoicesRepository.All.Where(x => x.CreationDate >= DateFrom && x.CreationDate <= DateTo
                && !x.Customer.Teams.Any(y => y.Title.ToLower().Contains("test") || y.Title.ToLower().Contains("training")) &&
                x.TeamEmployee.Id != 229081088 && x.TeamEmployee.Id != 498958337).ToList();

            var employeesIds = invoicesFromPeriod.Select(x => x.Id).Distinct().ToArray();

            for (int i = 0; i < employeesIds.Length; i++)
            {
                var currentEmployee = invoicesFromPeriod.FirstOrDefault(x => x.TeamEmployee.Id == employeesIds[i]).TeamEmployee;
                if (currentEmployee != null)
                {
                    //var employee = new EmployeeInfo()
                    //                   {
                    //                       EmployeeName = currentEmployee.Name,
                    //                       EmployeeRole = currentEmployee.Role.ToString(),
                    //                       TotalInvoices =
                    //                           invoicesFromPeriod.Count(
                    //                               x => x.TeamEmployee.Id == currentEmployee.Id),
                    //                       TotalAmount =
                    //                           invoicesFromPeriod.Where(
                    //                               x => x.TeamEmployee.Id == currentEmployee.Id)
                    //                           .Sum(y => y.TeamEmployee.TeamEmployeePercents)
                    //                   };
                }
            }
        }

        private void GetCustomerInfo()
        {
        }
    }
}
