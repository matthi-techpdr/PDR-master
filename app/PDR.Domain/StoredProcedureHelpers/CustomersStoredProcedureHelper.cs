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
    public class CustomersStoredProcedureHelper : IStoredProcedureHelper
    {
        private readonly ISession session;

        private IList<string> Parameters { get; set; }

        private const string SP = "exec [dbo].[GetCustomerModels] ";

        public IList<CustomerModel> CustomerModels { get; set; }

        public IList<CustomerModel> CustomerModelsFirstPage
        {
            get
            {
                GetFirstPage();
                return this.CustomerModels;
            }
            protected set { }
        }

        public int TotalCountRows { get; set; }

        public CustomersStoredProcedureHelper(long userId, string customerType, long? filterByTeam = null, long? filterByStates = null, bool? isOnlyOwn = null, int? rowsPerPage = null,
                                                int? pageNumber = null, string sortType = null)
        {
             session = ServiceLocator.Current.GetInstance<ISession>();
             #region Set input parameters
             Parameters = new List<string>();
             Parameters.Add(String.Format("@userId = {0}", userId));

             if (filterByTeam.HasValue && filterByTeam.Value != 0)
             {
                 Parameters.Add(String.Format(", @FilterByTeam = {0}", filterByTeam.Value));
             }
             if (filterByStates.HasValue)
             {
                 Parameters.Add(String.Format(", @FilterByStates = {0}", filterByStates.Value));
             }
             if (isOnlyOwn.HasValue)
             {
                 Parameters.Add(String.Format(", @IsOnlyOwn = {0}", Convert.ToSByte(isOnlyOwn.Value)));
             }
             if (rowsPerPage.HasValue)
             {
                 Parameters.Add(String.Format(", @RowsPerPage = {0}", rowsPerPage.Value));
             }
             if (pageNumber.HasValue)
             {
                 Parameters.Add(String.Format(", @PageNumber = {0}", pageNumber.Value));
             }
             if (!String.IsNullOrEmpty(sortType) && ((sortType.ToUpper() == "DESC") || (sortType.ToUpper() == "ASC")))
             {
                 Parameters.Add(String.Format(", @SortType = '{0}'", sortType));
             }
             if (!String.IsNullOrEmpty(customerType) && ((customerType.ToLower() == "wholesalecustomer") || (customerType.ToLower() == "affiliate")))
             {
                 Parameters.Add(String.Format(", @CustomerType = '{0}'", customerType));
             }

             #endregion
             this.CustomerModels = new List<CustomerModel>();
             this.CustomerModelsFirstPage = new List<CustomerModel>();
             ExecutStoredProcedure();
        }

        private void ExecutStoredProcedure()
        {
            var query = SP;
            for (int i = 0; i < Parameters.Count; i++)
            {
                query += Parameters[i];
            }

            try
            {
                var objects =
                   session.CreateSQLQuery(query)
                       .AddScalar("Id", NHibernateUtil.Int64)
                       .AddScalar("Name", NHibernateUtil.String)
                       .AddScalar("State", NHibernateUtil.Int16)
                       .AddScalar("Phone", NHibernateUtil.String)
                       .AddScalar("City", NHibernateUtil.String)
                       .AddScalar("Email", NHibernateUtil.String)
                       .AddScalar("Email2", NHibernateUtil.String)
                       .AddScalar("Email3", NHibernateUtil.String)
                       .AddScalar("Email4", NHibernateUtil.String)
                       .AddScalar("TotalCountRows", NHibernateUtil.Int32)
                       .List();

                for (int i = 0; i < objects.Count; i++)
                {
                    var item = (object[])objects[i];
                    var customer = new CustomerModel();
                    customer.Id = item[0] == null ? "" : item[0].ToString();
                    customer.Name = item[1] == null ? "" : item[1].ToString();
                    customer.State = item[2] == null ? (StatesOfUSA?)null : ((StatesOfUSA)Convert.ToInt32(item[2]));
                    customer.Phone = item[3] == null ? "" : item[3].ToString();
                    customer.City = item[4] == null ? "" : item[4].ToString();
                    customer.Email = item[5] == null ? "" : item[5].ToString();
                    customer.Email2 = item[6] == null ? "" : item[6].ToString();
                    customer.Email3 = item[7] == null ? "" : item[7].ToString();
                    customer.Email4 = item[8] == null ? "" : item[8].ToString();
                    this.CustomerModels.Add(customer);
                    if (i == 0)
                    {
                        TotalCountRows = item[9] == null ? 0 : Convert.ToInt32(item[9]);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public IList<Customer> GetCustomersForFilter()
        {
            return null;
        }
        
        private void GetFirstPage()
        {
            var pageNumber = Parameters.FirstOrDefault(x => x.Contains("@PageNumber"));
            if (pageNumber != null)
            {
                Parameters.Remove(pageNumber);
                Parameters.Add(", @PageNumber = 1");
            }
            else
            {
                Parameters.Add(String.Format(", @PageNumber = {0}", 1));
            }
            ExecutStoredProcedure();
        }
    }
}
