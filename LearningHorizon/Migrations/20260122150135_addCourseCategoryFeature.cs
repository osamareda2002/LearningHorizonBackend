using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningHorizon.Migrations
{
    /// <inheritdoc />
    public partial class addCourseCategoryFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "categoryId",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourseCategories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCategories", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_categoryId",
                table: "Courses",
                column: "categoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CourseCategories_categoryId",
                table: "Courses",
                column: "categoryId",
                principalTable: "CourseCategories",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CourseCategories_categoryId",
                table: "Courses");

            migrationBuilder.DropTable(
                name: "CourseCategories");

            migrationBuilder.DropIndex(
                name: "IX_Courses_categoryId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "categoryId",
                table: "Courses");
        }
    }
}
