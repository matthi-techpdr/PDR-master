using System;
using System.Globalization;
using System.Linq;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Webstorage;

namespace PDR.Web.Areas.Common.Models
{
    using PDR.Domain.Model.Enums;
    using PDR.Domain.Model.Enums.Extensions;

    public class InvoiceJsonModelBase : IJsonModel
    {
        private readonly Employee currentEmployee = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();

        public InvoiceJsonModelBase(Invoice invoice)
        {
            this.Id = invoice.Id;
            this.CreationDate = invoice.CreationDate.ToString("MM/dd/yyyy");
            this.Customer_LastName = invoice.RepairOrder.Estimate.Customer.GetCustomerName();
            this.TotalAmount = invoice.InvoiceSum.ToString("C", CultureInfo.CreateSpecificCulture("en-US")); ;
            this.PaidSum = invoice.PaidSum.ToString("C", CultureInfo.CreateSpecificCulture("en-US")); ;
            this.InvoiceStatus = invoice.Status.GetDescription();
            this.CarInfo = invoice.RepairOrder.Estimate.Car != null
                               ? string.Format(
                                               "{0}/{1}/{2}",
                                               invoice.RepairOrder.Estimate.Car.Year,
                                               invoice.RepairOrder.Estimate.Car.Make,
                                               invoice.RepairOrder.Estimate.Car.Model)
                               : string.Empty;

            this.commission = InvoiceHelper.GetCommission(invoice, currentEmployee);

            this.New = invoice.New;
            if (invoice.RepairOrder.TeamEmployee != null)
            {
                var allEmployess = invoice.RepairOrder.TeamEmployeePercents.Select(x => x.TeamEmployee.Name).OrderBy(x => x).ToList();
                this.Employee = string.Join(",", allEmployess);
            }
        }

        protected double commission;

        public long Id { get; set; }

        public string CreationDate { get; set; }

        public string Customer_LastName { get; set; }

        public string Employee { get; set; }

        public string CarInfo { get; set; } 

        public string TotalAmount { get; set; }

        public string PaidSum { get; set; }

        public string InvoiceStatus { get; set; }

        public string Commission
        {
            get
            {
                return (this.commission >= 0 ? commission : 0).ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
            }
        }

        public bool New { get; set; }
    }
}