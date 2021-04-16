using CTRL.Portal.Migrations.Metadata;
using FluentMigrator;

namespace CTRL.Portal.Migrations.Custom
{
    [Migration(9)]
    public class Version9 : Migration
    {
        public override void Down()
        {
            Delete.Column("IsActive").FromTable(Names.UserSettingsTable);
        }

        public override void Up()
        {
            Alter.Table(Names.UserSettingsTable)
                .AddColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue("false");
        }
    }
}
