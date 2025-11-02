using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Survey_Basket.Migrations
{
    /// <inheritdoc />
    public partial class addIsDiaablesColumnToUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "947231F5-74DA-4CB0-A20D-543F11E3BCD6",
                columns: new[] { "IsDisabled", "PasswordHash" },
                values: new object[] { false, "AQAAAAIAAYagAAAAENPzhbnGZrWFRPrmbAQVDGY+jRCwTMhsmrumK4aXRNim+ox8+0n1H4XqUkWp2gbWEw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "947231F5-74DA-4CB0-A20D-543F11E3BCD6",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEB5eBmNX0sqrhjxPrMn4NF8HuD8g6mrbzGhfWBeUE4DW5HuNShKSNEs88clicXt8PA==");
        }
    }
}
