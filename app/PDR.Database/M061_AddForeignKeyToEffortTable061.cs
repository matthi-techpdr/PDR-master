using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M061_AddForeignKeyToEffortTable061 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["CarSectionsPrices"].AddForeignKeyTo("CarModels", "Fk_carSecPr_carModel").Through("CarModelFk", "Id");
            db.Tables["EffortItems"].AddForeignKeyTo("CarSectionsPrices", "Fk_effortIt_carSecPr").Through("CarSectionsPricesFk", "Id");
        }
    }
}
