namespace PDR.Database
{
    using System.Data;
    using MigSharp;

    [MigrationExport]
    public class M022_CreateTableLogs22 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("Logs")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNotNullableColumn("EmployeeFk", DbType.Int64)
                .WithNullableColumn("Action", DbType.String)
                .WithNullableColumn("Date", DbType.DateTime);

            db.Tables["Logs"].AddForeignKeyTo("Users").Through("EmployeeFk", "Id");
        }
    }
}
