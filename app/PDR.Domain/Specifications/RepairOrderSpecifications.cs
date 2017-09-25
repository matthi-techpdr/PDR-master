using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using SmartArch.Data.Specification;

namespace PDR.Domain.Specifications
{
    public class RepairOrderSpecifications 
    {
        public static ISpecification<RepairOrder> ForTeamEmployee()
        {
            return new RepairOrderForTeamEmployee();
        }

        public static ISpecification<RepairOrder> Finalised()
        {
            return new QuerySpecification<RepairOrder>(x => x.RepairOrderStatus == RepairOrderStatuses.Finalised);
        }

        public static ISpecification<RepairOrder> ByCustomer(Customer customer)
        {
            return new QuerySpecification<RepairOrder>(x => x.Customer == customer);
        }

        public static ISpecification<RepairOrder> NotInvoice()
        {
            return new QuerySpecification<RepairOrder>(x => !x.IsInvoice);
        }

        public static ISpecification<RepairOrder> ByTeam(Team team)
        {
            return new QuerySpecification<RepairOrder>(x => x.TeamEmployee.Teams.Contains(team));
        }
    }
}
