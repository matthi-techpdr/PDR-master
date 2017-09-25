using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    class M107_AddColumnCanEditTeamMembersToUsersTable107: IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Users"].AddNullableColumn("CanEditTeamMembers", DbType.Boolean);
        }
    }
}
