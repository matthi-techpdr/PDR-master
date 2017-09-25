using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M097_AddColumnEditedStatuses97 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["RepairOrders"].AddNotNullableColumn("EditedStatus", DbType.Int16).HavingDefault(0);
        }
    }
}