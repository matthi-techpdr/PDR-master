using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M055_UpdateCompanyTable55 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Companies"].AddNullableColumn("DefaultMatrixFk", DbType.Int64);
        }
    }
}