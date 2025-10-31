using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Co_owner_Vehicle.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForRepositoryRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EContracts",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EContracts");
        }
    }
}
