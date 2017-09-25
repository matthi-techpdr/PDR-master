using System.Linq;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Accountant.Models.Team
{
    public class TeamJsonModel : IJsonModel
    {
        public TeamJsonModel()
        {
        }

        public TeamJsonModel(Domain.Model.Team team)
        {
            var repairOrderRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<RepairOrder>>();
            var invoiceRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Domain.Model.Invoice>>();

            this.Id = team.Id.ToString();
            this.CreationDate = team.CreationDate.ToString("MM/dd/yyyy");
            this.Title = team.Title;
            this.TechniciansAmount = team.Employees.Count(t => t.Role == UserRoles.Technician || t.Role == UserRoles.RITechnician);
            this.ManagersAmount = team.Employees.Count(t => t.Role == UserRoles.Manager);
            this.Status = team.Status.ToString();
            this.OpenEstimatesAmount = team.Employees.Sum(e => e.Estimates.Count(es => es.EstimateStatus == EstimateStatus.Open && es.Customer.Teams.Contains(team)));
            this.OpenWorkOrdersAmount = repairOrderRepository.Count(
                            ro =>
                                ro.RepairOrderStatus == RepairOrderStatuses.Open && ro.Customer.Teams.Contains(team)
                               && ro.TeamEmployeePercents.All(x => x.TeamEmployee is Domain.Model.Users.Admin || x.TeamEmployee.Teams.Contains(team)));
            this.NonPaidInvoicesAmount = invoiceRepository.Count(
                            inv =>
                               inv.IsDiscard == false && inv.Status == InvoiceStatus.Unpaid && inv.Customer.Teams.Contains(team) &&
                                inv.RepairOrder.TeamEmployeePercents.All(
                                    x =>
                                        x.TeamEmployee is Domain.Model.Users.Admin ||
                                        x.TeamEmployee.Teams.Contains(team)));
        }

        public string Id { get; set; }

        public string CreationDate { get; set; }

        public string Title { get; set; }

        public int TechniciansAmount { get; set; }

        public int ManagersAmount { get; set; }

        public string Status { get; set; }

        public int OpenEstimatesAmount { get; set; }

        public int OpenWorkOrdersAmount { get; set; }

        public int NonPaidInvoicesAmount { get; set; }
    }
}