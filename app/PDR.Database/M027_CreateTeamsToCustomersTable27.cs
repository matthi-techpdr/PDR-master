using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M027_CreateTeamsToCustomersTable27 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("Customers_Teams")
                .WithNullableColumn("CustomerFk", DbType.Int64)
                .WithNullableColumn("TeamFk", DbType.Int64);

            db.Tables["Customers_Teams"].AddForeignKeyTo("Customers", "Fk_Cust_Team").Through("CustomerFk", "Id");
            db.Tables["Customers_Teams"].AddForeignKeyTo("Teams", "Fk_Team_Cust").Through("TeamFk", "Id");
        }
    }
}
