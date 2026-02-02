using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningHorizon.Migrations
{
    /// <inheritdoc />
    public partial class InstructorsFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Instructors",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    specialty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    expertise = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    imageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    facebookUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    whatsappUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    instgramUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructors", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Instructors");
        }
    }
}
