using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M080_UpdateEstimateTable80 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Estimates"].AddNullableColumn("CarImageFk", DbType.Int64);
        }
    }
}