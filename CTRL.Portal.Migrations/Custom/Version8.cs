using CTRL.Portal.Migrations.Metadata;
using FluentMigrator;

namespace CTRL.Portal.Migrations.Custom
{
    [Migration(8)]
    public class Version8 : Migration
    {
        public override void Down()
        {
            Rename.Table(Names.BusinessEntities).To(Names.AccountsTable);
            Rename.Table(Names.UserBusinessEntitiesTable).To(Names.UserAccountsTable);
            Rename.Table(Names.BusinessEntityCodesTable).To(Names.AccountCodesTable);

            Rename.Column("BusinessEntityId").OnTable(Names.UserBusinessEntitiesTable).To("AccountId");
            Rename.Column("BusinessEntityId").OnTable(Names.SubscriptionsTable).To("AccountId");
            Rename.Column("BusinessEntityId").OnTable(Names.BusinessEntityCodesTable).To("AccountId");
            Rename.Column("DefaultBusiness").OnTable(Names.UserSettingsTable).To("DefaultAccount");
        }

        public override void Up()
        {
            Rename.Table(Names.AccountsTable).To(Names.BusinessEntities);
            Rename.Table(Names.UserAccountsTable).To(Names.UserBusinessEntitiesTable);
            Rename.Table(Names.AccountCodesTable).To(Names.BusinessEntityCodesTable);

            Rename.Column("AccountId").OnTable(Names.UserBusinessEntitiesTable).To("BusinessEntityId");
            Rename.Column("AccountId").OnTable(Names.SubscriptionsTable).To("BusinessEntityId");
            Rename.Column("AccountId").OnTable(Names.BusinessEntityCodesTable).To("BusinessEntityId");
            Rename.Column("DefaultAccount").OnTable(Names.UserSettingsTable).To("DefaultBusiness");
        }
    }
}
