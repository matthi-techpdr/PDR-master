using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PDR.Domain.Model;
using PDR.Domain.Services.TempImageStorage;
using PDR.Web.Areas.Estimator.Models.Estimates;
using PDR.Web.Areas.Technician.Models.RepairOrders;
using PDR.Web.Core.Helpers;
using PDR.Domain.Model.Enums;

namespace PDR.Web.Areas.Technician.Models
{
    public class RepairOrderModel : IPhotoContainer
    {
        public RepairOrderModel()
        {
            this.UploadPhotos = new List<ImageInfo>();
            this.StoredPhotos = new List<ImageInfo>();
            this.EstimateModel = new EstimateModel();
            this.SupplementModels = new List<SupplementModel>();
        }

        public RepairOrderModel(RepairOrder repairOrder) : this()
        {
            this.Id = repairOrder.Id;
            this.EstimateId = repairOrder.Estimate.Id;
            this.GrandTotal = repairOrder.TotalAmount.ToString();
            this.EstimateModel = new EstimateModel(repairOrder.Estimate);
            this.Statuses = repairOrder.RepairOrderStatus;
            this.AdditionalDiscount = Math.Round(repairOrder.AdditionalDiscount, 2, MidpointRounding.AwayFromZero).ToString();
            this.RetailDiscount = repairOrder.RetailDiscount.ToString();
            this.WorkByThemselve = repairOrder.WorkByThemselve;
            this.RepairOrderSumWithoutDiscountAndTax = repairOrder.RepairOrderSumWithoutDiscountAndTax.ToString();
            this.LaborSum = repairOrder.Estimate.GetLaborSum().ToString();
            this.EditedStatus = repairOrder.EditedStatus;
            this.NewHourlyRate = repairOrder.Estimate.NewLaborRate;
            this.CurrentHourlyRate = repairOrder.Estimate.CurrentHourlyRate;
            this.IsNewHourlyRate = repairOrder.Estimate.NewLaborRate.HasValue;
            this.SupplementsSum = repairOrder.SupplementsSum;
            if (repairOrder.AdditionalPhotos.Count > 0)
            {
                this.StoredPhotos.AddRange(repairOrder.AdditionalPhotos.ToList().ToImages());
            }

            if (repairOrder.Supplements.Count > 0)
            {
                this.SupplementModels.AddRange(repairOrder.Supplements.ToList().Select(x => new SupplementModel
                                                                                                {
                                                                                                    Sum = x.Sum > 0
                                                                                                    ? x.Sum.ToString()
                                                                                                    : string.Empty,
                                                                                                    Description = x.Description.Replace("<", "&lt;"),
                                                                                                    Id = x.Id
                                                                                                }));
            }
        }

        public long Id { get; set; }

        public string GrandTotal { get; set; }

        public RepairOrderStatuses Statuses { get; set; }

        public List<ImageInfo> UploadPhotos { get; set; }

        public List<ImageInfo> StoredPhotos { get; set; }

        public List<SupplementModel> SupplementModels { get; set; }
        [AllowHtml]
        public string BaseDescription { get; set; }

        public string BaseSum { get; set; }

        public bool IsConfirmed { get; set; }

        public bool SupplementsApproved { get; set; }

        public long EstimateId { get; set; }

        public EstimateModel EstimateModel { get; set; }

        public string RetailDiscount { get; set; }

        public string AdditionalDiscount { get; set; }

        public string RepairOrderSumWithoutDiscountAndTax { get; set; }

        public bool WorkByThemselve { get; set; }

        public string LaborSum { get; set; }

        public double? NewHourlyRate { get; set; }

        public double? CurrentHourlyRate { get; set; }

        public bool IsNewHourlyRate { get; set; }

        public EditedStatuses EditedStatus { get; set; }

        public double SupplementsSum { get; set; }
    }
}