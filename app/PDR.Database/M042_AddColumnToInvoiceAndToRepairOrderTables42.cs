// -----------------------------------------------------------------------
// <copyright file="M042_AddColumnToInvoiceAndToRepairOrderTables42.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M042_AddColumnToInvoiceAndToRepairOrderTables42 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Invoices"].AddNullableColumn("CustomerFk", DbType.Int64);
            db.Tables["RepairOrders"].AddNullableColumn("CustomerFk", DbType.Int64);

            db.Tables["Invoices"].AddForeignKeyTo("Customers").Through("CustomerFk", "Id");
            db.Tables["RepairOrders"].AddForeignKeyTo("Customers").Through("CustomerFk", "Id");
        }
    }
}
