using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M069_UpdateEstimatesTable69 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Estimates"].AddNullableColumn("VINStatus", DbType.String);
        }
    }
}
