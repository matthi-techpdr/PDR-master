using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Base;

namespace PDR.Domain.Model.Customers
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class Affiliate : Customer, IEntityWithStatus
    {
        public Affiliate()
        {
            this.Status = Statuses.Active;
        }
        public Affiliate(bool isNewEntity = false)
        {
            this.Status = Statuses.Active;
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public override CustomerType CustomerType
        {
            get
            {
                return CustomerType.Affiliates;
            }
        }

        public virtual DefaultMatrix DefaultMatrix
        {
            get
            {
                return this.Company.DefaultMatrix;
            }
        }

        public virtual string Name { get; set; }

        public virtual int HourlyRate { get; set; }

        public virtual double LaborRate { get; set; }

        public virtual double PartRate { get; set; }

        public virtual string Phone2 { get; set; }

        public virtual string ContactName { get; set; }

        public virtual string ContactTitle { get; set; }

        public virtual string Comment { get; set; }

        public virtual Statuses Status { get; set; }

        public virtual void AssignTeam(Team team)
        {
            this.Teams.Add(team);
            team.Customers.Add(this);
        }
    }
}

