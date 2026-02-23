using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningHorizon.Migrations
{
    /// <inheritdoc />
    public partial class makeIntegrationWithBunny : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "guid",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "libraryId",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "guid",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "libraryId",
                table: "Lessons");
        }
    }
}
