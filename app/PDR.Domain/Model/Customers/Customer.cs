using System;

using Iesi.Collections.Generic;

using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;

using SmartArch.Core.Helpers;

namespace PDR.Domain.Model.Customers
{
    public abstract class Customer : CompanyEntity
	{
        protected Customer()
        {
            this.Estimates = new HashedSet<Estimate>();
            this.Teams = new HashedSet<Team>();
            this.CreatingDate = SystemTime.Now();
            this.Invoices = new HashedSet<Invoice>();
            this.RepairOrders = new HashedSet<RepairOrder>();
        }

		public virtual string Address1 { get; set; }

		public virtual string Address2 { get; set; }

		public virtual string Phone { get; set; }

		public virtual string Fax { get; set; }

		public virtual StatesOfUSA? State { get; set; }
        
        public virtual string Zip { get; set; }

        public virtual string City { get; set; }

		public virtual string Email { get; set; }

        public virtual string Email2 { get; set; }

        public virtual string Email3 { get; set; }

        public virtual string Email4 { get; set; }

		public virtual DateTime CreatingDate { get; set; }

		public abstract CustomerType CustomerType { get; }

		public virtual ISet<Estimate> Estimates { get; set; }

        public virtual ISet<Team> Teams { get; set; }

        public virtual ISet<Invoice> Invoices { get; set; }

        public virtual ISet<RepairOrder> RepairOrders { get; set; }
	}
}
