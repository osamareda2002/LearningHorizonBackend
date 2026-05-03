using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningHorizon.Migrations
{
    /// <inheritdoc />
    public partial class linkBooksWithCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "categoryId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_categoryId",
                table: "Books",
                column: "categoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_CourseCategories_categoryId",
                table: "Books",
                column: "categoryId",
                principalTable: "CourseCategories",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_CourseCategories_categoryId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_categoryId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "categoryId",
                table: "Books");
        }
    }
}
