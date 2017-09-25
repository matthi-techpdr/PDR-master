using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M073_UpdateCustomersTable73 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Estimates"].AddNullableColumn("CustomerSignatureFk", DbType.Int64);
        }
    }
}
