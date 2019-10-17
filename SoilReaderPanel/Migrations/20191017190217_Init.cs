using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SoilReaderPanel.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    DeviceID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticleDeviceID = table.Column<string>(nullable: false),
                    DeviceName = table.Column<string>(nullable: false),
                    ImageLocation = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.DeviceID);
                });

            migrationBuilder.CreateTable(
                name: "DeviceEvent",
                columns: table => new
                {
                    eventId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    eventType = table.Column<string>(nullable: true),
                    TIMESTAMP = table.Column<DateTime>(nullable: false),
                    DeviceID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceEvent", x => x.eventId);
                    table.ForeignKey(
                        name: "FK_DeviceEvent_Device_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Device",
                        principalColumn: "DeviceID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceEvent_DeviceID",
                table: "DeviceEvent",
                column: "DeviceID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceEvent");

            migrationBuilder.DropTable(
                name: "Device");
        }
    }
}
