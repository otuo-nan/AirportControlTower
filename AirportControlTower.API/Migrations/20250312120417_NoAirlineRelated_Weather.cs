using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportControlTower.API.Migrations
{
    /// <inheritdoc />
    public partial class NoAirlineRelated_Weather : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Weather_Airlines_AirlineId",
                table: "Weather");

            migrationBuilder.DropIndex(
                name: "IX_Weather_AirlineId",
                table: "Weather");

            migrationBuilder.DropColumn(
                name: "AirlineId",
                table: "Weather");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AirlineId",
                table: "Weather",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Weather_AirlineId",
                table: "Weather",
                column: "AirlineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Weather_Airlines_AirlineId",
                table: "Weather",
                column: "AirlineId",
                principalTable: "Airlines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
