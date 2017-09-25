using Iesi.Collections.Generic;

using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;

namespace PDR.Domain.Model.Customers
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class WholesaleCustomer : Customer, IEntityWithStatus
	{
        public WholesaleCustomer()
        {
            this.Status = Statuses.Active;
            this.Matrices = new HashedSet<PriceMatrix>();
        }

        public WholesaleCustomer(bool isNewEntity = false)
        {
            this.Status = Statuses.Active;
            this.Matrices = new HashedSet<PriceMatrix>();
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
                return CustomerType.Wholesale;
            }
        }

        public virtual string Name { get; set; }

		public virtual int Discount { get; set; }

		public virtual double HourlyRate { get; set; }

		public virtual double LaborRate { get; set; }

		public virtual double PartRate { get; set; }

		public virtual bool Insurance { get; set; }

		public virtual bool EstimateSignature { get; set; }

		public virtual bool OrderSignature { get; set; }

		public virtual bool WorkByThemselve { get; set; }

        public virtual string Phone2 { get; set; }

		public virtual string Password { get; set; }		

		public virtual string ContactName { get; set; }

		public virtual string ContactTitle { get; set; }

		public virtual string Comment { get; set; }

		public virtual Statuses Status { get; set; }

        public virtual ISet<PriceMatrix> Matrices { get; set; }

        public virtual void AssignTeam(Team team)
        {
            this.Teams.Add(team);
            team.Customers.Add(this);
        }

        public virtual void AddMatrix(PriceMatrix matrix)
        {
            this.Matrices.Add(matrix);
            matrix.Customers.Add(this);
        }
	}
}
