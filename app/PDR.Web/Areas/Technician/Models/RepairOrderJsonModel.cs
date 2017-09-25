using System.Globalization;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Webstorage;
using PDR.Domain.Contracts.Repositories;

using System.Linq;

using SmartArch.Data.Proxy;

namespace PDR.Web.Areas.Technician.Models
{
    public class RepairOrderJsonModel : IJsonModel
    {
        private readonly Employee employee = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
        private readonly ICompanyRepository<Invoice> invoicesRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Invoice>>();

        public RepairOrderJsonModel()
        {
        }

        ////имена свойств не редактировать!!!
        public RepairOrderJsonModel(RepairOrder repairOrder)
        {
            this.Id = repairOrder.Id;
            this.CreationDate = repairOrder.CreationDate.ToString("MM/dd/yyyy");
            this.CustomerName = repairOrder.Customer.GetCustomerName();
            this.TotalAmount = repairOrder.TotalAmount.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
            this.RoStatus = repairOrder.RepairOrderStatus.ToString();
            this.EditableStatus = repairOrder.EditedStatus.ToString();
            this.RepairOrderStatus = repairOrder.EditedStatus == EditedStatuses.EditingReject 
                ? repairOrder.RepairOrderStatus.ToString() : repairOrder.EditedStatus.ToString();
            this.CarInfo = repairOrder.Estimate.Car != null
                               ? string.Format(
                                               "{0}/{1}/{2}",
                                               repairOrder.Estimate.Car.Year,
                                               repairOrder.Estimate.Car.Make,
                                               repairOrder.Estimate.Car.Model)
                               : string.Empty;
            var teamEmployee = this.employee as TeamEmployee;
            var invoice = invoicesRepository.FirstOrDefault(x => x.RepairOrder.Id == repairOrder.Id);
            this.IsPaidInvoice = invoice != null && (invoice.Status == InvoiceStatus.Paid || invoice.Status == InvoiceStatus.PaidInFull);
            if (repairOrder.RepairOrderStatus != RepairOrderStatuses.Open)
            {
                if (teamEmployee != null)
                {
                    if (teamEmployee.Role != UserRoles.RITechnician)
                    {
                        var teamEmpPercent =
                            teamEmployee.TeamEmployeePercents.SingleOrDefault(x => x.RepairOrder.Id == repairOrder.Id);
                        if (teamEmpPercent != null)
                        {
                            this.Percent = teamEmpPercent.EmployeePart + "%";
                        }
                    }
                    else
                    {
                        var countRi = repairOrder.TeamEmployeePercents.Count(x => x.TeamEmployee.Role == UserRoles.RITechnician);
                        double sum = 0;
                        if (repairOrder.IsFlatFee.HasValue)
                        {
                            var riPayment = repairOrder.Estimate.CarInspections.Sum(x => x.GetDocumentLaborSum()) <= 0 
                            ? 0 : repairOrder.Estimate.CarInspections.Sum(x => x.GetDocumentLaborSum())*(1 - repairOrder.Estimate.DocumentDiscount);

                            sum = repairOrder.IsFlatFee.Value
                                      ? (repairOrder.Payment.HasValue ? repairOrder.Payment.Value : 0)
                                      : riPayment;
                        }
                        this.Share = (sum / countRi).ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
                    }
                }
            }

            var names = repairOrder.TeamEmployeePercents.Select(x => x.TeamEmployee.Name).OrderBy(x => x).ToList();

            this.Employee = string.Join(",", names);
            this.New = repairOrder.New;

            this.HasOrderSignature = repairOrder.Estimate.Customer.CustomerType == CustomerType.Retail ||
                                        repairOrder.Estimate.Customer.ToPersist<WholesaleCustomer>().OrderSignature;
        }

        public long Id { get; set; }

        public string CreationDate { get; set; }

        public bool New { get; set; }

        public string CustomerName { get; set; }

        public string CarInfo { get; set; }

        public string TotalAmount { get; set; }

        public string RepairOrderStatus { get; set; }

        public string Percent { get; set; }

        public string Employee { get; set; }

        public bool HasOrderSignature { get; set; }

        public string Share { get; set; }

        public string EditableStatus { get; set; }

        public string RoStatus { get; set; }

        public bool IsPaidInvoice { get; set; }
    }
}