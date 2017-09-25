namespace PDR.Database
{
    using MigSharp;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [MigrationExport]
    public class M020_CreateAllReferencesToCompany20 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Cars"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
            db.Tables["Insurances"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
            db.Tables["Customers"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
            db.Tables["Teams"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
            db.Tables["Licenses"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
            db.Tables["Matrices"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
            db.Tables["CustomLines"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
            db.Tables["ChosenEffortItems"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
            db.Tables["EffortItems"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
            db.Tables["Photos"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
            db.Tables["CarInspections"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
            db.Tables["Estimates"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
            db.Tables["Prices"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
        }
    }
}
