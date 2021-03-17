using CTRL.Portal.Migrations.Metadata;
using FluentMigrator;

namespace CTRL.Portal.Migrations.Custom
{
    [Migration(3)]
    public class Version3 : Migration
    {
        public override void Down()
        {
            Delete.Column("DefaultAccount")
                .FromTable(Names.UserSettingsTable);
        }

        public override void Up()
        {
            Alter.Table(Names.UserSettingsTable)
                .AddColumn("DefaultAccount").AsString().Nullable();
        }
    }
}
