using System.Linq;
using PDR.Domain.Model;
using PDR.Domain.Model.Users;
using SmartArch.Data.Specification;

namespace PDR.Domain.Specifications
{
    public class TeamsRepairOrdersByManager : ISpecification<RepairOrder>
    {
        private readonly Manager manager;

        public TeamsRepairOrdersByManager(Manager manager)
        {
            this.manager = manager;
        }

        public IQueryable<RepairOrder> SatisfiedBy(IQueryable<RepairOrder> candidates)
        {

            return null;
        }
    }
}
