using System.Collections.Generic;
using System.Linq;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using SmartArch.Data.Proxy;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiEstimateModel : ApiEstimateLightModel
    {
        public ApiEstimateModel()
        {
        }

        public ApiEstimateModel(Estimate estimate, bool onlyView = false)
            : this()
        {
            if (estimate != null)
            {
                this.Id = estimate.Id;
                this.CreationDate = estimate.CreationDate.ToShortDateString();
                this.Archived = estimate.Archived;
                this.Status = estimate.EstimateStatus.ToString();
                this.CalculatedSum = estimate.TotalAmount;
                this.Car = new ApiCarModel(estimate.Car);
                this.Type = estimate.Type.ToString();
                this.Insurance = new ApiInsuranceModel(estimate.Insurance);
                this.New = estimate.New;
                this.EmployeeName = estimate.Employee.Name;
                this.LocationId = estimate.Affiliate != null ? estimate.Affiliate.Id : (long?)null;

                if (!onlyView)
                {
                    this.Signature = estimate.Signature;
                    this.VINStatus = estimate.VINStatus;
                    this.CalculatedSum = estimate.TotalAmount;
                    this.PriorDamages = estimate.PriorDamages;
                    this.EmployeeId = estimate.Employee.Id;
                    this.Archived = estimate.Archived;
                    this.Car = new ApiCarModel(estimate.Car);

                    if (estimate.Customer != null)
                    {
                        if (estimate.Customer.IsPersist<WholesaleCustomer>())
                        {
                            this.WholesaleCustomer =
                                new ApiWholesaleCustomerModel(estimate.Customer.ToPersist<WholesaleCustomer>());
                        }
                        else if (estimate.Customer.IsPersist<RetailCustomer>())
                        {
                            this.RetailCustomer =
                                new ApiRetailCustomerModel(estimate.Customer.ToPersist<RetailCustomer>());
                        }
                    }

                    if (estimate.Matrix != null)
                    {
                        this.Matrix = new ApiMatrixModel(estimate.Matrix);
                    }

                    this.CarInspections = estimate.CarInspections.Select(x => new ApiCarInspectionModel(x)).ToList();
                    this.CustomLines = estimate.CustomEstimateLines.Select(x => new ApiCustomFieldModel(x)).ToList();
                    
                    this.ExtraQuickCost = estimate.ExtraQuickCost;
                    this.PhotoIds = estimate.Photos.ToList().Select(x => new BaseIPhoneModel { Id = x.Id }).ToList();
                }
            }
        }

        public string CreationDate { get; set; }

        public long? EmployeeId { get; set; }

        public bool? Signature { get; set; }

        public bool? Archived { get; set; }

        public IList<BaseIPhoneModel> PhotoIds { get; set; } 

        public double? CalculatedSum { get; set; }

        public string PriorDamages { get; set; }

        public ApiCarModel Car { get; set; }

        public string VINStatus { get; set; }

        public ApiWholesaleCustomerModel WholesaleCustomer { get; set; }

        public ApiRetailCustomerModel RetailCustomer { get; set; }

        public ApiMatrixModel Matrix { get; set; }

        public ApiInsuranceModel Insurance { get; set; }

        public string Status { get; set; }

        public bool New { get; set; }

        public long? CustomerId { get; set; }

        public IList<ApiCarInspectionModel> CarInspections { get; set; }

        public IList<ApiCustomFieldModel> CustomLines { get; set; }

        public long? MatrixId { get; set; }

        public string Type { get; set; }

        public double? ExtraQuickCost { get; set; }

        public string EmployeeName { get; set; }

        public long? LocationId { get; set; }
    }
}