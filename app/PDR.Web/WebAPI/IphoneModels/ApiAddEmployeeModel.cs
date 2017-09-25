using System.Collections.Generic;
using Newtonsoft.Json;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiAddEmployeeModel
    {
        public long RoId { get; set; }

        public IEnumerable<ApiAssignedEmployee> AssignedEmployees { get; set; }

        public bool? IsFlatFee { get; set; }

        public double? PaymentFlatFee { get; set; }
    }
}