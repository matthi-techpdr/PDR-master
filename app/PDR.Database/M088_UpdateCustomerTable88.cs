using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M088_UpdateCustomerTable88 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Customers"].Columns["PartRate"].Drop();
            db.Tables["Customers"].AddNullableColumn("PartRate", DbType.Double);
        }
    }
}