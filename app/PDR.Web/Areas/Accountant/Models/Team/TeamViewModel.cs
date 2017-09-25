using System.Collections.Generic;
using System.Web.Mvc;

using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Accountant.Models.Team
{
    public class TeamViewModel : IViewModel
    {
        public TeamViewModel()
        {
        }

        public TeamViewModel(PDR.Domain.Model.Team team)
        {
            this.Id = team.Id.ToString();
            this.Title = team.Title;
            this.Comments = team.Comment;
        }

        public string Id { get; set; }

        public string Title { get; set; }

        public string Comments { get; set; }
        
        public IEnumerable<SelectListItem> Employees { get; set; }

        public IEnumerable<long> EmployeesList { get; set; }

        public int OpenEstimates { get; set; }

        public int OpenRO { get; set; }

        public int OpenInvoices { get; set; }
    }
}