using Fasterflect;

using PDR.Domain.Helpers;
using PDR.Domain.Model.Base;

namespace PDR.Web.Areas.Common.Models
{
    public class ReportItem
    {
        public ReportItem(IReportable reportableEntity)
        {
            this.CreationDate = reportableEntity.CreationDate.ToShortDateString();
            this.Id = this.GetType().GetPropertyValue("Id").ToString();
            this.CustomerName = reportableEntity.Customer.GetCustomerName();
            this.Amount = string.Format("{0:0.00}", reportableEntity.TotalAmount);
            this.Status = reportableEntity.RStatus;
        }

        public string CreationDate { get; set; }

        public string Id { get; set; }

        public string CustomerName { get; set; }

        public string Amount { get; set; }

        public string Status { get; set; }
    }
}