using System.Globalization;
using System.Linq;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Accountant.Models.Customer
{
    public class CustomersInvoiceJsonModel : IJsonModel
	{
        public CustomersInvoiceJsonModel(Domain.Model.Invoice invoice)
        {
            this.Id = invoice.Id;
            this.CreationDate = invoice.CreationDate.ToString("MM/dd/yyyy");
            this.CarsMakeModelYear = string.Format("{0}/{1}/{2}", invoice.RepairOrder.Estimate.Car.Year, invoice.RepairOrder.Estimate.Car.Make, invoice.RepairOrder.Estimate.Car.Model);
            this.InvoiceSum = invoice.InvoiceSum.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
            this.PaidSum = invoice.PaidSum.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
            this.InvoiceStatus = invoice.Status.ToString();
            var names = invoice.RepairOrder.TeamEmployeePercents.Select(x => x.TeamEmployee.Name).ToList();
            this.TechnicianName = string.Join(", ", names);
        }

	    public long Id { get; set; }

	    public string CreationDate { get; set; }

		public string CarsMakeModelYear { get; set; }

		public string InvoiceSum { get; set; }

		public string PaidSum { get; set; }

        public string TechnicianName { get; set; }

		public string InvoiceStatus { get; set; }
	}
}