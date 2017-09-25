namespace PDR.Database
{
    using MigSharp;

    [MigrationExport]
    public class M011_AddFkToUsersFromLicensees11 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Licenses"].AddForeignKeyTo("Users").Through("EmployeeFk", "Id");
        }
    }
}
