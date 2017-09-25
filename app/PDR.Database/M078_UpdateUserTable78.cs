using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M078_UpdateUserTable78 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Users"].AddNullableColumn("SignatureName", DbType.String).OfSize(255);
        }
    }
}
