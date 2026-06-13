using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentIdToIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StudentId",
                table: "Issues",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Issues_StudentId",
                table: "Issues",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Students_StudentId",
                table: "Issues",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Students_StudentId",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_StudentId",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Issues");
        }
    }
}
