using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M035_CreateRepairOrdersTables35 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("RepairOrders")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("CreationDate", DbType.DateTime)
                .WithNullableColumn("EstimateFk", DbType.Int64)
                .WithNullableColumn("RepairOrderStatus", DbType.Int16)
                .WithNullableColumn("TotalSum", DbType.Double)
                .WithNullableColumn("IsConfirmed", DbType.Boolean)
                .WithNullableColumn("SupplementsApproved", DbType.Boolean)
                .WithNullableColumn("TeamEmployeeFk", DbType.Int64)
                .WithNullableColumn("New", DbType.Boolean)
                .WithNullableColumn("CompanyFk", DbType.Int64);

            db.CreateTable("Supplements")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("Description", DbType.String).OfSize(255)
                .WithNullableColumn("RepairOrderFk", DbType.Int64)
                .WithNullableColumn("Sum", DbType.Double)
                .WithNullableColumn("CompanyFk", DbType.Int64);

            db.Tables["RepairOrders"].AddForeignKeyTo("Companies", "FK_r0_company").Through("CompanyFk", "Id");
            db.Tables["RepairOrders"].AddForeignKeyTo("Users", "FK_r0_user").Through("TeamEmployeeFk", "Id");

            db.Tables["Supplements"].AddForeignKeyTo("RepairOrders", "FK_sup_rp").Through("RepairOrderFk", "Id");
            db.Tables["Supplements"].AddForeignKeyTo("Companies", "FK_sup_company").Through("CompanyFk", "Id");
        }
    }
}