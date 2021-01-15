namespace CTRL.Portal.Data.Constants
{
    public class SqlQueries
    {
        public static readonly string AddAccountQuery = "INSERT INTO accounts (id, name) VALUES (@Id, @Name)";
        public static readonly string AddAccountToUser = "INSERT INTO UserAccounts (UserName, AccountId) VALUES (@UserName, @AccountId)";
        public static readonly string GetAllAccountsQuery =
            @"SELECT a.Id, a.Name FROM UserAccounts ua 
	            INNER JOIN Accounts a
		            ON a.Id = ua.AccountId
		    WHERE UserName = @UserName";
    }
}
