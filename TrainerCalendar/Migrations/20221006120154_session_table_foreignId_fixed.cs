using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainerCalendar.Migrations
{
    public partial class session_table_foreignId_fixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouresId",
                table: "Sessions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CouresId",
                table: "Sessions",
                type: "int",
                nullable: true);
        }
    }
}
