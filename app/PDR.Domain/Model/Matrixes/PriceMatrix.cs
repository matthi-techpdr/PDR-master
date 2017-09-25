using Iesi.Collections.Generic;

using PDR.Domain.Model.Customers;

namespace PDR.Domain.Model.Matrixes
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    public class PriceMatrix : Matrix
    {
        public PriceMatrix()
        {
            this.Customers = new HashedSet<WholesaleCustomer>();
        }

        public PriceMatrix(bool isNewEntity = false)
        {
            this.Customers = new HashedSet<WholesaleCustomer>();
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public virtual ISet<WholesaleCustomer> Customers { get; set; }

        public virtual void AddCustomer(WholesaleCustomer customer)
        {
            this.Customers.Add(customer);
            customer.Matrices.Add(this);
        }
    }
}
