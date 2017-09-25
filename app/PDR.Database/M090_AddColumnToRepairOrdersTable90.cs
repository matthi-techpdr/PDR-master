using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M090_AddColumnToRepairOrdersTable90 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["RepairOrders"]
                .AddNullableColumn("WorkByThemselve", DbType.Boolean);
        }
    }
}