namespace Survey_Basket.Abstractions.Consts
{
    public static class Permissions
    {
        public static string Type { get; } = "Permissions";

        public const string GetPolls = "Polls:read";
        public const string addPolls = "Polls:add";
        public const string updatePolls = "Polls:update";
        public const string deletePolls = "Polls:delete";


        public const string GetQuestions = "Question:read";
        public const string addQuestions = "Question:add";
        public const string updateQuestions = "Question:update";

        public const string GetUsers = "Users:read";
        public const string addUsers = "Users:add";
        public const string updateUsers = "Users:update";

        public const string GetRoles = "Roles:read";
        public const string addRoles = "Roles:add";
        public const string updateRoles = "Roles:update";

        public const string GetResults = "Results:read";

        public static IList<string?> GetAllPermissions() =>
            typeof(Permissions).GetFields().Select(x=>x.GetValue(x) as string).ToList();

    }
}
