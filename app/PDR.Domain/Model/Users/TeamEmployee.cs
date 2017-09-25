using System.Collections.Generic;
using System.Linq;

using Iesi.Collections.Generic;

namespace PDR.Domain.Model.Users
{
    public abstract class TeamEmployee : Employee
    {
        protected TeamEmployee()
        {
            this.Teams = new HashedSet<Team>();
            this.RepairOrders = new HashedSet<RepairOrder>();
            this.Invoices = new HashedSet<Invoice>();
        }

        public virtual int Commission { get; set; }

        public virtual Iesi.Collections.Generic.ISet<Team> Teams { get; set; }

        public virtual Iesi.Collections.Generic.ISet<RepairOrder> RepairOrders { get; set; }

        public virtual Iesi.Collections.Generic.ISet<Invoice> Invoices { get; set; }

        public virtual Iesi.Collections.Generic.ISet<TeamEmployeePercent> TeamEmployeePercents { get; set; }

        public virtual void AssignTeam(Team team)
        {
            this.Teams.Add(team);
            team.Employees.Add(this);
        }

        public virtual void UnAssignTeam(Team team)
        {
            this.Teams.Remove(team);
            team.Employees.Remove(this);
        }

        public virtual void AddRepairOrder(RepairOrder order)
        {
            order.TeamEmployee = this;
            this.RepairOrders.Add(order);
        }

        public virtual void AddInvoice(Invoice invoice)
        {
            invoice.TeamEmployee = this;
            this.Invoices.Add(invoice);
        }

        public virtual IList<RepairOrder> AssignedAndPersonalRepairOrders
        {
            get
            {
                var personal = this.RepairOrders.ToList();
                var assigned = this.TeamEmployeePercents.Select(x => x.RepairOrder).ToList();
                personal.AddRange(assigned);
                return personal.Distinct().ToList();
            }
        }
    }
}
