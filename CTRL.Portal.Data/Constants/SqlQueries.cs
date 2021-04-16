﻿namespace CTRL.Portal.Data.Constants
{
    public class SqlQueries
    {
        public static readonly string GetUserSettings = "SELECT * FROM UserSettings WHERE [UserName] = @UserName";
        public static readonly string SaveUserSettings =
            @"IF EXISTS (SELECT * FROM UserSettings WHERE [UserName] = @UserName)
                UPDATE UserSettings
                    SET [Theme] = @Theme,
                        [DefaultAccount] = @DefaultAccount
                WHERE [UserName] = @UserName;
            ELSE
                INSERT INTO UserSettings (UserName, Theme, DefaultAccount) VALUES (@UserName, @Theme, @DefaultAccount)
            ";
        public static readonly string AddAccountQuery = "INSERT INTO accounts (id, name) VALUES (@Id, @Name)";
        public static readonly string AddAccountToUser = "INSERT INTO UserAccounts (UserName, AccountId) VALUES (@UserName, @AccountId)";
        public static readonly string GetAllAccountsQuery =
            @"SELECT a.Id, a.Name FROM UserAccounts ua 
	            INNER JOIN Accounts a
		            ON a.Id = ua.AccountId
		    WHERE UserName = @UserName";
        public static readonly string AddCode = "INSERT INTO Codes(Id, Email, Expiration, Code) VALUES (@Id, @Email, @Expiration, @Code)";
        public static readonly string GetAccountById = "SELECT * FROM accounts WHERE [Id] = @Id";
        public static readonly string GetCode = "SELECT * FROM Codes WHERE [Code] = @Code AND [Email] = @Email AND [Expiration] >= GETDATE()";
        public static readonly string AddAccountCode = 
            @"INSERT INTO AccountCodes(Id, AccountId, CodeId, Accepted)
                VALUES (@Id, @AccountId, @CodeId, @Accepted)";
        public static readonly string GetAccountCodeByCodeId = 
            @"SELECT * FROM AccountCodes ac
            INNER JOIN Codes c ON c.Id = ac.CodeId WHERE c.code = @Code";
        public static readonly string UpdateCodeStatus = "UPDATE AccountCodes SET Accepted = 'True' WHERE [CodeId] = @CodeId";
        public static string AddSubscription = "INSERT INTO Subscriptions(Id, AccountId, Name) VALUES (@Id, @AccountId, @Name)";
        public static string AddModuleToSubscription = @"INSERT INTO SubscriptionModules(Id, ModuleId, SubscriptionId)
            VALUES (@Id, @ModuleId, @SubscriptionId)";
    }
}
