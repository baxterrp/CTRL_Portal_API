using FluentMigrator;
using FluentMigrator.SqlServer;

namespace CTRL.Portal.API.CustomMigrations
{
    [Migration(5)]
    public class Version5 : Migration
    {
        public override void Down()
        {
            Delete.Table("Codes");
        }

        public override void Up()
        {
            Create.Table("Codes")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity(1, 1)
                .WithColumn("Email").AsString().NotNullable()
                .WithColumn("Expiration").AsDateTime().NotNullable()
                .WithColumn("ResetCode").AsString(6);
        }
    }
}
