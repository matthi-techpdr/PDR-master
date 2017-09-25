using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M062_AddColumnToChosenEffortItemsTable62 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["ChosenEffortItems"]
                .AddNullableColumn("Hours", DbType.Double)
                .AddNullableColumn("Operations", DbType.Int16);
        }
    }
}
