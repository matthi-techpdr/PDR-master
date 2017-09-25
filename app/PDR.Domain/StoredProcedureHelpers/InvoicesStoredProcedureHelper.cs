using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using System;
using System.Collections.Generic;
using PDR.Domain.Model.Enums;
using System.Linq;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using SmartArch.Data;


namespace PDR.Domain.StoredProcedureHelpers
{
    public class InvoicesStoredProcedureHelper : IStoredProcedureHelper
    {
        private readonly ISession session;

        public int TotalCountRows { get; set; }

        public int NewActivityAmount { get; set; }

        public double UnpaidInvoicesSum { get; set; }

        public double PaidInvoicesSum { get; set; }

        private string CustomersIds { get; set; }

        private string InvoiceIds { get; set; }

        public IList<Invoice> Invoices
        {
            get
            {
                var invoices = new List<Invoice>();
                if (!String.IsNullOrEmpty(InvoiceIds))
                {
                    var invoicesRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Invoice>>();
                    var ids = InvoiceIds.Split(',');
                    foreach (var id in ids)
                    {
                        long curentId = 0;
                        var result = long.TryParse(id, out curentId);
                        if (result)
                        {
                            var invoice = invoicesRepository.Get(curentId);
                            invoices.Add(invoice);
                        }
                    }
                }
                return invoices;
            }
            protected set { }
        }

        public IList<Invoice> InvoicesFirstPage
        {
            get
            {
                GetFirstPage();
                return this.Invoices;
            }
            protected set { }
        }

        private const string InvoicesByUser = "dbo.GetInvoicesByUser";

        private IList<SqlParameter> Parameters { get; set; }

        public InvoicesStoredProcedureHelper(long userId, long? filterByTeam = null, long? filterByCustomers = null, bool? isOnlyOwn = null, int? rowsPerPage = null,
                                                int? pageNumber = null, string sortByColumn = null, string sortType = null, bool? isArchive = null,
                                                string vinCode = null, string stock = null, string custRo = null, InvoiceStatus? invoicesStatus = null,
                                                DateTime? dateFrom = null, DateTime? dateTo = null, bool? getPaidUnpaidInvoicesSum = null, long? affiliateId = null,
                                                bool? isGetAllInvoices = null, bool? isDiscarded = null, bool? isForCustomerFilter = null)
            {
             this.session = ServiceLocator.Current.GetInstance<ISession>();
             #region Set input parameters
             Parameters = new List<SqlParameter>();
             Parameters.Add(new SqlParameter("@userId", SqlDbType.BigInt) { Value = userId });

             if (filterByTeam.HasValue && filterByTeam.Value != 0)
             {
                 Parameters.Add(new SqlParameter("@FilterByTeam", SqlDbType.BigInt) { Value = filterByTeam.Value });
             }
             if (filterByCustomers.HasValue)
             {
                 Parameters.Add(new SqlParameter("@FilterByCustomers", SqlDbType.BigInt) { Value = filterByCustomers.Value });
             }
             if (isOnlyOwn.HasValue)
             {
                 Parameters.Add(new SqlParameter("@IsOnlyOwn", SqlDbType.Bit) { Value = isOnlyOwn.Value });
             }
             if (rowsPerPage.HasValue)
             {
                 Parameters.Add(new SqlParameter("@RowsPerPage", SqlDbType.Int) { Value = rowsPerPage.Value });
             }
             if (pageNumber.HasValue)
             {
                 Parameters.Add(new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber.Value });
             }
             if (!String.IsNullOrEmpty(sortByColumn))
             {
                 if (sortByColumn == "CarInfo") sortByColumn = "CarsMakeModelYear";
                 if (sortByColumn == "TotalAmount" || sortByColumn == "InvoiceAmount") sortByColumn = "InvoiceSum";
                 if (sortByColumn == "InvoiceStatus") sortByColumn = "Status";
                 Parameters.Add(new SqlParameter("@SortByColumn", SqlDbType.NVarChar, 50) { Value = sortByColumn });
             }
             if (!String.IsNullOrEmpty(sortType) && ((sortType.ToUpper() == "DESC") || (sortType.ToUpper() == "ASC")))
             {
                 Parameters.Add(new SqlParameter("@SortType", SqlDbType.NVarChar, 10) { Value = sortType });
             }
             if (isArchive.HasValue)
             {
                 Parameters.Add(new SqlParameter("@IsArchive", SqlDbType.Bit) { Value = isArchive.Value });
             }
             if (!String.IsNullOrEmpty(vinCode))
             {
                 Parameters.Add(new SqlParameter("@VinCode", SqlDbType.NVarChar, 50) { Value = vinCode });
             }
             if (!String.IsNullOrEmpty(stock))
             {
                 Parameters.Add(new SqlParameter("@Stock", SqlDbType.NVarChar, 50) { Value = stock });
             }
             if (!String.IsNullOrEmpty(custRo))
             {
                 Parameters.Add(new SqlParameter("@CustRo", SqlDbType.NVarChar, 50) { Value = custRo });
             }
             if (invoicesStatus.HasValue)
             {
                 Parameters.Add(new SqlParameter("@InvoicesStatus", SqlDbType.Int) { Value = (int)invoicesStatus.Value});
             }
             if (dateFrom.HasValue)
             {
                 Parameters.Add(new SqlParameter("@DateFrom", SqlDbType.DateTime) { Value = dateFrom.Value });
             }
             if (dateTo.HasValue)
             {
                 Parameters.Add(new SqlParameter("@DateTo", SqlDbType.DateTime) { Value = dateTo.Value });
             }

             if (getPaidUnpaidInvoicesSum.HasValue)
             {
                 Parameters.Add(new SqlParameter("@GetPaidUnpaidInvoicesSum", SqlDbType.Bit) { Value = getPaidUnpaidInvoicesSum.Value});
             }

             if (affiliateId.HasValue)
             {
                 Parameters.Add(new SqlParameter("@AffiliateId", SqlDbType.BigInt) { Value = affiliateId.Value });
             }

             if (isGetAllInvoices.HasValue)
             {
                 Parameters.Add(new SqlParameter("@IsGetAllInvoices", SqlDbType.Bit) { Value = isGetAllInvoices.Value });
             }

             if (isDiscarded.HasValue)
             {
                 Parameters.Add(new SqlParameter("@IsDiscarded", SqlDbType.Bit) { Value = isDiscarded.Value });
             }

             //if (isForCustomerFilter.HasValue)
             //{
             //    Parameters.Add(new SqlParameter("@IsForCustomerFilter", SqlDbType.Bit) { Value = isForCustomerFilter.Value });
             //}

             #endregion
             ExecutStoredProcedure();
        }

        public IList<Customer> GetCustomersForFilter()
        {
            var customers = new List<Customer>();
            if (!String.IsNullOrEmpty(CustomersIds))
            {
                var customersRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Customer>>();
                var ids = CustomersIds.Split(',');
                foreach (var id in ids)
                {
                    long curentId = 0;
                    var result = long.TryParse(id, out curentId);
                    if (result)
                    {
                        var customer = customersRepository.Get(curentId);
                        if (customer != null) customers.Add(customer);
                    }
                }
            }
            return customers;
        }

        private void ExecutStoredProcedure() 
        {
            using (ITransaction transaction = session.BeginTransaction())
            {
                using (IDbCommand command = new SqlCommand())
                {
                    command.Connection = session.Connection;
                    transaction.Enlist(command);

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = InvoicesByUser;
                    command.Parameters.Clear();
                    var parms = new SqlParameter[Parameters.Count];
                    for (int i = 0; i < Parameters.Count; i++)
                    {
                        var newParam = new SqlParameter()
                                           {
                                               Direction = Parameters[i].Direction,
                                               Offset = Parameters[i].Offset,
                                               ParameterName = Parameters[i].ParameterName,
                                               Size = Parameters[i].Size,
                                               SqlDbType = Parameters[i].SqlDbType,
                                               Value = Parameters[i].Value
                                           };
                        parms[i] = newParam;
                    }
                    foreach (var sqlParameter in parms)
                    {
                        command.Parameters.Add(sqlParameter);
                    }

                    #region Set output parameters

                    var totalCountRows = new SqlParameter("@totalCountRows", SqlDbType.Int);
                    totalCountRows.Direction = ParameterDirection.Output;
                    command.Parameters.Add(totalCountRows);

                    var customersForFilter = new SqlParameter("@customersForFilter", SqlDbType.NVarChar, -1);
                    customersForFilter.Direction = ParameterDirection.Output;
                    command.Parameters.Add(customersForFilter);

                    var newActivityAmount = new SqlParameter("@newActivityAmount", SqlDbType.Int);
                    newActivityAmount.Direction = ParameterDirection.Output;
                    command.Parameters.Add(newActivityAmount);

                    var invoiceIds = new SqlParameter("@invoiceIds", SqlDbType.NVarChar, -1);
                    invoiceIds.Direction = ParameterDirection.Output;
                    command.Parameters.Add(invoiceIds);

                    var paidInvSum = new SqlParameter("@paidInvSum", SqlDbType.Float);
                    paidInvSum.Direction = ParameterDirection.Output;
                    command.Parameters.Add(paidInvSum);

                    var unpaidInvSum = new SqlParameter("@unpaidInvSum", SqlDbType.Float);
                    unpaidInvSum.Direction = ParameterDirection.Output;
                    command.Parameters.Add(unpaidInvSum);

                    #endregion

                    // Execute the stored procedure
                    command.ExecuteNonQuery();

                    //Get results
                    int countRows;
                    bool result = Int32.TryParse(totalCountRows.Value.ToString(), out countRows);
                    if (result) TotalCountRows = countRows;

                    int newActivities;
                    result = Int32.TryParse(newActivityAmount.Value.ToString(), out newActivities);
                    if (result) NewActivityAmount = newActivities;

                    CustomersIds = customersForFilter.Value.ToString();

                    InvoiceIds = invoiceIds.Value.ToString();

                    double paidSum;
                    result = Double.TryParse(paidInvSum.Value.ToString(), out paidSum);
                    if (result) PaidInvoicesSum = paidSum;

                    double unpaidSum;
                    result = Double.TryParse(unpaidInvSum.Value.ToString(), out unpaidSum);
                    if (result) UnpaidInvoicesSum = unpaidSum;

                    transaction.Commit();
                }
            }
        }

        private void GetFirstPage()
        {
            var pageNumber = Parameters.FirstOrDefault(x => String.Equals(x.ParameterName, "@PageNumber"));
            if (pageNumber != null)
            {
                pageNumber.Value = 1;
            }
            else
            {
                Parameters.Add(new SqlParameter("@PageNumber", SqlDbType.Int) { Value = 1 });
            }
            ExecutStoredProcedure();
        }
    }
}
