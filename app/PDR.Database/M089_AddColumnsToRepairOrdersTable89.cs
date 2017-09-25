using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M089_AddColumnsToRepairOrdersTable89 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Estimates"]
                .AddNullableColumn("WorkByThemselve", DbType.Boolean);
            db.Tables["RepairOrders"]
                .AddNullableColumn("RetailDiscount", DbType.Int32)
                .AddNullableColumn("AdditionalDiscount", DbType.Double);
        }
    }
}