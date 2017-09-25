using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M040_AddColumnToInvoiceTable40 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Invoices"].AddNullableColumn("Commission", DbType.String).OfSize(255);
            db.Tables["InvoiceReportItems"].AddNullableColumn("Commission", DbType.String).OfSize(255);
        }
    }
}
