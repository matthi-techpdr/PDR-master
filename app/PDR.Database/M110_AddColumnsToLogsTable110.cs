using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    class M110_AddColumnsToLogsTable110 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["logs"].AddNullableColumn("OldValue", DbType.Double);
            db.Tables["logs"].AddNullableColumn("NewValue", DbType.Double);
        }
    }
}