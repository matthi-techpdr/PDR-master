using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M059_AddColumnToCarInspectionTable59 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["CarInspections"]
                .AddNullableColumn("PriorDamage", DbType.String).OfSize(255)
                .AddNullableColumn("CorrosionProtectionCost", DbType.Double);
        }
    }
}
