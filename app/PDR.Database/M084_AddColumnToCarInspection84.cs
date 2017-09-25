using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M084_AddColumnToCarInspection84 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["CarInspections"].AddNullableColumn("HasAlert", DbType.Boolean);
        }
    }
}
