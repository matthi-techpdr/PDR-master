using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M044_CreateLogsTable44 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("Logs")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("EmployeeFk", DbType.Int64)
                .WithNullableColumn("Class", DbType.String).OfSize(255)
                .WithNullableColumn("EntityFk", DbType.Int64)
                .WithNullableColumn("Action", DbType.Int32)
                .WithNullableColumn("Date", DbType.DateTime)
                .WithNullableColumn("NewOwnerFk", DbType.Int64)
                .WithNullableColumn("CompanyFk", DbType.Int64);

            db.Tables["Logs"].AddForeignKeyTo("Users").Through("EmployeeFk", "Id");
            db.Tables["Logs"].AddForeignKeyTo("Users", "FK_logs_newOwner").Through("NewOwnerFk", "Id");
            db.Tables["Logs"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
        }
    }
}

