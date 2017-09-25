using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M081_UpdateChosenEfforts81 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["ChosenEffortItems"].AddNullableColumn("ChosenEffortType", DbType.Int16);
        }
    }
}