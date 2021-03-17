using CTRL.Portal.Migrations.Metadata;
using FluentMigrator;
using FluentMigrator.SqlServer;

namespace CTRL.Portal.Migrations.Custom
{
    [Migration(2)]
    public class Version2 : Migration
    {
        public override void Down()
        {
            Delete.Table(Names.UserSettingsTable);
        }

        public override void Up()
        {
            Create.Table(Names.UserSettingsTable)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity(1, 1)
                .WithColumn("Theme").AsString().Nullable()
                .WithColumn("UserName").AsString().NotNullable();
        }
    }
}
