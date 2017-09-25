using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M038_AddColumnToRepairOrdersTable38 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["RepairOrders"].AddNullableColumn("IsInvoice", DbType.Boolean);
        }
    }
}
