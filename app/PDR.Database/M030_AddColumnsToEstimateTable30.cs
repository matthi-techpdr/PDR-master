using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M030_AddColumnsToEstimateTable30 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Estimates"].AddNullableColumn("UpdatedSum", DbType.Double);
            db.Tables["Estimates"].AddNullableColumn("CalculatedSum", DbType.Double);
        }
    }
}