using Microsoft.Practices.ServiceLocation;
using NHibernate;
using System;
using System.Collections.Generic;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using System.Linq;

namespace PDR.Domain.StoredProcedureHelpers
{
    using System.Data;
    using System.Data.SqlClient;

    public class CustomerStoredProcedureHelper : IStoredProcedureHelper
    {
        private readonly ISession session;

        private IList<SqlParameter> Parameters { get; set; }

        private const string GetOneCustomerModel = "dbo.GetOneCustomerModel";

        public int AmountOfOpenEstimates { get; set; }

        public double SumOfOpenEstimates { get; set; }

        public double SumOfOpenWorkOrders { get; set; }

        public double SumOfPaidInvoices { get; set; }

        public double SumOfUnpaidInvoices { get; set; }
        
        public CustomerStoredProcedureHelper(long userId, long customerId, string customerType, long? filterByTeam = null, bool? isOnlyOwn = null)
        {
             session = ServiceLocator.Current.GetInstance<ISession>();
             #region Set input parameters
             Parameters = new List<SqlParameter>();
             Parameters.Add(new SqlParameter("@userId", SqlDbType.BigInt) { Value = userId });
             Parameters.Add(new SqlParameter("@customerId", SqlDbType.BigInt) { Value = customerId });

             if (filterByTeam.HasValue && filterByTeam.Value != 0)
             {
                 Parameters.Add(new SqlParameter("@FilterByTeam", SqlDbType.BigInt) { Value = filterByTeam.Value });
             }
             if (isOnlyOwn.HasValue)
             {
                 Parameters.Add(new SqlParameter("@IsOnlyOwn", SqlDbType.Bit) { Value = isOnlyOwn.Value });
             }
             if (!String.IsNullOrEmpty(customerType) && ((customerType.ToLower() == "wholesalecustomer") || (customerType.ToLower() == "affiliate")))
             {
                 Parameters.Add(new SqlParameter("@CustomerType", SqlDbType.NVarChar, 50) { Value = customerType });
             }

             #endregion
             ExecutStoredProcedure();
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
                    command.CommandText = GetOneCustomerModel;
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

                    var amountOfOpenEstimates = new SqlParameter("@AmountOfOpenEstimates", SqlDbType.Int);
                    amountOfOpenEstimates.Direction = ParameterDirection.Output;
                    command.Parameters.Add(amountOfOpenEstimates);

                    var sumOfOpenEstimates = new SqlParameter("@SumOfOpenEstimates", SqlDbType.Float);
                    sumOfOpenEstimates.Direction = ParameterDirection.Output;
                    command.Parameters.Add(sumOfOpenEstimates);

                    var sumOfOpenWorkOrders = new SqlParameter("@SumOfOpenWorkOrders", SqlDbType.Float);
                    sumOfOpenWorkOrders.Direction = ParameterDirection.Output;
                    command.Parameters.Add(sumOfOpenWorkOrders);

                    var sumOfPaidInvoices = new SqlParameter("@SumOfPaidInvoices", SqlDbType.Float);
                    sumOfPaidInvoices.Direction = ParameterDirection.Output;
                    command.Parameters.Add(sumOfPaidInvoices);

                    var sumOfUnpaidInvoices = new SqlParameter("@SumOfUnpaidInvoices", SqlDbType.Float);
                    sumOfUnpaidInvoices.Direction = ParameterDirection.Output;
                    command.Parameters.Add(sumOfUnpaidInvoices);

                    #endregion

                    // Execute the stored procedure
                    command.ExecuteNonQuery();

                    //Get results
                    int amountOpenEstimates;
                    bool result = Int32.TryParse(amountOfOpenEstimates.Value.ToString(), out amountOpenEstimates);
                    if (result) AmountOfOpenEstimates = amountOpenEstimates;

                    double sumOpenEstimates;
                    result = Double.TryParse(sumOfOpenEstimates.Value.ToString(), out sumOpenEstimates);
                    if (result) SumOfOpenEstimates = sumOpenEstimates;

                    double sumOpenWorkOrders;
                    result = Double.TryParse(sumOfOpenWorkOrders.Value.ToString(), out sumOpenWorkOrders);
                    if (result) SumOfOpenWorkOrders = sumOpenWorkOrders;

                    double sumPaidInvoices;
                    result = Double.TryParse(sumOfPaidInvoices.Value.ToString(), out sumPaidInvoices);
                    if (result) SumOfPaidInvoices = sumPaidInvoices;

                    double sumUnpaidInvoices;
                    result = Double.TryParse(sumOfUnpaidInvoices.Value.ToString(), out sumUnpaidInvoices);
                    if (result) SumOfUnpaidInvoices = sumUnpaidInvoices;

                    transaction.Commit();
                }
            }
        }

        public IList<Customer> GetCustomersForFilter()
        {
            return null;
        }
        
    }
}
