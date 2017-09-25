using System.Data;
using System.Data.SqlClient;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using System;
using System.Collections.Generic;
using PDR.Domain.Model.Enums;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using SmartArch.Data;
using System.Linq;

namespace PDR.Domain.StoredProcedureHelpers
{
    public class EstimatesStoredProcedureHelper : IStoredProcedureHelper
    {
        private readonly ISession session;

        public int TotalCountRows { get; set; }

        public int NewActivityAmount { get; set; }

        private string CustomersIds { get; set; }

        private string EstimateIds { get; set; }

        public double TotalAmountSum { get; set; }

        public IList<Estimate> Estimates
        {
            get
            {
                var estimates = new List<Estimate>();
                if (!String.IsNullOrEmpty(EstimateIds))
                {
                    var estimatesRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Estimate>>();
                    var ids = EstimateIds.Split(',');
                    foreach (var id in ids)
                    {
                        long curentId = 0;
                        var result = long.TryParse(id, out curentId);
                        if (result)
                        {
                            var invoice = estimatesRepository.Get(curentId);
                            estimates.Add(invoice);
                        }
                    }
                }
                return estimates;
            }
            protected set { }
        }

        public IList<Estimate> EstimatesFirstPage
        {
            get
            {
                GetFirstPage();
                return this.Estimates;
            }
            protected set { }
        }

        private const string EstimatesByUser = "dbo.GetEstimatesByUser";

        private IList<SqlParameter> Parameters { get; set; }

        public EstimatesStoredProcedureHelper(long userId, long? filterByTeam = null, long? filterByCustomers = null, bool? isOnlyOwn = null, int? rowsPerPage = null,
                                                int? pageNumber = null, string sortByColumn = null, string sortType = null, bool? isArchived = null,
                                                string vinCode = null, string stock = null, string custRo = null, bool? isForReport = null,
                                                EstimateStatus? estimatesStatus = null, DateTime? dateFrom = null, DateTime? dateTo = null,
                                                long? affiliateId = null, bool? isGetAllEstimates = null, bool? isGetTotalSum = null, bool? isForCustomerFilter = null)
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
                 Parameters.Add(new SqlParameter("@SortByColumn", SqlDbType.NVarChar, 50) { Value = sortByColumn });
             }
             if (!String.IsNullOrEmpty(sortType) && ((sortType.ToUpper() == "DESC") || (sortType.ToUpper() == "ASC")))
             {
                 Parameters.Add(new SqlParameter("@SortType", SqlDbType.NVarChar, 10) { Value = sortType });
             }
             if (isArchived.HasValue)
             {
                 Parameters.Add(new SqlParameter("@IsArchived", SqlDbType.Bit) { Value = isArchived.Value });
             }
             if (isForReport.HasValue)
             {
                 Parameters.Add(new SqlParameter("@IsForReport", SqlDbType.Bit) { Value = isForReport.Value });
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
             if (estimatesStatus.HasValue)
             {
                 Parameters.Add(new SqlParameter("@EstimatesStatus", SqlDbType.SmallInt) { Value = (int)estimatesStatus.Value });
             }
             if (dateFrom.HasValue)
             {
                 Parameters.Add(new SqlParameter("@DateFrom", SqlDbType.DateTime) { Value = dateFrom.Value });
             }
             if (dateTo.HasValue)
             {
                 Parameters.Add(new SqlParameter("@DateTo", SqlDbType.DateTime) { Value = dateTo.Value });
             }

             if (affiliateId.HasValue)
             {
                 Parameters.Add(new SqlParameter("@AffiliateId", SqlDbType.BigInt) { Value = affiliateId.Value });
             }

             if (isGetAllEstimates.HasValue)
             {
                 Parameters.Add(new SqlParameter("@IsGetAllEstimates", SqlDbType.Bit) { Value = isGetAllEstimates.Value });
             }

             if (isGetTotalSum.HasValue)
             {
                 Parameters.Add(new SqlParameter("@GetTotalAmountSum", SqlDbType.Bit) { Value = isGetTotalSum.Value });
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
                    command.CommandText = EstimatesByUser;
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

                    var estimatesIds = new SqlParameter("@EstimatesIds", SqlDbType.NVarChar, -1);
                    estimatesIds.Direction = ParameterDirection.Output;
                    command.Parameters.Add(estimatesIds);

                    var totalAmountSum = new SqlParameter("@EstimatesTotalAmountSum", SqlDbType.Float);
                    totalAmountSum.Direction = ParameterDirection.Output;
                    command.Parameters.Add(totalAmountSum);

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

                    EstimateIds = estimatesIds.Value.ToString();

                    double totalSum;
                    result = Double.TryParse(totalAmountSum.Value.ToString(), out totalSum);
                    if (result) TotalAmountSum = totalSum;

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
