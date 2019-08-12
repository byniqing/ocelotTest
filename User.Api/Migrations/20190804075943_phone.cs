using Microsoft.EntityFrameworkCore.Migrations;

namespace User.Api.Migrations
{
    public partial class phone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "phone",
                table: "Users");
        }
    }
}
