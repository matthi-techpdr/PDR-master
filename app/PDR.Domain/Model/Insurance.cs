using System;
using Newtonsoft.Json;
using PDR.Domain.Model.Base;

namespace PDR.Domain.Model
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class Insurance : CompanyEntity
	{
		public virtual string InsuredName { get; set; }

		public virtual string CompanyName { get; set; }

		public virtual string Policy { get; set; }

		public virtual string Claim { get; set; }

		public virtual DateTime? ClaimDate { get; set; }

		public virtual DateTime? AccidentDate { get; set; }

		public virtual string Phone { get; set; }

		public virtual string ContactName { get; set; }

        [JsonIgnore]
        public virtual Estimate Estimate { get; set; }

        public Insurance()
        {
            
        }

        public Insurance(bool isNewEntity = false, Employee employee = null)
        {
            if (isNewEntity)
            {
                var user = employee ?? ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }
	}
}
