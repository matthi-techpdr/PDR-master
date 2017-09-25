using System.Collections.Generic;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiDefinePercentsModel
    {
        public long RoId { get; set; }

        public IList<ApiAssignedEmployee> Employees { get; set; }
    }
}