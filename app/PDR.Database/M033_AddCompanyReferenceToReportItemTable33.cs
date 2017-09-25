using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M033_AddCompanyReferenceToReportItemTable33 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["EstimateReportItems"].AddNullableColumn("CompanyFk", DbType.Int64);
        }
    }
}
