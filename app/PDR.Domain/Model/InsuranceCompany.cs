using PDR.Domain.Model.Base;

namespace PDR.Domain.Model
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class InsuranceCompany : CompanyEntity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public virtual string Name { get; set; }

        public InsuranceCompany(){}

        public InsuranceCompany(bool isNewEntity = false)
        {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }
    }
}
