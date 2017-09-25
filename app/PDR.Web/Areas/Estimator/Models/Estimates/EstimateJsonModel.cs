using System;
using System.Globalization;
using System.Linq;

using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Services.Grid.Interfaces;

using SmartArch.Data.Proxy;

namespace PDR.Web.Areas.Estimator.Models.Estimates
{
    public class EstimateJsonModel : IJsonModel
    {
        public EstimateJsonModel(Estimate estimate)
        {
            this.Id = estimate.Id.ToString();
            this.CustomerName = estimate.Customer.GetCustomerName();
            this.CreationDate = estimate.CreationDate.ToString("MM/dd/yyyy");
            this.CarInfo = estimate.Car != null
                               ? string.Format("{0}/{1}/{2}", estimate.Car.Year, estimate.Car.Make, estimate.Car.Model)
                               : string.Empty;

            this.TotalAmount = GetEstimateSum(estimate);
            this.EstimateStatus = estimate.EstimateStatus.ToString();
            this.CustomerType = estimate.Customer.CustomerType.ToString();
            this.Employee = estimate.Employee.Name;
            this.HasInsurance = estimate.Customer.CustomerType == Domain.Model.Enums.CustomerType.Retail || estimate.Customer.ToPersist<WholesaleCustomer>().Insurance;
            this.HasEstimateSignature = estimate.Customer.CustomerType == Domain.Model.Enums.CustomerType.Retail ||
                                        estimate.Customer.ToPersist<WholesaleCustomer>().EstimateSignature;
            this.Type = estimate.Type.ToString();
            this.New = estimate.New;
        }

        public string CreationDate { get; set; }

        public string Id { get; set; }

        public bool New { get; set; }

        public string CustomerName { get; set; }

        public bool HasInsurance { get; set; }

        public string CarInfo { get; set; }

        public string TotalAmount { get; set; }

        public string EstimateStatus { get; set; }

        public string CustomerType { get; set; }

        public string Employee { get; set; }

        public bool HasEstimateSignature { get; set; }

        public string Type { get; set; }

        private static string GetEstimateSum(Estimate estimate)
        {
            var total = Math.Round(estimate.TotalAmount, 2);
            switch (estimate.Type)
            {
                case EstimateType.ExtraQuick:
                    total = estimate.ExtraQuickCost.HasValue ? Math.Round(estimate.ExtraQuickCost.Value, 2) : 0;
                    break;
                case EstimateType.Quick:
                    total = Math.Round(estimate.CarInspections.Where(insp => insp.QuickCost.HasValue).Sum(insp => insp.QuickCost.Value), 2);
                    break;
            }

            return total.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
        }
    }
}