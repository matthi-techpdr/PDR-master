using System.Web.Mvc;
using Iesi.Collections.Generic;
using PDR.Domain.Model.Enums;

namespace PDR.Web.Areas.Estimator.Models.Estimates
{
    public class CarInspectionsInfo
    {
        public CarInspectionsInfo()
        {
            this.CustomCarInspectionLines = new HashedSet<CustomCarInspectionLineModel>();
            this.EffortItems = new HashedSet<EffortItemModel>();
        }

        public long? Id { get; set; }

        public PartOfBody Name { get; set; }

        public AverageSize AverageSize { get; set; }

        public TotalDents TotalDents { get; set; }

        public double? DentsCost { get; set; }

        public long? OversizedDentsId { get; set; }

        public double? OversizedDents { get; set; }

        public int? AmountOversizedDents { get; set; }
        
        public bool OversizedRoof { get; set; }

        public bool Aluminium { get; set; }

        public bool DoubleMetal { get; set; }

        public bool CorrosionProtection { get; set; }

        public double? OptionsPersent { get; set; }

        public double? EffortLineCost { get; set; }

        public double? CorrosionProtectionCost { get; set; }

        public ISet<EffortItemModel> EffortItems { get; set; }

        public ISet<CustomCarInspectionLineModel> CustomCarInspectionLines { get; set; }

        public double? PartsTotal { get; set; }
        [AllowHtml]
        public string PriorDamage { get; set; }

        public string IsChanges { get; set; }

        public double? QuickCost { get; set; }

        public bool? HasAlert { get; set; }
    }
}