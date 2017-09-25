using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M083_UpdateLogsTable85 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Logs"].AddNullableColumn("EntityId", DbType.Int64);
        }
    }
}