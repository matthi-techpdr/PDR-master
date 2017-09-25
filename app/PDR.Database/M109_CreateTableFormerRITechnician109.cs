using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    class M109_CreateTableFormerRITechnician109 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("FormerRIs")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNotNullableColumn("EmployeeFk", DbType.Int64)
                .WithNotNullableColumn("CompanyFk", DbType.Int64)
                .WithNotNullableColumn("RoleChangeDate", DbType.DateTime);

            db.Tables["FormerRIs"].AddForeignKeyTo("Users").Through("EmployeeFk", "Id");
            db.Tables["FormerRIs"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
        }
    }
}