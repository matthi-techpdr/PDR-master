using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M050_AddClassToPhotoTable50 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Photos"].AddNullableColumn("Class", DbType.String).OfSize(255);
        }
    }
}
