using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M063_UpdateCompany63 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Companies"]
                .AddNullableColumn("EstimatesEmailSubject", DbType.String).OfSize(255)
                .AddNullableColumn("InvoicesEmailSubject", DbType.String).OfSize(255);
        }
    }
}
