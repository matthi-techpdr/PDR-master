using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M083_UpdateInvoicesTable86 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["RepairOrders"].AddNullableColumn("IsViewedByBoss", DbType.Boolean);
        }
    }
}