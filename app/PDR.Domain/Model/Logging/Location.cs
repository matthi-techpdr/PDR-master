using System;
using System.Web.Script.Serialization;

using PDR.Domain.Model.Base;

namespace PDR.Domain.Model.Logging
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class Location : CompanyEntity
    {
        public Location(bool isNewEntity = false)
        {
            this.Date = DateTime.Now;
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public Location(){}

        public virtual DateTime Date { get; protected set; }

        [ScriptIgnore]
        public virtual License License { get; set; }

        public virtual double Lat { get; set; }

        public virtual double Lng { get; set; }
    }
}