namespace PDR.Domain.Model.CustomLines
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class OversizedDentsLine : CustomLine
	{
		public virtual CarInspection CarInspection { get; set; }

        public OversizedDentsLine(){}

        public OversizedDentsLine(bool isNewEntity = false)
	    {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }
	}
}
