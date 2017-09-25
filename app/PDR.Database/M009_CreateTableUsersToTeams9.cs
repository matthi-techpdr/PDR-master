using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M009_CreateTableUsersToTeams9 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("TeamEmployees_Teams")
                .WithPrimaryKeyColumn("TeamEmployeeFk", DbType.Int64)
                .WithPrimaryKeyColumn("TeamFk", DbType.Int64);

            db.Tables["TeamEmployees_Teams"].AddForeignKeyTo("Users").Through("TeamEmployeeFk", "Id");
            db.Tables["TeamEmployees_Teams"].AddForeignKeyTo("Teams").Through("TeamFk", "Id");
        }
    }
}
