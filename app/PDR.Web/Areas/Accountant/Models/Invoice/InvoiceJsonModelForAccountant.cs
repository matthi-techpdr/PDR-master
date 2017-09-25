using System.Collections.Generic;
using System.Linq;
using PDR.Domain.Model.Enums;
using PDR.Web.Areas.Common.Models;

namespace PDR.Web.Areas.Accountant.Models.Invoice
{
    public class InvoiceJsonModelForAccountant : InvoiceJsonModelBase
    {
        private readonly IList<Domain.Model.Team> team;

        private readonly IList<Domain.Model.Team> teams;

        private readonly IEnumerable<Domain.Model.Team> intersectteams;

        public InvoiceJsonModelForAccountant(Domain.Model.Invoice invoice) : base(invoice)
        {
            foreach (var teamEmp in invoice.RepairOrder.TeamEmployeePercents)
            {
                var isRiTechnician = teamEmp.TeamEmployee.Role == UserRoles.RITechnician;
                
                this.Technicians += string.Format("{0} ({1});\n",
                    teamEmp.TeamEmployee.Name,
                    isRiTechnician
                    ? teamEmp.RiPart + "$"
                    : teamEmp.EmployeePart + "%");
            }

            this.PaidDate = invoice.PaidDate.ToString("MM/dd/yyyy") != "01/01/1753"
                                ? invoice.PaidDate.ToString("MM/dd/yyyy")
                                : string.Empty;

            this.team = invoice.RepairOrder.TeamEmployeePercents.SelectMany(x => x.TeamEmployee.Teams).Distinct().ToList();
            this.teams = invoice.Customer.Teams.ToList();
            this.intersectteams = this.team.Count > 0 ? this.teams.Count > 0 ? this.teams.Intersect(this.team).ToList() : this.team : new List<Domain.Model.Team>();

            this.Team = this.intersectteams.Any() ? string.Join(",\n", this.intersectteams.Select(x => x.Title)) : string.Empty;
        }

        public InvoiceJsonModelForAccountant(Domain.Model.Invoice invoice, long? teamId = null) : this(invoice)
        {
            this.team = invoice.RepairOrder.TeamEmployeePercents.SelectMany(x => x.TeamEmployee.Teams).Distinct().Where(x => x.Id == teamId.Value).ToList();
            this.teams = invoice.Customer.Teams.Where(x => x.Id == teamId.Value).ToList();
            this.intersectteams = this.team.Count > 0 ? this.teams.Count > 0 ? this.teams.Intersect(this.team).ToList() : this.team : new List<Domain.Model.Team>();
            this.Team = this.intersectteams.Any() ? this.intersectteams.SingleOrDefault(x => x.Id == teamId.Value).Title : string.Empty;
        }

        public string PaidDate { get; set; }

        public string Technicians { get; set; }

        public string Team { get; set; }
    }
}