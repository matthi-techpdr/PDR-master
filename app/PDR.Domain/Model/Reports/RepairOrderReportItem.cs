using System;

using PDR.Domain.Helpers;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;

namespace PDR.Domain.Model.Reports
{
    public class RepairOrderReportItem : CompanyEntity
    {
        protected RepairOrderReportItem()
        {
        }

        public RepairOrderReportItem(RepairOrder repairOrder)
        {
            this.RepairOrderID = repairOrder.Id;
            this.CreationDate = repairOrder.CreationDate;
            this.CustomerName = repairOrder.Estimate.Customer.GetCustomerName();
            this.TotalSum = repairOrder.TotalSum;
            this.Status = repairOrder.RepairOrderStatus;
        }

        public virtual long RepairOrderID { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual string CustomerName { get; set; }

        public virtual double TotalSum { get; set; }

        public virtual RepairOrderStatuses Status { get; set; }

        public virtual RepairOrderReport RepairOrderReport { get; set; } 
    }
}