using Microsoft.EntityFrameworkCore.Migrations;

namespace Promotion.Migrations
{
    public partial class AddedPasswordToParticipant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "participants",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "participants");
        }
    }
}
