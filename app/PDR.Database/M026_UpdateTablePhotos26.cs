
using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M026_UpdateTablePhotos26 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Photos"].AddNullableColumn("ContentType", DbType.String).OfSize(32);
        }
    }
}