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
    public class RepairOrdersStoredProcedureHelper : IStoredProcedureHelper
    {
        private readonly ISession session;

        public int TotalCountRows { get; set; }

        public int NewActivityAmount { get; set; }

        private string CustomersIds { get; set; }

        private string RepairOrderIds { get; set; }

        public IList<RepairOrder> RepairOrders
        {
            get
            {
                var repairOrders = new List<RepairOrder>();
                if (!String.IsNullOrEmpty(RepairOrderIds))
                {
                    var repairOrdersRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<RepairOrder>>();
                    var ids = RepairOrderIds.Split(',');
                    foreach (var id in ids)
                    {
                        long curentId = 0;
                        var result = long.TryParse(id, out curentId);
                        if (result)
                        {
                            var invoice = repairOrdersRepository.Get(curentId);
                            repairOrders.Add(invoice);
                        }
                    }
                }
                return repairOrders;
            }
            protected set { }
        }

        public IList<RepairOrder> RepairOrdersFirstPage
        {
            get
            {
                GetFirstPage();
                return this.RepairOrders;
            }
            protected set { }
        }

        private const string RepairOrdersByUser = "dbo.GetRepairOrdersByUser";

        private IList<SqlParameter> Parameters { get; set; }

        public RepairOrdersStoredProcedureHelper(long userId, long? filterByTeam = null, long? filterByCustomers = null, bool? isOnlyOwn = null, int? rowsPerPage = null,
                                                int? pageNumber = null, string sortByColumn = null, string sortType = null, bool? isFinalised = null,
                                                bool? isInvoice = null, string vinCode = null, string stock = null, string custRo = null, bool? isForReport = null,
                                                RepairOrderStatuses? repairOrdersStatus = null, DateTime? dateFrom = null, DateTime? dateTo = null,
                                                long? affiliateId = null, bool? isGetAllRo = null, bool? isForCustomerFilter = null)
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
             if (isFinalised.HasValue)
             {
                 Parameters.Add(new SqlParameter("@IsFinalised", SqlDbType.Bit) { Value = isFinalised.Value });
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
             if (repairOrdersStatus.HasValue)
             {
                 Parameters.Add(new SqlParameter("@RepairOrdersStatus", SqlDbType.SmallInt) { Value = (int)repairOrdersStatus.Value });
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

             if (isInvoice.HasValue)
             {
                 Parameters.Add(new SqlParameter("@IsInvoice", SqlDbType.Bit) { Value = isInvoice.Value });
             }

             if (isGetAllRo.HasValue)
             {
                 Parameters.Add(new SqlParameter("@IsGetAllRo", SqlDbType.Bit) { Value = isGetAllRo.Value });
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
                    command.CommandText = RepairOrdersByUser;
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

                    var repairOrdersIds = new SqlParameter("@repairOrdersIds", SqlDbType.NVarChar, -1);
                    repairOrdersIds.Direction = ParameterDirection.Output;
                    command.Parameters.Add(repairOrdersIds);

                    var totalAmount = new SqlParameter("@totalAmount", SqlDbType.Decimal);
                    totalAmount.Direction = ParameterDirection.Output;
                    command.Parameters.Add(totalAmount);

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

                    RepairOrderIds = repairOrdersIds.Value.ToString();

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
