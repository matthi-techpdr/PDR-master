using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M079_AddVehicleTypeColumnToCarTable79: IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Cars"].AddNullableColumn("Type", DbType.Boolean);
        }
    }
}
