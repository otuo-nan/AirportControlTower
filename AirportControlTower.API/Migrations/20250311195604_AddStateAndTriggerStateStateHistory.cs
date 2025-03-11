using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportControlTower.API.Migrations
{
    /// <inheritdoc />
    public partial class AddStateAndTriggerStateStateHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "FromState",
                table: "StateChangeHistory",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Trigger",
                table: "StateChangeHistory",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromState",
                table: "StateChangeHistory");

            migrationBuilder.DropColumn(
                name: "Trigger",
                table: "StateChangeHistory");
        }
    }
}
