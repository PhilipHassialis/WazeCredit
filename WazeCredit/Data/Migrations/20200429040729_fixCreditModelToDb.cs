using Microsoft.EntityFrameworkCore.Migrations;

namespace WazeCredit.Data.Migrations
{
    public partial class fixCreditModelToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adderss",
                table: "CreditApplicationModel");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "CreditApplicationModel",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "CreditApplicationModel");

            migrationBuilder.AddColumn<string>(
                name: "Adderss",
                table: "CreditApplicationModel",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
