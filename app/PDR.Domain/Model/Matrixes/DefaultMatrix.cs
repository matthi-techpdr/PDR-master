namespace PDR.Domain.Model.Matrixes
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class DefaultMatrix : PriceMatrix
    {
        public DefaultMatrix() { }

        public DefaultMatrix(bool isNewEntity = false, Admin newAdmin = null)
        {
            if (isNewEntity)
            {
                if (newAdmin != null)
                {
                    var company = newAdmin.Company;
                    this.Company = company;
                }
                else
                {
                    var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                    this.Company = user.Company;
                }
            }
        }
    }
}
