using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M093_UpdateCompanyTable93 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Companies"].AddNullableColumn("Notes", DbType.String).OfSize(255);
        }
    }
}