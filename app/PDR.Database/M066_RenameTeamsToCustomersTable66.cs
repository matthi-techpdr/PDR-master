using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M066_RenameTeamsToCustomersTable66 : IMigration
    {
        public void Up(IDatabase db)
        {
            //db.Tables["Teams_Customers"].Rename("Customers_Teams");
        }
    }
}
