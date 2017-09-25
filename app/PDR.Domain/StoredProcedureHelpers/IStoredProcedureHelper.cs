using System.Collections.Generic;
using PDR.Domain.Model.Customers;

namespace PDR.Domain.StoredProcedureHelpers
{
    public interface IStoredProcedureHelper
    {
        IList<Customer> GetCustomersForFilter();
    }
}
