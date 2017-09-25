
using PDR.Domain.Model.Customers;

namespace PDR.Domain.Model
{
    using System;

    using Iesi.Collections.Generic;

    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Base;
    using PDR.Domain.Model.Enums;
    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Team : CompanyEntity, IEntityWithStatus
    {
        public virtual string Title { get; set; }

        public virtual string Comment { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual Statuses Status { get; set; }

        public virtual ISet<TeamEmployee> Employees { get; set; }

        public virtual ISet<Customer> Customers { get; set; }

        public Team(bool isNewEntity = false)
        {
            this.Employees = new HashedSet<TeamEmployee>();
            this.CreationDate = DateTime.Now;
            this.Customers = new HashedSet<Customer>();
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public Team(){}
    }
}
