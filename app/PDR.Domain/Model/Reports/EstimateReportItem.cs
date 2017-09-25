using System;
using PDR.Domain.Helpers;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;

namespace PDR.Domain.Model.Reports
{
    public class EstimateReportItem : CompanyEntity
    {
        protected EstimateReportItem()
        {
        }

        public EstimateReportItem(Estimate estimate)
        {
            this.EstimateID = estimate.Id;
            this.CreationDate = estimate.CreationDate;
            this.CustomerName = estimate.Customer.GetCustomerName();
            this.CalculatedSum = estimate.TotalAmount;
            this.UpdatedSum = estimate.TotalAmount;
            this.Status = estimate.EstimateStatus;
        }

        public virtual long EstimateID { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual string CustomerName { get; set; }

        public virtual double CalculatedSum { get; set; }

        public virtual double UpdatedSum { get; set; }

        public virtual EstimateStatus Status { get; set; }

        public virtual EstimateReport EstimateReport { get; set; }
    }
}