using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;

namespace PDR.Domain.Helpers
{
    using SmartArch.Data.Proxy;

    public static class CustomerNameResolver
    {
        public static string GetCustomerName(this Customer customer)
        {
            if (customer != null)
            {
                var customerName = string.Empty;

                if (customer.IsPersist<RetailCustomer>())
                {
                    var retailCustomer = customer.ToPersist<RetailCustomer>();
                    customerName = string.Format("{0} {1}", retailCustomer.FirstName, retailCustomer.LastName);
                }
                else if (customer.IsPersist<WholesaleCustomer>())
                {
                    var wholeSaleCustomer = customer.ToPersist<WholesaleCustomer>();
                    customerName = wholeSaleCustomer.Name;
                }
                else if (customer.IsPersist<Affiliate>())
                {
                    var wholeSaleCustomer = customer.ToPersist<Affiliate>();
                    customerName = wholeSaleCustomer.Name;
                }

                return customerName;
            }
            
            return string.Empty;
        }
    }
}
