using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M048_AddColumnToInvoiceTable48 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Invoices"].AddNullableColumn("IsImported", DbType.Boolean);
        }
    }
}

