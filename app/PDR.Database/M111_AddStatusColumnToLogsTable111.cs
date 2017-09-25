using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    class M111_AddStatusColumnToLogsTable111 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Logs"].AddNullableColumn("Status", DbType.Int64);
        }
    }
}