using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M034_AddRepairOrderFkToPhotosTable34 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Photos"].AddNullableColumn("RepairOrderFk", DbType.Int64);
        }
    }
}