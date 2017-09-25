using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M083_UpdateLogsTable83 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Logs"].AddNullableColumn("NewTeamEmployees", DbType.String).OfSize(255);
        }
    }
}