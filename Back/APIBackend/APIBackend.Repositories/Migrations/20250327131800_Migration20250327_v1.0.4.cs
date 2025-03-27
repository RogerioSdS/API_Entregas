using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIBackend.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class Migration20250327_v104 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "AssignmentDate",
                table: "AspNetUserRoles",
                type: "TEXT",
                nullable: true,
                defaultValueSql: "STRFTIME('%Y-%m-%d %H:%M:%S', 'now')",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true,
                oldDefaultValueSql: "GETDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "AssignmentDate",
                table: "AspNetUserRoles",
                type: "TEXT",
                nullable: true,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true,
                oldDefaultValueSql: "STRFTIME('%Y-%m-%d %H:%M:%S', 'now')");
        }
    }
}
