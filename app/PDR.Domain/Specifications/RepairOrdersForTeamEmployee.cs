using System;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Model;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;
using SmartArch.Data.Specification;

namespace PDR.Domain.Specifications
{
    internal class RepairOrderForTeamEmployee : ISpecification<RepairOrder>
    {
        private readonly TeamEmployee currentEmployee;

        public RepairOrderForTeamEmployee()
        {
            this.currentEmployee = ServiceLocator.Current.GetInstance<ICurrentWebStorage<TeamEmployee>>().Get();
            if (this.currentEmployee == null)
            {
                throw new ArgumentException("There is not team employee.");
            }
        }

        public IQueryable<RepairOrder> SatisfiedBy(IQueryable<RepairOrder> candidates)
        {
            var repairOrders = candidates
                .SelectMany(x => x.TeamEmployeePercents)
                .Where(x => x.TeamEmployee == this.currentEmployee)
                .Select(x => x.RepairOrder);
            return repairOrders;
        }
    }
}
