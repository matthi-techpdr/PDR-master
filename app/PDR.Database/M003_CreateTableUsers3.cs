using System.Data;

using MigSharp;

namespace PDR.Database
{
	[MigrationExport]
    public class M003_CreateTableUsers3 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("Users")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("Login", DbType.String).OfSize(255)
                .WithNullableColumn("Password", DbType.String).OfSize(255)
                .WithNullableColumn("Class", DbType.String).OfSize(255)
                .WithNullableColumn("Name", DbType.String).OfSize(255)
                .WithNullableColumn("CompanyFk", DbType.Int64)
                .WithNullableColumn("PhoneNumber", DbType.String).OfSize(255)
                .WithNullableColumn("Address", DbType.String).OfSize(255)
                .WithNullableColumn("Email", DbType.String).OfSize(255)
                .WithNullableColumn("TaxId", DbType.String).OfSize(255)
                .WithNullableColumn("Commission", DbType.Int16)
                .WithNullableColumn("Comment", DbType.String).OfSize(1100)
                .WithNullableColumn("CanQuickEstimate", DbType.Boolean)
                .WithNullableColumn("CanExtraQuickEstimate", DbType.Boolean)
                .WithNullableColumn("HiringDate", DbType.DateTime)
                .WithNullableColumn("Status", DbType.Int16);
        }
    }
}
