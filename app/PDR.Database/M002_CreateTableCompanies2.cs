using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M002_CreateTableCompanies2 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("Companies")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("Name", DbType.String).OfSize(255)
                .WithNullableColumn("Address1", DbType.String).OfSize(255)
                .WithNullableColumn("Address2", DbType.String).OfSize(255)
                .WithNullableColumn("State", DbType.Int16)
                .WithNullableColumn("Status", DbType.Int16)
                .WithNullableColumn("Zip", DbType.String).OfSize(255)
                .WithNullableColumn("Email", DbType.String).OfSize(255)
                .WithNullableColumn("PhoneNumber", DbType.String).OfSize(255)
                .WithNullableColumn("Comment", DbType.String).OfSize(1100)
                .WithNullableColumn("MobileLicensesNumber", DbType.Int32)
                .WithNullableColumn("ActiveUsersNumber", DbType.Int32)
                .WithNullableColumn("Url", DbType.String).OfSize(255)
                .WithNullableColumn("CreationDate", DbType.DateTime);
        }
    }
}
