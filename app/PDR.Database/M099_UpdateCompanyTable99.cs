using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M099_UpdateCompanyTable99 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Companies"].AddNullableColumn("RepairOrdersEmailSubject", DbType.String).OfSize(255);
        }
    }
}