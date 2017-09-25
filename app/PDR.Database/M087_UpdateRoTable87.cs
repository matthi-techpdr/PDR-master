using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M086_UpdateRoTable87 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["RepairOrders"].AddNullableColumn("RoCustomerSignatureFk", DbType.Int64);
        }
    }
}