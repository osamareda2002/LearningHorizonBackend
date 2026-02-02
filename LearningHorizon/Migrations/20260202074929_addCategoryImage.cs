using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningHorizon.Migrations
{
    /// <inheritdoc />
    public partial class addCategoryImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "about",
                table: "CourseCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "imageUrl",
                table: "CourseCategories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "about",
                table: "CourseCategories");

            migrationBuilder.DropColumn(
                name: "imageUrl",
                table: "CourseCategories");
        }
    }
}
