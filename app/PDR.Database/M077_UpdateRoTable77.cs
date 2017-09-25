using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M077_UpdateRoTable77 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["RepairOrders"].AddNullableColumn("Finalised", DbType.Boolean);
        }
    }
}