namespace PDR.Database
{
    using MigSharp;

    [MigrationExport]
    public class M007_AddFkToCompaniesFromUsers7 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Users"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
        }
    }
}
