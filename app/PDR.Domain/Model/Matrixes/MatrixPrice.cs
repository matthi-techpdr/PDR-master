namespace PDR.Domain.Model.Matrixes
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class MatrixPrice : Price
	{
		public virtual Matrix Matrix { get; set; }

	    public MatrixPrice()
	    {
	        
	    }

        public MatrixPrice(bool isNewEntity = false, Admin newAdmin = null)
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