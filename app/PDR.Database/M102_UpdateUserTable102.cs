using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M102_UpdateUserTable102 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Users"].AddNotNullableColumn("IsBasic", DbType.Boolean).HavingDefault(0);
        }
    }
}
