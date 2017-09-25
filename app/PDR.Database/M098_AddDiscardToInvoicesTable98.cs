using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M098_AddDiscardToInvoicesTable98 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Invoices"].AddNotNullableColumn("IsDiscard", DbType.Boolean).HavingDefault(0);
        }
    }
}