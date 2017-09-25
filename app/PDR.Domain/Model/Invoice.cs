using System;
using System.Linq;

using PDR.Domain.Helpers;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Model
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Services.Webstorage;

    public class Invoice : CompanyEntity, IReportableInvoice
    {
        public Invoice()
        {
        }

        public Invoice(RepairOrder repairOrder, TeamEmployee currentTeamEmployee, bool isNewEntity = false)
        {
            repairOrder.IsInvoice = true;
            repairOrder.IsConfirmed = true;
            repairOrder.RepairOrderStatus = RepairOrderStatuses.Finalised;
            repairOrder.EditedStatus = EditedStatuses.EditingReject;

            repairOrder.New = repairOrder.TeamEmployeePercents.Count > 1;

            this.CreationDate = DateTime.Now;
            this.RepairOrder = repairOrder;
            this.Status = InvoiceStatus.Unpaid;
            this.Customer = repairOrder.With(x => x.Estimate).With(x => x.Customer);
            this.InvoiceSum = Math.Round(repairOrder.TotalAmount, 2);
            this.TeamEmployee = currentTeamEmployee;
            this.PaidDate = new DateTime(1753, 1, 1);
            this.IsImported = false;
            this.IsDiscard = false;
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public virtual bool IsDiscard { get; set; }

        public virtual bool New { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual RepairOrder RepairOrder { get; set; }

        public virtual InvoiceStatus Status { get; set; }

        public virtual double InvoiceSum { get; set; }

        public virtual double PaidSum { get; set; }

        public virtual bool Archived { get; set; }

        public virtual TeamEmployee TeamEmployee { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual DateTime PaidDate { get; set; }

        public virtual bool IsImported { get; set; }

        public virtual double GetCommission()
        {
            var commission = this.RepairOrder.TeamEmployeePercents.Where(x => x.TeamEmployee.Role != UserRoles.RITechnician)
                .Select(x => x.EmployeePart * x.TeamEmployee.Commission).Sum() * this.GetPaidSumForComission() * 0.0001;
            var commissionRi = this.RepairOrder.IsFlatFee.HasValue
                                   ? this.RepairOrder.IsFlatFee.Value
                                         ? this.RepairOrder.Payment.HasValue
                                            ?this.RepairOrder.Payment.Value
                                            :0
                                         : this.RepairOrder.Estimate.CarInspections.Sum(x => x.GetLaborSum())
                                           - RepairOrder.Estimate.Discount
                                   : 0;

            return Math.Round(commission + commissionRi, 2);
        }

        public virtual double GetPaidSumForComission()
        {
            return this.Status == InvoiceStatus.Unpaid ? this.TotalAmount : this.PaidSum;
        }

        public virtual double TotalAmount
        {
            get
            {
                return this.InvoiceSum;
            }
        }

        public virtual string RStatus
        {
            get
            {
                return this.Status.ToString();
            }
        }

        public virtual bool IsFullyPaid
        {
            get
            {
                return this.InvoiceSum == this.PaidSum;
            }
        }

        public virtual double Balance
        {
            get
            {
                return this.InvoiceSum - this.PaidSum;
            }
        }
    }
}
