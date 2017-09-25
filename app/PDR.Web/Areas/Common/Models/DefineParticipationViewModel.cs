using System.Collections.Generic;
using PDR.Domain.Model;

namespace PDR.Web.Areas.Common.Models
{
    public class DefineParticipationViewModel
    {
        public long RepairOrderId { get; set; }

        public IList<TeamEmployeePercent> TeamEmployeePercents { get; set; }
    }
}