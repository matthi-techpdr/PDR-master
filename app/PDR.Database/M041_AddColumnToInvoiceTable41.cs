using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M041_AddColumnToInvoiceTable41 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Invoices"].AddNullableColumn("InvoiceSum", DbType.Double);
        }
    }
}
