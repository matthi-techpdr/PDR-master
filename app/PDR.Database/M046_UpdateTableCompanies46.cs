using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M046_UpdateTableCompanies46 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Companies"].AddNullableColumn("City", DbType.String).OfSize(255);
        }
    }
}