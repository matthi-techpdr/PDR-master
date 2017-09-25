using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;

namespace PDR.Domain.Model.Customers
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class RetailCustomer : Customer
	{
        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public override CustomerType CustomerType
        {
            get
            {
                return CustomerType.Retail;
            }
        }

	    public virtual DefaultMatrix DefaultMatrix
	    {
	        get
	        {
	            return this.Company.DefaultMatrix;
	        }
	    }

        public RetailCustomer(){}

	    public RetailCustomer(bool isNewEntity = false)
	    {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }
    }
}
