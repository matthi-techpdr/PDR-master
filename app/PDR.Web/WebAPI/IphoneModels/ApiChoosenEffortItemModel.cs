using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using PDR.Domain.Helpers;
using PDR.Domain.Model.Effort;
using PDR.Domain.Model.Enums;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiChoosenEffortItemModel
    {
        public ApiChoosenEffortItemModel()
        {
        }

        public ApiChoosenEffortItemModel(ChosenEffortItem item)
        {
            if (item != null)
            {
                this.EffortId = item.EffortItem != null ? item.EffortItem.Id : 0;
                this.Id = item.Id;
                this.Name = item.With(x => x.EffortItem).With(x => x.Name);
                this.Hours = item.Hours;
                this.Operation = item.Operations;
                this.EffortType = item.ChosenEffortType;
            }
        }

        public long Id { get; set; }

        public long EffortId { get; set; }

        public string Name { get; set; }

        public double Hours { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Operations Operation { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ChosenEffortType EffortType { get; set; }
    }
}