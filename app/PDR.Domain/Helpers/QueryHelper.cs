using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Helpers
{
    public class QueryHelper
    {
        public static IQueryable<RepairOrder> TeamRepairOrdersByManager(Manager manager,  Team team = null, bool onlyOwn = false)
        {
            IEnumerable<TeamEmployee> employees = team != null ? team.Employees : manager.Teams.SelectMany(x => x.Employees);

            var percents = employees.SelectMany(x => x.TeamEmployeePercents);

            if (onlyOwn)
            {
                percents = percents.Where(x => x.TeamEmployee == manager);
            }

            IEnumerable<RepairOrder> repairOrdersByPercents = percents.Select(x => x.RepairOrder);

            var own = employees.SelectMany(x => x.RepairOrders);
            var ro = repairOrdersByPercents.Union(own).AsQueryable();

            return ro;
        }

        public static IQueryable<RepairOrder> RepairOrdersByTechnician(Technician technician)
        {
            var roByPercents = technician.TeamEmployeePercents.Select(x => x.RepairOrder);
            var own = technician.RepairOrders;
            var ro = roByPercents.Union(own).AsQueryable();
            return ro;
        }

        public static IQueryable<Invoice> TeamInvoicesByManger(Manager manager)
        {
            var invoiceRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Invoice>>();
            var roIds = TeamRepairOrdersByManager(manager).Select(x => x.Id).ToList();
            var invoices = invoiceRepository.Where(x => roIds.Contains(x.Id));
            var own = manager.Invoices.ToList();
            return own.Union(invoices.ToList()).AsQueryable();
        }

        public static IQueryable<Invoice> InvoicesByTechnician(Technician technician)
        {
            var invoiceRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Invoice>>();
            var roIds = RepairOrdersByTechnician(technician).Select(x => x.Id).ToList();
            var invoices = invoiceRepository.Where(x => roIds.Contains(x.Id)).ToList();
            var own = technician.Invoices.ToList();
            var allInvoices = invoices.Union(own);
            return allInvoices.AsQueryable();
        }
    }
}
