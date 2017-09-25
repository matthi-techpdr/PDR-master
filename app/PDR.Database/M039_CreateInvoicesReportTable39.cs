using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M039_CreateInvoicesReportTable39 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("InvoiceReports")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("Date", DbType.DateTime)
                .WithNullableColumn("Title", DbType.String).OfSize(255)
                .WithNullableColumn("CompanyFk", DbType.Int64)
                .WithNullableColumn("EmployeeFk", DbType.Int64);

            db.CreateTable("InvoiceReportItems")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("CreationDate", DbType.DateTime)
                .WithNullableColumn("CustomerName", DbType.String).OfSize(255)
                .WithNullableColumn("InvoiceSum", DbType.Double)
                .WithNullableColumn("PaidSum", DbType.Double)
                .WithNullableColumn("Status", DbType.Int64)
                .WithNullableColumn("InvoiceReportFk", DbType.Int64)
                .WithNullableColumn("CompanyFk", DbType.Int64);

            db.Tables["InvoiceReportItems"].AddForeignKeyTo("InvoiceReports", "Fk_invRepIt_invRep").Through("InvoiceReportFk", "Id");
            db.Tables["InvoiceReports"].AddForeignKeyTo("Companies", "Fk_invRep_com").Through("CompanyFk", "Id");
            db.Tables["InvoiceReports"].AddForeignKeyTo("Users", "Fk_invRep_emp").Through("EmployeeFk", "Id");
        } 
    }
}
