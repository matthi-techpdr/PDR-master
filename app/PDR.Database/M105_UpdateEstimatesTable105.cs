using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M105_UpdateEstimatesTable105 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Estimates"].AddNullableColumn("NewLaborRate", DbType.Double);
        }
    }
}
