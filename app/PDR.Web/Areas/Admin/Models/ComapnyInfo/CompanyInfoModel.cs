using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using PDR.Domain.Model;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Web.Core.Helpers;

namespace PDR.Web.Areas.Admin.Models.ComapnyInfo
{
    public class CompanyInfoModel : IViewModel
    {
        public CompanyInfoModel()
        {
            this.States = ListsHelper.GetStates(null);
        }

        public CompanyInfoModel(Company company)
        {
            this.Id = company.Id.ToString();
            this.Name = company.Name;
            this.Address1 = company.Address1;
            this.Address2 = company.Address2;
            this.City = company.City;
            this.Zip = company.Zip;
            this.PhoneNumber = company.PhoneNumber;
            this.Email = company.Email;

            this.DefaultHourlyRate = company.DefaultHourlyRate;
            this.LimitForBodyPartEstimate = company.LimitForBodyPartEstimate;
            this.EstimateEmailSubject = company.EstimatesEmailSubject;
            this.RepairOrderEmailSubject = company.RepairOrdersEmailSubject;
            this.InvoiceEmailSubject = company.InvoicesEmailSubject;
            this.States = ListsHelper.GetStates((int)company.State).ToList();
            this.State = (int)company.State;
            this.Notes = company.Notes;
        }

        public string Name { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string Zip { get; set; }

        public int State { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string EstimateEmailSubject { get; set; }

        public string RepairOrderEmailSubject { get; set; }

        public string InvoiceEmailSubject { get; set; }

        public int DefaultHourlyRate { get; set; }

        public int LimitForBodyPartEstimate { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }

        public string Id { get; set; }

        [AllowHtml]
        public string Notes { get; set; }
    }
}