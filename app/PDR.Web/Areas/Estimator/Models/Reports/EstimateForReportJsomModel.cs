using System.Globalization;

using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Estimator.Models.Reports
{
    public class EstimateForReportJsomModel : IJsonModel
    {
        public EstimateForReportJsomModel(Estimate estimate)
        {
            this.Id = estimate.Id;
            this.CreationDate = estimate.CreationDate.ToString("MM/dd/yyyy");
            this.TotalAmount = estimate.TotalAmount.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
            this.EstimateStatus = estimate.EstimateStatus.ToString();
            this.Customer_LastName = estimate.Customer.GetCustomerName();
            this.Employee = estimate.Employee.With(x => x.Name);
        }

        public long Id { get; set; }

        public string CreationDate { get; set; }

        public string Customer_LastName { get; set; }

        public string TotalAmount { get; set; }

        public string EstimateStatus { get; set; }

        public string Employee { get; set; }
    }
}