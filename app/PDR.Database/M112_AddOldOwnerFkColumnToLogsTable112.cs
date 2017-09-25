using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    class M112_AddOldOwnerFkColumnToLogsTable112 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Logs"].AddNullableColumn("OldOwnerFk", DbType.Int64);

            db.Tables["Logs"].AddForeignKeyTo("Users", "FK_Logs_OldOwner").Through("OldOwnerFk", "Id");
        }
    }
}
