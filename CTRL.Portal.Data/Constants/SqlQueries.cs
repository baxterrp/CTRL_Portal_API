namespace CTRL.Portal.Data.Constants
{
    public class SqlQueries
    {
        public static readonly string GetUserSettings = "SELECT * FROM UserSettings WHERE [UserName] = @UserName";
        public static readonly string SaveUserSettings =
            @"IF EXISTS (SELECT * FROM UserSettings WHERE [UserName] = @UserName)
                UPDATE UserSettings
                    SET [Theme] = @Theme,
                        [DefaultBusiness] = @DefaultBusiness,
                        [IsActive] = @IsActive
                WHERE [UserName] = @UserName;
            ELSE
                INSERT INTO UserSettings (UserName, Theme, DefaultBusiness, IsActive) VALUES (@UserName, @Theme, @DefaultBusiness, @IsActive)
            ";
        public static readonly string AddBusinessEntity = "INSERT INTO BusinessEntities (id, name) VALUES (@Id, @Name)";
        public static readonly string AddBusinessEntityToUser = "INSERT INTO UserBusinessEntities (UserName, BusinessEntityId) VALUES (@UserName, @BusinessEntityId)";
        public static readonly string GetAllBusinessEntitiesQuery =
            @"SELECT a.Id, a.Name FROM UserBusinessEntities ua 
	            INNER JOIN BusinessEntities a
		            ON a.Id = ua.BusinessEntityId
		    WHERE UserName = @UserName";
        public static readonly string AddCode = "INSERT INTO Codes(Id, Email, Expiration, Code) VALUES (@Id, @Email, @Expiration, @Code)";
        public static readonly string GetBusinessEntityById = "SELECT * FROM BusinessEntities WHERE [Id] = @Id";
        public static readonly string GetCode = "SELECT * FROM Codes WHERE [Code] = @Code AND [Email] = @Email AND [Expiration] >= GETDATE()";
        public static readonly string AddBusinessEntityCode =
            @"INSERT INTO BusinessEntityCodes(Id, BusinessEntityId, CodeId, Accepted)
                VALUES (@Id, @BusinessEntityId, @CodeId, @Accepted)";
        public static readonly string GetBussinessEntityCodeByCodeId =
            @"SELECT * FROM BusinessEntityCodes ac
            INNER JOIN Codes c ON c.Id = ac.CodeId WHERE c.code = @Code";
        public static readonly string UpdateCodeStatus = "UPDATE BusinessEntityCodes SET Accepted = 'True' WHERE [CodeId] = @CodeId";
        public static string AddSubscription = "INSERT INTO Subscriptions(Id, BusinessEntityId, Name) VALUES (@Id, @BusinessEntityId, @Name)";
        public static readonly string UpdateCodeExpiration = "UPDATE Codes SET Expiration = @Expiration WHERE [Id] = @Id";
    }
}
