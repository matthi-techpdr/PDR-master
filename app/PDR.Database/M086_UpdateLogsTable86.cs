using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M086_UpdateLogsTable86 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Logs"].AddNullableColumn("Emails", DbType.String).OfSize(255);
        }
    }
}