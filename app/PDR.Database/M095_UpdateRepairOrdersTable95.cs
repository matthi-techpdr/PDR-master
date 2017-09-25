using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M095_UpdateRepairOrdersTable95 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["RepairOrders"].AddNullableColumn("IsFlatFee", DbType.Boolean);
            db.Tables["RepairOrders"].AddNullableColumn("Payment", DbType.Double);
        }
    }
}