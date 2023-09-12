using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADO_EF.Migrations
{
    /// <inheritdoc />
    public partial class NavChiefDep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleteDt",
                table: "Departments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDt",
                table: "Departments",
                type: "datetime2",
                nullable: true);
        }
    }
}
