using Newtonsoft.Json;

namespace PDR.Domain.Model.CustomLines
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class CustomCarInspectionLine : CustomLine
	{
        [JsonIgnore]
		public virtual CarInspection CarInspection { get; set; }

        public CustomCarInspectionLine()
        {
            
        }

        public CustomCarInspectionLine(bool isNewEntity = false)
	    {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }
	}
}
