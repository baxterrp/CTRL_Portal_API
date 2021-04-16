using CTRL.Portal.Migrations.Metadata;
using FluentMigrator;

namespace CTRL.Portal.Migrations.Custom
{
    [Migration(8)]
    public class Version8 : Migration
    {
        public override void Down()
        {
            Delete.Table("SubscriptionModules");
        }

        public override void Up()
        {
            Create.Table("SubscriptionModules")
               .WithColumn("Id").AsString().NotNullable().PrimaryKey()
               .WithColumn("ModuleId").AsString().NotNullable().ForeignKey()
               .WithColumn("SubscriptionId").AsString().NotNullable().ForeignKey();

            Create.ForeignKey()
                .FromTable("SubscriptionModules").ForeignColumn("ModuleId")
                .ToTable(Names.ModulesTable).PrimaryColumn("Id");

            Create.ForeignKey()
                .FromTable("SubscriptionModules").ForeignColumn("SubscriptionId")
                .ToTable(Names.SubscriptionsTable).PrimaryColumns("Id");
        }
    }
}
