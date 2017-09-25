using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Effort;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Photos;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Model
{
    using PDR.Domain.Model.Customers;

    public class Company : Entity, IEntityWithStatus
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public virtual string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the second address.
        /// </summary>
        public virtual string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public virtual StatesOfUSA State { get; set; }

        /// <summary>
        /// Gets or sets the zip.
        /// </summary>
        public virtual string Zip { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public virtual string PhoneNumber { get; set; }

        // <summary>
        // Gets or sets the URL.
        // </summary>
        public virtual string Url { get; set; }

        /// <summary>
        /// Gets or sets the employees.
        /// </summary>
        public virtual Iesi.Collections.Generic.ISet<Employee> Employees { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Gets or sets the mobile licenses number.
        /// </summary>
        public virtual int MobileLicensesNumber { get; set; }

        /// <summary>
        /// Gets or sets the active users number.
        /// </summary>
        public virtual int ActiveUsersNumber { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public virtual DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public virtual Statuses Status { get; set; }

        public virtual string City { get; set; }

        public virtual int DefaultHourlyRate { get; set; }

        public virtual int LimitForBodyPartEstimate { get; set; }

        public virtual CompanyLogo Logo { get; protected set; }

        public virtual DefaultMatrix DefaultMatrix { get; protected set; }

        public virtual CarModel DefaultVehicle { get; protected set; }

        public virtual string EstimatesEmailSubject { get; set; }

        public virtual string RepairOrdersEmailSubject { get; set; }

        public virtual string InvoicesEmailSubject { get; set; }

        public virtual string Notes { get; set; }

        public virtual Iesi.Collections.Generic.ISet<Affiliate> Affiliates { get; set; }

        public Company()
        {
            this.Employees = new HashedSet<Employee>();
            this.CreationDate = DateTime.Now;
            this.Affiliates = new HashedSet<Affiliate>();
        }

        public Company(bool isNewEntity = false)
        {
            this.Employees = new HashedSet<Employee>();
            this.CreationDate = DateTime.Now;
            this.Affiliates = new HashedSet<Affiliate>();
        }

        public virtual void AddEmployee(Employee admin)
        {
            this.Employees.Add(admin);
            admin.Company = this;
        }

        public virtual void AddLogo(CompanyLogo logo)
        {
            this.Logo = logo;
            logo.Company = this;
        }

        public virtual void SetDefaultMatrix(DefaultMatrix defaultMatrix)
        {
            this.DefaultMatrix = defaultMatrix;
            defaultMatrix.Company = this;
        }

        public virtual void SetDefaultVehicle(CarModel model)
        {
            this.DefaultVehicle = model;
            model.Company = this;
        }
    }
}
