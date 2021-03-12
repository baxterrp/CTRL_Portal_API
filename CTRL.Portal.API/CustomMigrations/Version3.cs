using CTRL.Portal.API.APIConstants;
using FluentMigrator;

namespace CTRL.Portal.API.CustomMigrations
{
    [Migration(3)]
    public class Version3 : Migration
    {
        public override void Down()
        {
            Delete.Column("DefaultAccount")
                .FromTable(ApiNames.UserSettingsTable);
        }

        public override void Up()
        {
            Alter.Table(ApiNames.UserSettingsTable)
                .AddColumn("DefaultAccount").AsString().Nullable();
        }
    }
}
