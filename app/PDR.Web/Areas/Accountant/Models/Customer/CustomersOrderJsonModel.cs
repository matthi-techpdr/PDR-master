using System.Globalization;
using System.Linq;

using PDR.Domain.Model;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Accountant.Models.Customer
{
    public class CustomersOrderJsonModel : IJsonModel
	{
        public CustomersOrderJsonModel(RepairOrder repairOrder)
        {
            this.Id = repairOrder.Id;
            this.TotalAmount = repairOrder.TotalAmount.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
            this.CarsMakeModelYear = string.Format(
                "{0}/{1}/{2}",
                repairOrder.Estimate.Car.Year,
                repairOrder.Estimate.Car.Make,
                repairOrder.Estimate.Car.Model);
            this.CreationDate = repairOrder.CreationDate.ToString("MM/dd/yyyy");
            this.Status = repairOrder.RepairOrderStatus.ToString();
            var names = repairOrder.TeamEmployeePercents.Select(x => x.TeamEmployee.Name).ToList();
            this.TechnicianName = string.Join(", ", names);
        }

        public long Id { get; set; }

		public string CreationDate { get; set; }

		public string CarsMakeModelYear { get; set; }

        public string TechnicianName { get; set; }

        public string TotalAmount { get; set; }

		public string Status { get; set; }
	}
}