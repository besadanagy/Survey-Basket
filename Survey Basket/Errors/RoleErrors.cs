namespace Survey_Basket.Errors
{
    public class RoleErrors
    {
        //public static readonly Error InvalidCredentials =
        //   new("User.InvalidCredentials", "Invalid email / password", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidPermission =
            new("Role.InvalidPermission", "Invalid Permission", StatusCodes.Status400BadRequest);

        //public static readonly Error InvalidRefreshToken =
        //    new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);

        public static readonly Error DublicatedRole=
            new("Role.DublicatedRole", "Dublicated Role", StatusCodes.Status409Conflict);

        public static readonly Error RoleNotFound =
            new("Role.RoleNotFound", "Role Not Found", StatusCodes.Status404NotFound);
        //public static readonly Error InvalidCode =
        //    new("User.InvalidCode", "Invalid Code", StatusCodes.Status401Unauthorized);
        //public static readonly Error EmailIsConfirmed =
        //    new("User.EmailIsConfirmed", "Email already Confirmed", StatusCodes.Status409Conflict);


    }
}
