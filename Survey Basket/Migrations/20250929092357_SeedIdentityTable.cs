using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Survey_Basket.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefualt", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "316a7978-47ad-4db8-adc7-3eec11cef3a8", "80b05d16-1856-4f99-b72e-866d3faffe9e", true, false, "member", "MEMBER" },
                    { "c48cdceb-c874-4c03-b234-3e2ebf756f10", "6f5d681f-65c6-4293-aa10-986d3c7d0e2c", false, false, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "947231F5-74DA-4CB0-A20D-543F11E3BCD6", 0, "88EF8745-7602-481E-B5DA-7B580276D730", "admin@Survey_Basket.com", true, "Survey Basket", "Admin", false, null, "ADMIN@SURVEY_BASKET.COM", "ADMIN@SURVEY_BASKET.COM", "AQAAAAIAAYagAAAAEB5eBmNX0sqrhjxPrMn4NF8HuD8g6mrbzGhfWBeUE4DW5HuNShKSNEs88clicXt8PA==", null, false, "B13885BC56C64E6498CE69E37BE47D6A", false, "admin@Survey_Basket.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "Permissions", "Polls:read", "c48cdceb-c874-4c03-b234-3e2ebf756f10" },
                    { 2, "Permissions", "Polls:add", "c48cdceb-c874-4c03-b234-3e2ebf756f10" },
                    { 3, "Permissions", "Polls:update", "c48cdceb-c874-4c03-b234-3e2ebf756f10" },
                    { 4, "Permissions", "Polls:delete", "c48cdceb-c874-4c03-b234-3e2ebf756f10" },
                    { 5, "Permissions", "Question:read", "c48cdceb-c874-4c03-b234-3e2ebf756f10" },
                    { 6, "Permissions", "Question:add", "c48cdceb-c874-4c03-b234-3e2ebf756f10" },
                    { 7, "Permissions", "Question:update", "c48cdceb-c874-4c03-b234-3e2ebf756f10" },
                    { 8, "Permissions", "Users:read", "c48cdceb-c874-4c03-b234-3e2ebf756f10" },
                    { 9, "Permissions", "Users:add", "c48cdceb-c874-4c03-b234-3e2ebf756f10" },
                    { 10, "Permissions", "Users:update", "c48cdceb-c874-4c03-b234-3e2ebf756f10" },
                    { 11, "Permissions", "Roles:read", "c48cdceb-c874-4c03-b234-3e2ebf756f10" },
                    { 12, "Permissions", "Roles:add", "c48cdceb-c874-4c03-b234-3e2ebf756f10" },
                    { 13, "Permissions", "Roles:update", "c48cdceb-c874-4c03-b234-3e2ebf756f10" },
                    { 14, "Permissions", "Results:read", "c48cdceb-c874-4c03-b234-3e2ebf756f10" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "c48cdceb-c874-4c03-b234-3e2ebf756f10", "947231F5-74DA-4CB0-A20D-543F11E3BCD6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "316a7978-47ad-4db8-adc7-3eec11cef3a8");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "c48cdceb-c874-4c03-b234-3e2ebf756f10", "947231F5-74DA-4CB0-A20D-543F11E3BCD6" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c48cdceb-c874-4c03-b234-3e2ebf756f10");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "947231F5-74DA-4CB0-A20D-543F11E3BCD6");
        }
    }
}
