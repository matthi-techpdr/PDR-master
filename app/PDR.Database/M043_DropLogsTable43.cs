using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M043_DropLogsTable43 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Logs"].Drop();
        }
    }
}