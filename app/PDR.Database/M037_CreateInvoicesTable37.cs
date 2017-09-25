using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M037_CreateInvoicesTable37 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("Invoices")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("CreationDate", DbType.DateTime)
                .WithNullableColumn("PaidSum", DbType.Double)
                .WithNullableColumn("PaidDate", DbType.DateTime)
                .WithNullableColumn("Status", DbType.Int64)
                .WithNullableColumn("RepairOrderFk", DbType.Int64)
                .WithNullableColumn("TeamEmployeeFk", DbType.Int64)
                .WithNullableColumn("New", DbType.Boolean)
                .WithNullableColumn("Archived", DbType.Boolean)
                .WithNullableColumn("CompanyFk", DbType.Int64);

            db.Tables["Invoices"].AddForeignKeyTo("RepairOrders", "FK_inv_rp").Through("RepairOrderFk", "Id");
            db.Tables["Invoices"].AddForeignKeyTo("Companies", "FK_inv_company").Through("CompanyFk", "Id");
            db.Tables["Invoices"].AddForeignKeyTo("Users", "FK_inv_user").Through("TeamEmployeeFk", "Id");
        }
    }
}
