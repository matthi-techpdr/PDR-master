using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M047_AddNameColumnToCustomerTable47 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Customers"].AddNullableColumn("Name", DbType.String).OfSize(255);
        }
    }
}