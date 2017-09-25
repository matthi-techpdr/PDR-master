using System.Data;

using MigSharp;

namespace PDR.Database
{
	[MigrationExport]
    public class M005_CreateTableInsurances5 : IMigration
	{
		public void Up(IDatabase db)
		{
			db.CreateTable("Insurances")
				.WithPrimaryKeyColumn("Id", DbType.Int64)
				.WithNullableColumn("InsuredName", DbType.String).OfSize(255)
                .WithNotNullableColumn("CompanyName", DbType.String).OfSize(255)
                .WithNullableColumn("Policy", DbType.String).OfSize(255)
                .WithNullableColumn("Claim", DbType.String).OfSize(255)
				.WithNullableColumn("ClaimDate", DbType.DateTime)
				.WithNullableColumn("AccidentDate", DbType.DateTime)
                .WithNullableColumn("Phone", DbType.String).OfSize(255)
                .WithNullableColumn("CompanyFk", DbType.Int64)
                .WithNullableColumn("ContactName", DbType.String).OfSize(255);
		}
	}
}
