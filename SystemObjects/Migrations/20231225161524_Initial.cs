using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemObjects.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EvaluationCriteria",
                columns: table => new
                {
                    CriId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CriDesc = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationCriteria", x => x.CriId);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    SerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SerDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.SerId);
                });

            migrationBuilder.CreateTable(
                name: "SystemInformation",
                columns: table => new
                {
                    SysId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SysName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Logo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Zalo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hotline = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemInformation", x => x.SysId);
                });

            migrationBuilder.CreateTable(
                name: "SystemLog",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemLog", x => x.LogId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationCriteria_CriDesc",
                table: "EvaluationCriteria",
                column: "CriDesc",
                unique: true,
                filter: "[CriDesc] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Service_SerName",
                table: "Service",
                column: "SerName",
                unique: true,
                filter: "[SerName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EvaluationCriteria");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "SystemInformation");

            migrationBuilder.DropTable(
                name: "SystemLog");
        }
    }
}
