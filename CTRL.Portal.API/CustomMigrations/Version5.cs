﻿using FluentMigrator;
using FluentMigrator.SqlServer;

namespace CTRL.Portal.API.CustomMigrations
{
    [Migration(5)]
    public class Version5 : Migration
    {
        public override void Down()
        {
            Delete.Table("AccountCode");
        }

        public override void Up()
        {
            Create.Table("AccountCodes")
                .WithColumn("Id").AsString().NotNullable().PrimaryKey()
                .WithColumn("AccountId").AsString().NotNullable().ForeignKey()
                .WithColumn("Code").AsString(6).NotNullable().ForeignKey()
                .WithColumn("Accepted").AsBoolean().NotNullable().WithDefaultValue("false");

            Create.ForeignKey()
                .FromTable("AccountCodes").ForeignColumn("AccountId")
                .ToTable("Accounts").PrimaryColumn("Id");

            //Create.ForeignKey()
            //    .FromTable("AccountCodes").ForeignColumn("Code")
            //    .ToTable("Codes").PrimaryColumns("Code");
        }
    }
}

