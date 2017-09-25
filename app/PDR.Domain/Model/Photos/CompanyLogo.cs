namespace PDR.Domain.Model.Photos
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class CompanyLogo : Photo
    {
        protected CompanyLogo()
        {
        }

        public CompanyLogo(string fullsizePath, string thumbailPath, string contentType, Admin newAdmin, bool isNewEntity = false)
                    : base(fullsizePath, thumbailPath, contentType)
        {
            if (isNewEntity)
            {
                var company = newAdmin.Company;
                this.Company = company;
            }
        }
    }
}
