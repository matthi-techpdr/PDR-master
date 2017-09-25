using System;

using PDR.Domain.Model.Customers;

namespace PDR.Domain.Model.Base
{
    public interface IReportable
    {
        DateTime CreationDate { get; set; }

        Customer Customer { get; set; }

        double TotalAmount { get; }
    }
}