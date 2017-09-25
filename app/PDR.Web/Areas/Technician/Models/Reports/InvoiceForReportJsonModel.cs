using System.Globalization;
using System.Linq;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Webstorage;

namespace PDR.Web.Areas.Technician.Models.Reports
{
    public class InvoiceForReportJsonModel : IJsonModel
    {
        private readonly Employee currentEmployye = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();

        public InvoiceForReportJsonModel(Invoice invoice)
        {
            this.Id = invoice.Id;
            this.CreationDate = invoice.CreationDate.ToString("MM/dd/yyyy");
            this.InvoiceAmount = invoice.RepairOrder.TotalAmount.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
            this.PaidAmount = invoice.PaidSum.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
            this.InvoiceStatus = invoice.Status.ToString();
            this.CustomerName = invoice.RepairOrder.Estimate.Customer.GetCustomerName();
            var teamEmployee = this.currentEmployye as TeamEmployee;
            if (teamEmployee != null)
            {
                var currentEmployeeCommission = teamEmployee.TeamEmployeePercents.SingleOrDefault(x => x.RepairOrder == invoice.RepairOrder);
                if (currentEmployeeCommission != null)
                {
                    this.Commission = currentEmployeeCommission.GetCommissionSum(invoice).ToString("C", CultureInfo.CreateSpecificCulture("en-US")); ;
                }
            }

            if (this.currentEmployye is Domain.Model.Users.Admin)
            {
                this.Commission = invoice.GetCommission().ToString("C", CultureInfo.CreateSpecificCulture("en-US")); ;
            }

            foreach (var teamEmp in invoice.RepairOrder.TeamEmployeePercents.Select(x=> x.TeamEmployee).OrderBy(x => x.Name))
            {
                var part = invoice.RepairOrder.TeamEmployeePercents
                    .SingleOrDefault(x => x.TeamEmployee.Id == teamEmp.Id).EmployeePart;
                this.Employee += string.Format("{0} ({1}%);\n", teamEmp.Name, part);
            }
        }

        public long Id { get; set; }

        public string CreationDate { get; set; }

        public string CustomerName { get; set; }

        public string InvoiceAmount { get; set; }

        public string PaidAmount { get; set; }

        public string InvoiceStatus { get; set; }

        public string Commission { get; set; }

        public string Employee { get; set; }
    }
}