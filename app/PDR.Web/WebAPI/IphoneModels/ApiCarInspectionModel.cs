using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PDR.Domain.Model;
using PDR.Domain.Model.CustomLines;
using PDR.Domain.Model.Enums;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiCarInspectionModel
    {
        public ApiCarInspectionModel()
        {
        }

        public ApiCarInspectionModel(CarInspection carInspection)
        {
            if (carInspection != null)
            {
                this.Id = carInspection.Id;
                this.Name = carInspection.Name;
                this.DentsAmount = carInspection.DentsAmount;
                this.AverageSize = carInspection.AverageSize;
                this.DentsCost = carInspection.DentsCost;
                this.OversizedRoof = carInspection.OversizedRoof;
                this.Aluminium = carInspection.Aluminium;
                this.DoubleMetal = carInspection.DoubleMetal;
                this.CorrosionProtection = carInspection.CorrosionProtection;
                this.CorrosionProtectionCost = carInspection.CorrosionProtectionCost;
                this.OptionsPercent = carInspection.OptionsPercent;
                this.ChoosenEffortItems = carInspection.ChosenEffortItems.Select(x => new ApiChoosenEffortItemModel(x)).ToList();
                this.CustomLines = carInspection.CustomLines.Where(x => x is CustomCarInspectionLine).Select(x => new ApiCustomFieldModel(x)).ToList();
                var oversizedDent = carInspection.CustomLines.LastOrDefault(x => x is OversizedDentsLine);
                this.OversizedDents = new ApiOversizedDents(oversizedDent);
                this.PartsTotal = carInspection.GrandTotalWithEffortAndCorrosionProtection();
                this.PriorDamage = carInspection.PriorDamage;
                this.QuickCost = carInspection.QuickCost;
            }
        }

        public long Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PartOfBody Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TotalDents DentsAmount { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AverageSize AverageSize { get; set; }

        public double DentsCost { get; set; } 

        public bool OversizedRoof { get; set; }

        public double? QuickCost { get; set; }

        public bool Aluminium { get; set; }

        public bool DoubleMetal { get; set; }

        public bool CorrosionProtection { get; set; }

        public double CorrosionProtectionCost { get; set; }

        public double OptionsPercent { get; set; } 

        public IList<ApiChoosenEffortItemModel> ChoosenEffortItems { get; set; }

        public IList<ApiCustomFieldModel> CustomLines { get; set; }

        public ApiOversizedDents OversizedDents { get; set; }

        public double PartsTotal { get; set; }

        public string PriorDamage { get; set; }
    }
}