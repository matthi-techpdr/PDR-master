using System.Collections.Generic;
using System.Linq;
using PDR.Domain.Model;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiTeamModel : BaseIPhoneModel
    {
        public ApiTeamModel()
        {
        }

        public ApiTeamModel(Team team)
        {
            this.Id = team.Id;
            this.Title = team.Title;
            this.Comment = team.Comment;
            this.CreationDate = team.CreationDate.ToShortDateString();
        }

        public string Title { get; set; }

        public string Comment { get; set; }

        public string CreationDate { get; set; }

        public string Status { get; set; }

    }
}