using Microsoft.EntityFrameworkCore.Migrations;

namespace SoilReaderPanel.Migrations
{
    public partial class UpdateToDeviceEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "data",
                table: "DeviceEvent",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data",
                table: "DeviceEvent");
        }
    }
}
