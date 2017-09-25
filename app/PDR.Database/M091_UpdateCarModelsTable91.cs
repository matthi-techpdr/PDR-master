using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M091_UpdateCarModelsTable91 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["CarModels"]
                .AddNullableColumn("YearTo", DbType.Int32)
                .AddNullableColumn("Type", DbType.Int32);
        }
    }
}