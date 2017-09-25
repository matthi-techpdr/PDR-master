using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M067_UpdateEstimateTable67 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Estimates"].AddNullableColumn("SignatureImage", DbType.Binary);
        }
    }
}
