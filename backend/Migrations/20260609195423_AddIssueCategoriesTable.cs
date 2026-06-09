using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddIssueCategoriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IssueCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentCategory = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsWomenWelfare = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueCategories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "IssueCategories",
                columns: new[] { "Id", "IsActive", "IsWomenWelfare", "Name", "ParentCategory" },
                values: new object[,]
                {
                    { 1, true, false, "Electrical", "Infrastructure" },
                    { 2, true, false, "Plumbing", "Infrastructure" },
                    { 3, true, false, "Internet", "Infrastructure" },
                    { 4, true, false, "Furniture", "Infrastructure" },
                    { 5, true, false, "Cleaning", "Infrastructure" },
                    { 6, true, false, "Security", "Infrastructure" },
                    { 7, true, false, "Water Supply", "Infrastructure" },
                    { 8, true, false, "Room Maintenance", "Hostel" },
                    { 9, true, false, "Cleanliness", "Hostel" },
                    { 10, true, false, "Hostel Facilities", "Hostel" },
                    { 11, true, false, "Classroom Issue", "Academic" },
                    { 12, true, false, "Faculty Issue", "Academic" },
                    { 13, true, false, "Academic Facility Issue", "Academic" },
                    { 14, true, false, "Security Concern", "CampusSafety" },
                    { 15, true, false, "Unsafe Area", "CampusSafety" },
                    { 16, true, false, "Broken CCTV", "CampusSafety" },
                    { 17, true, false, "Poor Lighting", "CampusSafety" },
                    { 18, true, false, "Ragging", "StudentWelfare" },
                    { 19, true, false, "Bullying", "StudentWelfare" },
                    { 20, true, false, "Discrimination", "StudentWelfare" },
                    { 21, true, false, "Mental Wellbeing", "StudentWelfare" },
                    { 22, true, false, "Counseling Request", "StudentWelfare" },
                    { 23, true, true, "Harassment", "WomenWelfare" },
                    { 24, true, true, "Safety & Privacy Concern", "WomenWelfare" },
                    { 25, true, true, "Complaint Against Student", "WomenWelfare" },
                    { 26, true, true, "Complaint Against Faculty", "WomenWelfare" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_IssueCategories_ParentCategory",
                table: "IssueCategories",
                column: "ParentCategory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssueCategories");
        }
    }
}
