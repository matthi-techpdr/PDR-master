using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PDR.Domain.Services.Grid
{
    using System.Web.Mvc;

    [DataContract]
    public class GridModel<TJsonModel>
    {
        [DataMember(Name = "total")]
        public double total { get; set; }

        [DataMember(Name = "page")]
        public int page { get; set; }

        [DataMember(Name = "records")]
        public int records { get; set; }

        [DataMember(Name = "rows")]
        public IList<TJsonModel> rows { get; set; }

        [DataMember(Name = "customersFilter")]
        public IList<SelectListItem> customersFilter { get; set; }

        [DataMember(Name = "active")]
        public int active { get; set; }
    }
}