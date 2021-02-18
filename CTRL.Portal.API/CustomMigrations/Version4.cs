using FluentMigrator;
using FluentMigrator.SqlServer;

namespace CTRL.Portal.API.CustomMigrations
{
    [Migration(4)]
    public class Version4 : Migration
    {
        public override void Down()
        {
            Delete.Table("ResetCodes");
        }

        public override void Up()
        {
            Create.Table("ResetCodes")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity(1, 1)
                .WithColumn("Email").AsString().NotNullable()
                .WithColumn("Expiration").AsDateTime().NotNullable()
                .WithColumn("ResetCode").AsString(6);
        }
    }
}
