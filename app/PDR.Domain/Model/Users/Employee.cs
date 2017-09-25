using System;
using Iesi.Collections.Generic;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Logging;

namespace PDR.Domain.Model.Users
{
    public abstract class Employee : User, ICompanyEntity, IEntityWithStatus
    {
        protected Employee()
        {
            this.HiringDate = DateTime.Now;
            this.Estimates = new HashedSet<Estimate>();
            this.Logs = new HashedSet<Log>();
            this.PreviousEstimates = new HashedSet<Estimate>();
            this.Licenses = new HashedSet<License>();
        }

        public virtual string Address { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string Email { get; set; }

        public virtual string TaxId { get; set; }

        public virtual string Comment { get; set; }

        public virtual bool CanQuickEstimate { get; set; }

        public virtual bool CanExtraQuickEstimate { get; set; }

        public virtual bool IsShowAllTeams { get; set; }

        public virtual bool? CanEditTeamMembers { get; set; }

        public virtual Statuses Status { get; set; }

        public virtual DateTime HiringDate { get; set; }

        public virtual ISet<Estimate> Estimates { get; set; }

        public virtual ISet<Log> Logs { get; set; }

        public virtual Company Company { get; set; }

        public virtual ISet<License> Licenses { get; set; }

        public virtual string City { get; set; }

        public virtual string Zip { get; set; }

        public virtual StatesOfUSA State { get; set; }

        public virtual ISet<Estimate> PreviousEstimates { get; set; }

        public virtual string SignatureName { get; set; }

        public virtual void AddEstimate(Estimate estimate)
        {
            estimate.Employee = this;
            this.Estimates.Add(estimate);
        }

        public virtual void AddLicense(License license)
        {
            license.Employee = this;
            this.Licenses.Add(license);
        }

        public virtual bool IsBasic { get; set; }
    }
}
