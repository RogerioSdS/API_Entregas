using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIBackend.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class migration_25072025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_AspNetUsers_ResponsibleId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_ResponsibleId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ResponsibleId",
                table: "Students");

            migrationBuilder.CreateTable(
                name: "StudentResponsible",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false),
                    ResponsibleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentResponsible", x => new { x.StudentId, x.ResponsibleId });
                    table.ForeignKey(
                        name: "FK_StudentResponsible_AspNetUsers_ResponsibleId",
                        column: x => x.ResponsibleId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentResponsible_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentResponsible_ResponsibleId",
                table: "StudentResponsible",
                column: "ResponsibleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentResponsible");

            migrationBuilder.AddColumn<int>(
                name: "ResponsibleId",
                table: "Students",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_ResponsibleId",
                table: "Students",
                column: "ResponsibleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AspNetUsers_ResponsibleId",
                table: "Students",
                column: "ResponsibleId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
