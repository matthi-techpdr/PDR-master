using System.Collections.Generic;
using System.Linq;

using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiRepairOrderModel : BaseIPhoneModel
    {
        public ApiRepairOrderModel()
        {
        }

        public ApiRepairOrderModel(RepairOrder repairOrder)
        {
            this.Id = repairOrder.Id;
            this.CreationDate = repairOrder.CreationDate.ToShortDateString();
            this.RepairOrderStatus = repairOrder.RepairOrderStatus.ToString();
            this.TotalAmount = repairOrder.TotalAmount;
            this.CustomerName = repairOrder.Customer.GetCustomerName();
            this.IsConfirmed = repairOrder.IsConfirmed;
            this.SupplementsApproved = repairOrder.SupplementsApproved;
            this.Estimate = new ApiEstimateModel(repairOrder.Estimate);
            this.Customer = new ApiCustomerModel(repairOrder.Customer);
            this.Supplements = repairOrder.Supplements.Select(x => new ApiSupplementModel(x)).ToList();
            this.PhotoIds = repairOrder.AdditionalPhotos.Select(x => new BaseIPhoneModel { Id = x.Id }).ToList();
            this.AssignedEmployees = repairOrder.TeamEmployeePercents.Select(x => new ApiAssignedEmployee(x)).ToList();
            this.New = repairOrder.New;
            this.WorkByThemselve = repairOrder.WorkByThemselve;
            this.RetailDiscount = repairOrder.RetailDiscount;
            this.AdditionalDiscount = repairOrder.AdditionalDiscount;
            this.EditedStatus = repairOrder.EditedStatus;
            this.IsRiPaymentAmount   = this.AssignedEmployees.Count(x => x.IsRiTechnician) > 0;
            this.IsFlatFee = IsRiPaymentAmount && (repairOrder.IsFlatFee ?? false); //Todo check logic
            this.PaymentFlatFee = IsRiPaymentAmount
                                  ? repairOrder.Payment ?? 0
                                  : 0; //Todo check logic
            var totalLaborSum = repairOrder.Estimate.CarInspections.Sum(x => x.GetLaborSum());
            this.PaymentRiOperations = totalLaborSum <= 0
                                       ? 0
                                       : totalLaborSum * (1 - repairOrder.Estimate.Discount);
            this.NewHourlyRate = repairOrder.Estimate.NewLaborRate.HasValue ? repairOrder.Estimate.NewLaborRate.Value : 0;
            this.CurrentHourlyRate = repairOrder.Estimate.CurrentHourlyRate;
            this.IsNewHourlyRate = repairOrder.Estimate.NewLaborRate.HasValue;
            CheckRiTechnician();
        }

        private void CheckRiTechnician()
        {
            double sumPart = 0;
            this.CurrentPayment = 0;

            var countRi = this.AssignedEmployees.Count(x => x.IsRiTechnician);
            if (countRi > 0 && this.IsFlatFee.HasValue)
            {
                if (this.IsFlatFee.Value && this.PaymentFlatFee.HasValue)
                {
                    sumPart = this.PaymentFlatFee.Value / countRi;
                }
                if (!this.IsFlatFee.Value && this.PaymentRiOperations.HasValue)
                {
                    sumPart = this.PaymentRiOperations.Value / countRi;
                }
                this.CurrentPayment = sumPart;
            }
        }

        public IList<BaseIPhoneModel> PhotoIds { get; set; }

        public string CreationDate { get; set; }

        public bool New { get; set; }

        public bool IsConfirmed { get; set; }

        public bool SupplementsApproved { get; set; }

        public string RepairOrderStatus { get; set; }

        public ApiEstimateModel Estimate { get; set; }

        public ApiEmployeeModel TeamEmployee { get; set; }

        public ApiCustomerModel Customer { get; set; }

        public IEnumerable<ApiSupplementModel> Supplements { get; set; }

        public double TotalAmount { get; set; }

        public IList<ApiAssignedEmployee> AssignedEmployees  { get; set; }

        public EditedStatuses EditedStatus { get; set; }

        public string CustomerName { get; set; }

        public bool WorkByThemselve { get; set; }

        public int RetailDiscount { get; set; }

        public double AdditionalDiscount { get; set; }

        public double? PaymentRiOperations { get; set; }

        public double? PaymentFlatFee { get; set; }

        public double? CurrentPayment { get; set; }

        public bool? IsFlatFee { get; set; }

        public bool IsRiPaymentAmount { get; set; }

        public double NewHourlyRate { get; set; }

        public double CurrentHourlyRate { get; set; }

        public bool IsNewHourlyRate { get; set; }
    }
}