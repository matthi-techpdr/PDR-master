namespace PDR.Database
{
    using System.Data;

    using MigSharp;

    [MigrationExport]
    public class M049_AddNewColumnsToCompanyTable49 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Companies"]
                .AddNullableColumn("DefaultHourlyRate", DbType.Int16)
                .AddNullableColumn("LimitForBodyPartEstimate", DbType.Int16)
                .AddNullableColumn("LogoFk", DbType.Int64);

            db.Tables["Companies"].AddForeignKeyTo("Photos").Through("LogoFk", "Id");
        }
    }
}