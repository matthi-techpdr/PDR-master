using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M096_AddColumnAffiliateToEstimateTable96 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Estimates"].AddNullableColumn("AffiliateFk", DbType.Int64);
            db.Tables["Estimates"].AddForeignKeyTo("Customers", "fk_est_affiliate").Through("AffiliateFk", "Id");
        }
    }
}