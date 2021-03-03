using CTRL.Portal.API.APIConstants;
using FluentMigrator;

namespace CTRL.Portal.API.CustomMigrations
{
    [Migration(4)]
    public class Version4 : Migration
    {
        public override void Down()
        {
            Delete.Table(ApiNames.CodesTable);
        }

        public override void Up()
        {
            Create.Table(ApiNames.CodesTable)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey()
                .WithColumn("Email").AsString().NotNullable()
                .WithColumn("Expiration").AsDateTime().NotNullable()
                .WithColumn("Code").AsString(6);
        }
    }
}
