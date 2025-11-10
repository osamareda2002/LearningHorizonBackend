using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningHorizon.Migrations
{
    /// <inheritdoc />
    public partial class addMeetingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Meetings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    meetingId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    topic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    startTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    durationInMinutes = table.Column<int>(type: "int", nullable: false),
                    hostId = table.Column<int>(type: "int", nullable: false),
                    hostEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    startUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    joinUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    passCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    numericPassword = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meetings", x => x.id);
                    table.ForeignKey(
                        name: "FK_Meetings_Users_hostId",
                        column: x => x.hostId,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "UserMeetings",
                columns: table => new
                {
                    participatedMeetingsid = table.Column<int>(type: "int", nullable: false),
                    participatesid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMeetings", x => new { x.participatedMeetingsid, x.participatesid });
                    table.ForeignKey(
                        name: "FK_UserMeetings_Meetings_participatedMeetingsid",
                        column: x => x.participatedMeetingsid,
                        principalTable: "Meetings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMeetings_Users_participatesid",
                        column: x => x.participatesid,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_hostId",
                table: "Meetings",
                column: "hostId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMeetings_participatesid",
                table: "UserMeetings",
                column: "participatesid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMeetings");

            migrationBuilder.DropTable(
                name: "Meetings");
        }
    }
}
