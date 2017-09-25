using System.Globalization;

using PDR.Domain.Model;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Accountant.Models.Customer
{
	public class CustomerEstimateJsonModel : IJsonModel
	{
        public CustomerEstimateJsonModel(Estimate estimate)
        {
            this.Id = estimate.Id;
            this.CreationDate = estimate.CreationDate.ToString("MM/dd/yyyy");
            this.CarsMakeModelYear = string.Format("{0}/{1}/{2}", estimate.Car.Year, estimate.Car.Make, estimate.Car.Model);
            this.EmployeeName = estimate.Employee.Name;
            this.TotalAmount = estimate.TotalAmount.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
            this.Status = estimate.EstimateStatus.ToString();
        }

        public long Id { get; set; }

	    public string CreationDate { get; set; }

		public string CarsMakeModelYear { get; set; }

		public string EmployeeName { get; set; }

        public string TotalAmount { get; set; }

		public string Status { get; set; }
	}
}