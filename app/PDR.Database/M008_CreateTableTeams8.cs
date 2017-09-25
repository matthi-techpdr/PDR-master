namespace PDR.Database
{
    using System.Data;
    using MigSharp;

    [MigrationExport]
    public class M008_CreateTableTeams8 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("Teams")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("Title", DbType.String).OfSize(255)
                .WithNullableColumn("Status", DbType.Int16)
                .WithNullableColumn("CreationDate", DbType.DateTime)
                .WithNullableColumn("CompanyFk", DbType.Int64)
                .WithNullableColumn("Comment", DbType.String).OfSize(1100);
        }
    }
}
