namespace Survey_Basket.Errors
{
    public class UserErrors
    {
        public static readonly Error InvalidCredentials =
           new("User.InvalidCredentials", "Invalid email / password", StatusCodes.Status401Unauthorized);

       public static readonly Error DisabledUser =
           new("User.DisabledUser", "Disabled User ,Please contact your adminstrator", StatusCodes.Status401Unauthorized);

       public static readonly Error LockedUser =
           new("User.LockedUser", "Locked User ,Please contact your adminstrator", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidJwtToken =
            new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidRefreshToken =
            new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);

        public static readonly Error DublicatedEmail =
            new("User.DublicatedEmail", "Dublicated Email", StatusCodes.Status409Conflict);

        public static readonly Error EmailNotConfirmed =
            new("User.EmailNotConfirmed", "Email is Not Confirmed", StatusCodes.Status401Unauthorized);
        public static readonly Error InvalidCode =
            new("User.InvalidCode", "Invalid Code", StatusCodes.Status401Unauthorized);
        public static readonly Error EmailIsConfirmed =
            new("User.EmailIsConfirmed", "Email already Confirmed", StatusCodes.Status409Conflict);

        public static readonly Error UserNotFound =
         new("User.UserNotFound", "User Not Found", StatusCodes.Status404NotFound);
        public static readonly Error InvalidRole =
        new("User.InvalidRole", "Invalid Role", StatusCodes.Status400BadRequest);



    }
}
