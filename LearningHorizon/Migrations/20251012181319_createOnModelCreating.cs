using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningHorizon.Migrations
{
    /// <inheritdoc />
    public partial class createOnModelCreating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseUser_Courses_CoursescourseId",
                table: "CourseUser");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseUser_Users_Usersid",
                table: "CourseUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseUser",
                table: "CourseUser");

            migrationBuilder.RenameTable(
                name: "CourseUser",
                newName: "UserCoursesShowed");

            migrationBuilder.RenameColumn(
                name: "courseTitle",
                table: "Courses",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "coursePrice",
                table: "Courses",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "coursePath",
                table: "Courses",
                newName: "path");

            migrationBuilder.RenameColumn(
                name: "courseImagePath",
                table: "Courses",
                newName: "imagePath");

            migrationBuilder.RenameColumn(
                name: "courseCreator",
                table: "Courses",
                newName: "creator");

            migrationBuilder.RenameColumn(
                name: "courseId",
                table: "Courses",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Usersid",
                table: "UserCoursesShowed",
                newName: "UsersShowedid");

            migrationBuilder.RenameColumn(
                name: "CoursescourseId",
                table: "UserCoursesShowed",
                newName: "CoursesShowedid");

            migrationBuilder.RenameIndex(
                name: "IX_CourseUser_Usersid",
                table: "UserCoursesShowed",
                newName: "IX_UserCoursesShowed_UsersShowedid");

            migrationBuilder.AddColumn<bool>(
                name: "isAdmin",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Courses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCoursesShowed",
                table: "UserCoursesShowed",
                columns: new[] { "CoursesShowedid", "UsersShowedid" });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isFree = table.Column<bool>(type: "bit", nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false),
                    courseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.id);
                    table.ForeignKey(
                        name: "FK_Lessons_Courses_courseId",
                        column: x => x.courseId,
                        principalTable: "Courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sliders",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sliders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Suggests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suggests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserCoursesPurchased",
                columns: table => new
                {
                    CoursesPurchasedid = table.Column<int>(type: "int", nullable: false),
                    UsersPurchasedid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCoursesPurchased", x => new { x.CoursesPurchasedid, x.UsersPurchasedid });
                    table.ForeignKey(
                        name: "FK_UserCoursesPurchased_Courses_CoursesPurchasedid",
                        column: x => x.CoursesPurchasedid,
                        principalTable: "Courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCoursesPurchased_Users_UsersPurchasedid",
                        column: x => x.UsersPurchasedid,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_courseId",
                table: "Lessons",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCoursesPurchased_UsersPurchasedid",
                table: "UserCoursesPurchased",
                column: "UsersPurchasedid");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCoursesShowed_Courses_CoursesShowedid",
                table: "UserCoursesShowed",
                column: "CoursesShowedid",
                principalTable: "Courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCoursesShowed_Users_UsersShowedid",
                table: "UserCoursesShowed",
                column: "UsersShowedid",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCoursesShowed_Courses_CoursesShowedid",
                table: "UserCoursesShowed");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCoursesShowed_Users_UsersShowedid",
                table: "UserCoursesShowed");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Sliders");

            migrationBuilder.DropTable(
                name: "Suggests");

            migrationBuilder.DropTable(
                name: "UserCoursesPurchased");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCoursesShowed",
                table: "UserCoursesShowed");

            migrationBuilder.DropColumn(
                name: "isAdmin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Courses");

            migrationBuilder.RenameTable(
                name: "UserCoursesShowed",
                newName: "CourseUser");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Courses",
                newName: "courseTitle");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "Courses",
                newName: "coursePrice");

            migrationBuilder.RenameColumn(
                name: "path",
                table: "Courses",
                newName: "coursePath");

            migrationBuilder.RenameColumn(
                name: "imagePath",
                table: "Courses",
                newName: "courseImagePath");

            migrationBuilder.RenameColumn(
                name: "creator",
                table: "Courses",
                newName: "courseCreator");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Courses",
                newName: "courseId");

            migrationBuilder.RenameColumn(
                name: "UsersShowedid",
                table: "CourseUser",
                newName: "Usersid");

            migrationBuilder.RenameColumn(
                name: "CoursesShowedid",
                table: "CourseUser",
                newName: "CoursescourseId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCoursesShowed_UsersShowedid",
                table: "CourseUser",
                newName: "IX_CourseUser_Usersid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseUser",
                table: "CourseUser",
                columns: new[] { "CoursescourseId", "Usersid" });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseUser_Courses_CoursescourseId",
                table: "CourseUser",
                column: "CoursescourseId",
                principalTable: "Courses",
                principalColumn: "courseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseUser_Users_Usersid",
                table: "CourseUser",
                column: "Usersid",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
