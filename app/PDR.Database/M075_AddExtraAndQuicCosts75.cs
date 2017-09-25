using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M075_AddExtraAndQuicCosts75 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["CarInspections"].AddNullableColumn("QuickCost", DbType.Double);
            db.Tables["Estimates"]
                .AddNullableColumn("ExtraQuickCost", DbType.Double)
                .AddNullableColumn("Type", DbType.Int16);
        }
    }
}
