using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M070_UpdateUsersTable70 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Users"]
                .AddNullableColumn("ZIP", DbType.String).OfSize(255)
                .AddNullableColumn("City", DbType.String).OfSize(255)
                .AddNullableColumn("State", DbType.Int16);
        }
    }
}
