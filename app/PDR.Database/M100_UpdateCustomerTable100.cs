using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M100_UpdateCustomerTable100 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Customers"].AddNullableColumn("Email2", DbType.String).OfSize(255);
            db.Tables["Customers"].AddNullableColumn("Email3", DbType.String).OfSize(255);
            db.Tables["Customers"].AddNullableColumn("Email4", DbType.String).OfSize(255);
        }
    }
}