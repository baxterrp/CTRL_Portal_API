namespace CTRL.Portal.API.APIConstants
{
    public static class ApiMessages
    {
        public static readonly string Unauthorized = "Unauthorized";
        public static readonly string LoginSuccessful = "Successfully logged in";
        public static readonly string InvalidCredentials = "Invalid credentials given";
        public static readonly string UserAlreadyExists = "User with that name already exists";
        public static readonly string UnhandledErrorCreatingUser = "Unhandled error happened creating user";
        public static readonly string InvalidPassword = "Password must be 8 characters, alphanumeric, and contain special characters";
    }
}
