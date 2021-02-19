using FluentMigrator;
using FluentMigrator.SqlServer;

namespace CTRL.Portal.API.CustomMigrations
{
    [Migration(4)]
    public class Version4 : Migration
    {
        public override void Down()
        {
            Delete.Table("Codes");
        }

        public override void Up()
        {
            Create.Table("Codes")
                .WithColumn("Id").AsString().NotNullable().PrimaryKey()
                .WithColumn("Email").AsString().NotNullable()
                .WithColumn("Expiration").AsDateTime().NotNullable()
                .WithColumn("Code").AsString(6);
        }
    }
}
