using System;
using System.Linq;
using Iesi.Collections.Generic;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.CustomLines;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Effort;
using PDR.Domain.Model.Enums;

using SmartArch.Data.Proxy;

namespace PDR.Domain.Model
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class CarInspection : CompanyEntity
	{
		public virtual PartOfBody Name { get; set; }

		public virtual TotalDents DentsAmount { get; set; }

		public virtual AverageSize AverageSize { get; set; }

        public virtual double DentsCost { get; set; } 

        public virtual bool OversizedRoof { get; set; } 

        public virtual bool Aluminium { get; set; }

		public virtual bool DoubleMetal { get; set; }

		public virtual bool CorrosionProtection { get; set; }

        public virtual double CorrosionProtectionCost { get; set; }

        public virtual double OptionsPercent { get; set; } 

        public virtual ISet<EffortItem> EffortItems { get; set; }

		public virtual ISet<ChosenEffortItem> ChosenEffortItems { get; set; }

        public virtual ISet<CustomLine> CustomLines { get; set; }

        public virtual Estimate Estimate { get; set; }

        public virtual double PartsTotal { get; set; }

        public virtual string PriorDamage { get; set; }

        public virtual double? QuickCost { get; set; }

        public virtual bool HasAlert { get; set; }

        //////////////////////////////////////////////////////

        public virtual double AdditionalPercentsAmount { get; set; }

        public virtual double AluminiumAmount { get; set; }

        public virtual double DoubleMetallAmount { get; set; }

        public virtual double OversizedRoofAmount { get; set; }

        /////////////////////////////////////////////////////

        public virtual double OversizedDentsCost
        {
            get
            {
                var singleOrDefault = this.CustomLines.SingleOrDefault(x => x is OversizedDentsLine);
                if (singleOrDefault != null)
                {
                    return singleOrDefault.Cost;
                }

                return 0;
            }
        }

        public virtual double CalculateAluminiumSum()
        {
            return this.Aluminium ? this.DentsCost * this.Estimate.Matrix.AluminiumPanel * 0.01 : 0;
        }

        public virtual double CalculateDoubleMetallSum()
        {
            return this.DoubleMetal ? this.DentsCost * this.Estimate.Matrix.DoubleLayeredPanels * 0.01 : 0;
        }

        public virtual double OversizedRoofSum()
        {
            return this.OversizedRoof ? this.DentsCost * this.Estimate.Matrix.OversizedRoof * 0.01 : 0;
        }

        public virtual double AdditionalPercentsSum()
        {
            var percents = 
                this.AluminiumAmount
                + this.DoubleMetallAmount
                + this.OversizedRoofAmount;
            var max = this.DentsCost * this.Estimate.Matrix.Maximum * 0.01;
            return percents >= max ? max : percents;
        }

        public virtual double GrandTotal() 
        {
                return 
                    this.DentsCost +
                    this.AdditionalPercentsSum() + 
                    this.CustomLines.Select(x => x.Cost).Sum();
        }

        public virtual double GrandTotalWithEfforts()
        {
            return this.GrandTotal() + this.GetLaborSum();
        }

        public virtual double GrandTotalWithCorrosionProtection()
        {
            if(this.CorrosionProtection)
            {
                var totalCorProtect = this.Estimate.CarInspections.Select(x => x.CorrosionProtectionCost).Sum();
                return this.Estimate.EstMaxCorProtect > totalCorProtect
                           ? this.GrandTotal() + this.CorrosionProtectionCost
                           : this.GrandTotal();
            }
            return this.GrandTotal();
        }

        public virtual double GrandTotalWithEffortAndCorrosionProtection()
        {
            return this.GrandTotalWithCorrosionProtection() + this.GetLaborSum();
        }

        public virtual double GetLaborSum()
        {
            var totalHours = this.ChosenEffortItems.Select(c => c.Hours).Sum();
            return Math.Round(totalHours * (this.Estimate.NewLaborRate.HasValue ? (double)this.Estimate.NewLaborRate : this.Estimate.CurrentHourlyRate), 2);
        }

        public virtual double GetDocumentLaborSum()
        {
            var totalHours = this.ChosenEffortItems.Select(c => c.Hours).Sum();
            return Math.Round(totalHours * (this.Estimate.NewLaborRate.HasValue ? (double)this.Estimate.NewLaborRate : this.Estimate.EstHourlyRate), 2);
        }

        public CarInspection()
        {
            
        }

        public CarInspection(bool isNewEntity = false)
        {
            this.EffortItems = new HashedSet<EffortItem>();
            this.ChosenEffortItems = new HashedSet<ChosenEffortItem>();
            this.CustomLines = new HashedSet<CustomLine>();
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }

        }
	}
}
