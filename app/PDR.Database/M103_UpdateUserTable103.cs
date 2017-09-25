using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M103_UpdateUserTable103 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Users"].AddNotNullableColumn("IsShowAllTeams", DbType.Boolean).HavingDefault(0);
        }
    }
}
