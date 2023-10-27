using Microsoft.EntityFrameworkCore.Migrations;

namespace RMS.Migrations
{
    public partial class exeatlocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExeatLocation",
                table: "ExeatRecords",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExeatLocation",
                table: "ExeatRecords");
        }
    }
}
