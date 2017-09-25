using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M029_AddColumnToEstimateTable29 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Estimates"].AddNullableColumn("New", DbType.Boolean);
        }
    }
}