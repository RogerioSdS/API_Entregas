using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIBackend.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class Migration20250327_v105 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ZipCode",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

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
            migrationBuilder.AlterColumn<int>(
                name: "ZipCode",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

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
