using CTRL.Portal.API.APIConstants;
using FluentMigrator;
using FluentMigrator.SqlServer;

namespace CTRL.Portal.API.CustomMigrations
{
    [Migration(2)]
    public class Version2 : Migration
    {
        public override void Down()
        {
            Delete.Table(ApiNames.UserSettingsTable);
        }

        public override void Up()
        {
            Create.Table(ApiNames.UserSettingsTable)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity(1, 1)
                .WithColumn("Theme").AsString().Nullable()
                .WithColumn("UserName").AsString().NotNullable();
        }
    }
}
