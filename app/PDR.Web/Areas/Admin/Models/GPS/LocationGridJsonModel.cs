using System.Linq;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using SmartArch.Data.Proxy;

namespace PDR.Web.Areas.Admin.Models.GPS
{
    public class LocationGridJsonModel : IJsonModel
    {
        public LocationGridJsonModel(License license)
        {
            this.Id = license.Id;
            if (license.Locations.Count() != 0)
            {
                this.LastReportDate = license.Locations.Select(x => x.Date).Max().ToString("MM/dd/yyyy HH:mm");
            }

            var employee = license.Employee.ToPersist<Employee>();

            this.OwnerName = employee.With(x => x.Name);
            var teamEmployee = employee as TeamEmployee;
            if (teamEmployee != null)
            {
                this.Teams = string.Join(",", teamEmployee.Teams.Select(x => x.Title).ToList());
            }

            this.Role = license.Employee.Role.ToString();
        }

        public long Id { get; set; }

        public string LastReportDate { get; set; }

        public string OwnerName { get; set; }

        public string Teams { get; set; }

        public string Role { get; set; }
    }
}
