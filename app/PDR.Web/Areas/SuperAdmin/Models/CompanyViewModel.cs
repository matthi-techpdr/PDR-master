using System.Collections.Generic;
using System.Web.Mvc;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Web.Core.Helpers;

namespace PDR.Web.Areas.SuperAdmin.Models
{
    public class CompanyViewModel : IViewModel
    {
        public CompanyViewModel()
        {
            this.States = ListsHelper.GetStates(0);
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public int State { get; set; }

        public string Zip { get; set; }

        public string City { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Url { get; set; }

        public string AdminLogin { get; set; }        

        public string AdminName { get; set; }
        [AllowHtml]
        public string Comment { get; set; }

        public int? MobileLicensesNumber { get; set; }

        public int? ActiveUsersNumber { get; set; }

        public IList<SelectListItem> States { get; set; }
    }
}