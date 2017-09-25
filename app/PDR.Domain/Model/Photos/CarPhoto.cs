namespace PDR.Domain.Model.Photos
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class CarPhoto : Photo
	{
        protected CarPhoto()
        {
        }

        public CarPhoto(string fullsizePath, string thumbailPath, string contentType, bool isNewEntity = false)
                    : base(fullsizePath, thumbailPath, contentType)
        {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public CarPhoto(byte[] photo, string contentType, bool isNewEntity = false)
            : base(photo, contentType)
        {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public virtual Estimate Estimate { get; set; }
	}
}
