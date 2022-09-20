using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainerCalendar.Migrations
{
    public partial class UserIdUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_AspNetUsers_UserId1",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Trainers_UserId1",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Trainers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Trainers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_UserId",
                table: "Trainers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_AspNetUsers_UserId",
                table: "Trainers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_AspNetUsers_UserId",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Trainers_UserId",
                table: "Trainers");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Trainers",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Trainers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_UserId1",
                table: "Trainers",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_AspNetUsers_UserId1",
                table: "Trainers",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
