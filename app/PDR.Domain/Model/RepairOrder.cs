using System;
using System.Linq;
using Iesi.Collections.Generic;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Photos;
using PDR.Domain.Model.Users;
using SmartArch.Core.Helpers;

namespace PDR.Domain.Model
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Services.Webstorage;

    public class RepairOrder : CompanyEntity, IReportable
    {
        public RepairOrder(bool isNewEntity = false)
        {
            this.Supplements = new HashedSet<Supplement>();
            this.AdditionalPhotos = new HashedSet<AdditionalCarPhoto>();
            this.CreationDate = SystemTime.Now();
            this.TeamEmployeePercents = new HashedSet<TeamEmployeePercent>();
            this.New = true;
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public RepairOrder(){}

        public virtual DateTime CreationDate { get; set; }

        public virtual double TotalAmount
        {
            get
            {
                var sum = this.RepairOrderSum - this.DiscountSum + this.TaxSum;
                return Math.Round(sum, 2, MidpointRounding.AwayFromZero);
            }
        }

        public virtual double RepairOrderSumWithoutLaborSum
        {
            get { return this.RepairOrderSumWithoutDiscountAndTax - this.Estimate.GetLaborSum(); }
        }

        public virtual double DocumentRepairOrderSumWithoutLaborSum
        {
            get { return this.RepairOrderSumWithoutDiscountAndTax - this.Estimate.GetDocumentLaborSum(); }
        }

        public virtual double TaxSum
        {
            get
            {
                var tax = (this.RepairOrderSum - this.DiscountSum) * this.Estimate.CurrentLaborTax;
                return Math.Round(tax, 2, MidpointRounding.AwayFromZero);
            }
        }

        public virtual double RepairOrderSum
        {
            get
            {
                var sum = this.WorkByThemselve ? this.RepairOrderSumWithoutLaborSum : this.RepairOrderSumWithoutDiscountAndTax;
                return Math.Round(sum - this.AdditionalDiscount, 2, MidpointRounding.AwayFromZero);
            }
        }


        public virtual double DocumentRepairOrderSum
        {
            get
            {
                var sum = this.WorkByThemselve ? this.DocumentRepairOrderSumWithoutLaborSum : this.RepairOrderSumWithoutDiscountAndTax;
                return Math.Round(sum - this.AdditionalDiscount, 2, MidpointRounding.AwayFromZero);
            }
        }

        public virtual double DiscountSum
        {
            get
            {
                var disc = this.RepairOrderSum * this.Discount;
                return Math.Round(disc, 2, MidpointRounding.AwayFromZero);
            }
        }

        public virtual double DocumentDiscountSum
        {
            get
            {
                var disc = this.DocumentRepairOrderSum * this.DocumentDiscount;
                return Math.Round(disc, 2, MidpointRounding.AwayFromZero);
            }
        }

        public virtual double Discount
        {
            get { return this.Estimate.Discount > 0 ? this.Estimate.Discount : this.RetailDiscount * 0.01; }
        }

        public virtual double DocumentDiscount
        {
            get { return this.Estimate.DocumentDiscount > 0 ? this.Estimate.DocumentDiscount : this.RetailDiscount * 0.01; }
        }

        public virtual double SupplementsSum
        {
            get { return this.Supplements.Sum(x => x.Sum); }
        }

        public virtual double RepairOrderSumWithoutDiscountAndTax
        {
            get { return this.Estimate.CleanTotalAmount + this.SupplementsSum; }
        }

        public virtual double RoSubtotal
        {
           get
           {
               var subtotal = this.Estimate.Subtotal + this.SupplementsSum;
               return Math.Round(subtotal, 2, MidpointRounding.AwayFromZero);
           }
        }

        public virtual bool New { get; set; }

        public virtual bool IsConfirmed { get; set; }

        public virtual bool IsInvoice { get; set; }

        public virtual bool SupplementsApproved { get; set; }

        public virtual RepairOrderStatuses RepairOrderStatus { get; set; }

        public virtual EditedStatuses EditedStatus { get; set; }

        public virtual Estimate Estimate { get; set; }

        public virtual TeamEmployee TeamEmployee { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ISet<Supplement> Supplements { get; set; }

        public virtual ISet<AdditionalCarPhoto> AdditionalPhotos { get; set; }

        public virtual ISet<TeamEmployeePercent> TeamEmployeePercents { get; set; }

        public virtual RoCustomerSignature RoCustomerSignature { get; set; }

        public virtual bool WorkByThemselve { get; set; }

        public virtual double AdditionalDiscount { get; set; }

        public virtual int RetailDiscount { get; set; }

        public virtual bool? IsFlatFee { get; set; }

        public virtual double? Payment { get; set; }
    }
}
