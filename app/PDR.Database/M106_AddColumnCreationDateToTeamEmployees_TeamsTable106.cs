using System.Data;

using MigSharp;

namespace PDR.Database
{
    using System;

    [MigrationExport]
    public class M106_AddColumnCreationDateToTeamEmployees_TeamsTable106 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["TeamEmployees_Teams"].AddNotNullableColumn("CreationDate", DbType.DateTime).HavingCurrentDateTimeAsDefault();
        }
    }
}