using System;
using System.Linq;

using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;

using SmartArch.Data.Proxy;

namespace PDR.Domain.Model
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    using NLog;

    public partial class Estimate 
    {
        public virtual double Subtotal { get; set; }

        public virtual double LaborSum { get; set; }

        public virtual double TaxSum { get; set; }

        public virtual double TotalAmount { get; set; }

        public virtual double CalculateTotalAmount()
        {
            double totalAmount = 0;
            if (this.Type == EstimateType.Normal)
            {
                totalAmount = this.Subtotal + this.GetLaborSum() + this.TaxSum;
            }

            if (this.Type == EstimateType.Quick)
            {
                totalAmount = this.CarInspections.Sum(x => x.QuickCost.HasValue ? x.QuickCost.Value : 0);
            }

            if (this.Type == EstimateType.ExtraQuick)
            {
                totalAmount = this.ExtraQuickCost.HasValue ? this.ExtraQuickCost.Value : 0;
            }


            return Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero);
        }

        public virtual double CleanTotalAmount
        {
            get { return this.TotalAmount - this.TaxSum; }
        }

        public virtual double CalculateSubtotal()
        {
            var carInspectionsSum = this.CarInspections.Select(x => x.GrandTotal()).Sum();
            var customLinesSum = this.CustomEstimateLines.Select(x => x.Cost).Sum();
            var totalCorrosionProtection = this.TotalCorrosionProtection();
            var subtotal = Math.Round(carInspectionsSum + customLinesSum + totalCorrosionProtection, 2, MidpointRounding.AwayFromZero);
            return Math.Round(subtotal, 2, MidpointRounding.AwayFromZero);
        }

        public virtual double TotalLaborHours
        {
            get
            {
                var laborHours = this.CarInspections.SelectMany(x => x.ChosenEffortItems.Select(c => c.Hours)).Sum();
                return Math.Round(laborHours, 2, MidpointRounding.AwayFromZero);
            }
        }

        public virtual double CurrentHourlyRate
        {
            get
            {
                var customer = this.Customer.ToPersist<WholesaleCustomer>();
                var affiliate = this.Affiliate;

                 //return this.NewLaborRate.HasValue 
                 //           ?  this.NewLaborRate.Value
                 //           : customer != null ? customer.HourlyRate :
                 //             affiliate != null ? affiliate.HourlyRate : 0;
                return customer != null 
                            ? customer.HourlyRate 
                            :affiliate != null ? affiliate.HourlyRate : 0;
            }
        }

        public virtual double CurrentLaborTax
        {
            get
            {
                var customer = this.Customer.ToPersist<WholesaleCustomer>();
                return customer != null ? customer.LaborRate * 0.01 : 0;
            }
        }

        public virtual double Discount
        {
            get
            {
                var customer = this.Customer.ToPersist<WholesaleCustomer>();
                return customer != null ? customer.Discount * 0.01 : 0;
            }
        }

        public virtual double DocumentDiscount
        {
            get { return this.EstDiscount * 0.01; }
        }

        public virtual double GetDiscountSum()
        {
            return Math.Round((this.Subtotal + this.GetLaborSum()) * this.Discount, 2, MidpointRounding.AwayFromZero);
        }

        public virtual double GetLaborSum()
        {
                //var customer = this.Customer.ToPersist<WholesaleCustomer>();
                //return customer == null ? Math.Round(this.TotalLaborHours * this.CurrentHourlyRate, 2)
                //                        : customer.WorkByThemselve
                //                            ? 0
                //                            : Math.Round(this.TotalLaborHours * this.CurrentHourlyRate, 2);
            return Math.Round(this.TotalLaborHours * (this.NewLaborRate.HasValue ? (double)this.NewLaborRate : this.CurrentHourlyRate), 2, MidpointRounding.AwayFromZero);
        }

        public virtual double GetDocumentLaborSum()
        {
            return Math.Round(this.TotalLaborHours * (this.NewLaborRate.HasValue ? (double)this.NewLaborRate : this.EstHourlyRate), 2, MidpointRounding.AwayFromZero);
        }

        public virtual double GetTaxSum()
        {
            var taxSum = (this.Subtotal + this.GetLaborSum()) * this.CurrentLaborTax;
            return taxSum;
        }

        public virtual double TotalCorrosionProtection()
        {
            var corrosion = this.CarInspections.Select(x => x.CorrosionProtectionCost).Sum();
            return corrosion <= this.Matrix.MaxCorrosionProtection ? corrosion : this.Matrix.MaxCorrosionProtection;
        }

        public virtual void SaveCalculation()
        {
                this.Subtotal = this.CalculateSubtotal();
                this.LaborSum = this.GetLaborSum();
                this.TaxSum = this.GetTaxSum();
                this.TotalAmount = this.CalculateTotalAmount();
                this.EstMaxPercent = this.Matrix.Maximum;
                this.EstMaxCorProtect = this.Matrix.MaxCorrosionProtection;
                this.EstCorProtectPart = this.Matrix.CorrosionProtectionPart;
                this.EstAluminiumPer = this.Matrix.AluminiumPanel;
                this.EstDoubleMetalPer = this.Matrix.DoubleLayeredPanels;
                this.EstOversizedRoofPer = this.Matrix.OversizedRoof;
                this.EstOversizedDents = this.Matrix.OversizedDents;

                if (this.Customer.CustomerType == CustomerType.Wholesale)
                {
                    var wholesale = this.Customer.ToPersist<WholesaleCustomer>();
                    this.EstHourlyRate = wholesale.HourlyRate;
                    this.EstLaborTax =  wholesale.LaborRate;
                    this.EstDiscount = wholesale.Discount;
                }
                else
                {
                    var affiliate = this.Affiliate; //RetailCustomer
                    this.EstLaborTax = affiliate.LaborRate; //0
                    this.EstHourlyRate = affiliate.HourlyRate;//retail.Company.DefaultHourlyRate;
                    this.EstDiscount = 0;
                }
                try
                {
                    this.EstLimitForBodyPart = this.Customer.Company.LimitForBodyPartEstimate;
                }
                catch (Exception exception)
                {
                    var ex = exception.Message;
                }
                this.DrawCar();
        }
    }
}