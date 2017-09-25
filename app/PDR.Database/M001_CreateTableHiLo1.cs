namespace PDR.Database
{
    using System.Data;

    using MigSharp;

    [MigrationExport]
    public class M001_CreateTableHiLo1 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("Hibernate_unique_key")
                .WithNotNullableColumn("next_hi", DbType.Int32);

            db.Execute("INSERT INTO Hibernate_unique_key VALUES (2)");
        }
    }
}