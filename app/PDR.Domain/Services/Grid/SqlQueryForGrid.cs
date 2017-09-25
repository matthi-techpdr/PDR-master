// -----------------------------------------------------------------------
// <copyright file="SqlQueryForGrid.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace PDR.Domain.Services.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Practices.ServiceLocation;

    using NHibernate;
    using NHibernate.Linq;

    using PDR.Domain.Model;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SqlQueryForGrid<TEntity>
    {
        private readonly ISession session;

        public SqlQueryForGrid()
        {
            this.session = ServiceLocator.Current.GetInstance<ISession>();
        }

        public const string RoQueryForGrid = "SELECT TOP {0} * FROM [PDR_test2].[dbo].[RepairOrders] Ord INNER JOIN (SELECT * FROM   [PDR_test2].[dbo].[TeamEmployeePercents] teamemploy0_ WHERE  teamemploy0_.TeamEmployeeFk = {1}) TeamEmpPerc ON Ord.Id = TeamEmpPerc.RepairOrderFk ORDER BY CreationDate DESC;";
        public const string RoCount = "SELECT COUNT(*) FROM [PDR_test2].[dbo].[RepairOrders] Ord INNER JOIN (SELECT * FROM   [PDR_test2].[dbo].[TeamEmployeePercents] teamemploy0_ WHERE  teamemploy0_.TeamEmployeeFk = {0}) TeamEmpPerc ON Ord.Id = TeamEmpPerc.RepairOrderFk;";
        public IEnumerable<TEntity> GetRepairOrders(long userId, int rows)
        {
            var query = String.Format(RoQueryForGrid, rows, userId);
            var result = session.CreateSQLQuery(query)
                .AddEntity(typeof(TEntity))
                .List<TEntity>();
            return (IEnumerable<TEntity>)result;
        }

        public int CountRepairOrders(long userId)
        {
            var query = String.Format(RoCount, userId);
            var result = (int)session.CreateSQLQuery(query).UniqueResult();
            return result;
        } 

    }
}
