using System.Collections.Generic;
using System.Web.Mvc;

using PDR.Domain.Model.Users;

namespace PDR.Web.Areas.Common.Models
{
    public class AssignTechniciansViewModel
    {
        public IList<SelectListItem> Technicians { get; set; }

        public IList<TeamEmployee> TeamEmployees { get; set; }

        public long? RepairOrderId { get; set; }

        public long[] TechnicianIds { get; set; }

        public RiTechnicianModel RiTechnicianModel { get; set; }

        public AssignTechniciansViewModel()
        {
            Technicians = new List<SelectListItem>();
            TeamEmployees = new List<TeamEmployee>();
            RiTechnicianModel = new RiTechnicianModel();
        }
    }
}