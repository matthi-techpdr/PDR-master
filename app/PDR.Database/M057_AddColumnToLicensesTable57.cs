using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M057_AddColumnToLicensesTable57 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Licenses"]
                .AddNullableColumn("LicenseNumber", DbType.String).OfSize(255);
        }
    }
}
