using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M101_UpdateLogsTable101 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Logs"].Columns["NewTeamEmployees"].AlterToNullable(DbType.String).OfSize(510);
        }
    }
}