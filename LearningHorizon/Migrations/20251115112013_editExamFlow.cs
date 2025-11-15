using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningHorizon.Migrations
{
    /// <inheritdoc />
    public partial class editExamFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExamSubmissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    examId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false),
                    quesionId = table.Column<int>(type: "int", nullable: false),
                    answerId = table.Column<int>(type: "int", nullable: false),
                    submissionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSubmissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_ExamSubmissions_Answers_answerId",
                        column: x => x.answerId,
                        principalTable: "Answers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_ExamSubmissions_Exams_examId",
                        column: x => x.examId,
                        principalTable: "Exams",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_ExamSubmissions_Questions_quesionId",
                        column: x => x.quesionId,
                        principalTable: "Questions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_ExamSubmissions_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "UserExams",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    examId = table.Column<int>(type: "int", nullable: false),
                    currentQuestionId = table.Column<int>(type: "int", nullable: true),
                    userFinished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExams", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserExams_Exams_examId",
                        column: x => x.examId,
                        principalTable: "Exams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserExams_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubmissions_answerId",
                table: "ExamSubmissions",
                column: "answerId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubmissions_examId",
                table: "ExamSubmissions",
                column: "examId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubmissions_quesionId",
                table: "ExamSubmissions",
                column: "quesionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubmissions_userId",
                table: "ExamSubmissions",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExams_examId",
                table: "UserExams",
                column: "examId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExams_userId",
                table: "UserExams",
                column: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamSubmissions");

            migrationBuilder.DropTable(
                name: "UserExams");
        }
    }
}
