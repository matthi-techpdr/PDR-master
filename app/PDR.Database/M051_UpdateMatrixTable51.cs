using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M050_UpdateMatrixTable51 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Matrices"].AddNullableColumn("Status", DbType.Int16);
        }
    }
}
