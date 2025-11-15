using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningHorizon.Migrations
{
    /// <inheritdoc />
    public partial class addMarksToQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_Question_questionId",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_Question_Exams_examId",
                table: "Question");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Question",
                table: "Question");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answer",
                table: "Answer");

            migrationBuilder.RenameTable(
                name: "Question",
                newName: "Questions");

            migrationBuilder.RenameTable(
                name: "Answer",
                newName: "Answers");

            migrationBuilder.RenameIndex(
                name: "IX_Question_examId",
                table: "Questions",
                newName: "IX_Questions_examId");

            migrationBuilder.RenameIndex(
                name: "IX_Answer_questionId",
                table: "Answers",
                newName: "IX_Answers_questionId");

            migrationBuilder.AddColumn<double>(
                name: "mark",
                table: "Questions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Questions",
                table: "Questions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answers",
                table: "Answers",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Questions_questionId",
                table: "Answers",
                column: "questionId",
                principalTable: "Questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Exams_examId",
                table: "Questions",
                column: "examId",
                principalTable: "Exams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Questions_questionId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Exams_examId",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Questions",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answers",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "mark",
                table: "Questions");

            migrationBuilder.RenameTable(
                name: "Questions",
                newName: "Question");

            migrationBuilder.RenameTable(
                name: "Answers",
                newName: "Answer");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_examId",
                table: "Question",
                newName: "IX_Question_examId");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_questionId",
                table: "Answer",
                newName: "IX_Answer_questionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Question",
                table: "Question",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answer",
                table: "Answer",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_Question_questionId",
                table: "Answer",
                column: "questionId",
                principalTable: "Question",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Exams_examId",
                table: "Question",
                column: "examId",
                principalTable: "Exams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
