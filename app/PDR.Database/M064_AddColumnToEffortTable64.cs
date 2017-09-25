using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M064_AddColumnToEffortTable64 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["CarInspections"]
                .AddNullableColumn("DentsCost", DbType.Double)
                .AddNullableColumn("OversizedRoof", DbType.Boolean)
                .AddNullableColumn("OptionsPercent", DbType.Double)
                .AddNullableColumn("PartsTotal", DbType.Double);
        }
    }
}
