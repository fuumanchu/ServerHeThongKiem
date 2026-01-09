using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerHeThongKiem.Migrations
{
    /// <inheritdoc />
    public partial class AddLastSeenToDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastSeen",
                table: "Devices",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSeen",
                table: "Devices");
        }
    }
}
