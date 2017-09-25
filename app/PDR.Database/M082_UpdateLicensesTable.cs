using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M082_UpdateLicensesTable82 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Licenses"].AddNullableColumn("DeviceToken", DbType.String).OfSize(255);
        }
    }
}