using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M104_CreateInsuranceCompanyTable104 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("InsuranceCompanies")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNotNullableColumn("Name", DbType.String).OfSize(255)
                .WithNullableColumn("CompanyFk", DbType.Int64);

            db.Tables["InsuranceCompanies"].AddForeignKeyTo("Companies", "FK_insCom_company").Through("CompanyFk", "Id");
        }
    }
}
