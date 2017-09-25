using System.Web.Script.Serialization;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;

namespace PDR.Domain.Model.Base
{
    public abstract class CompanyEntity : Entity, ICompanyEntity  
    {
        [ScriptIgnore]
        public virtual Company Company { get; set; }
    }
}
