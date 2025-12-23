using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tech_software_engineer_consultant_int_backend.Migrations
{
    /// <inheritdoc />
    public partial class AjoutTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedToEmail",
                table: "CodeActivations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedToPhone",
                table: "CodeActivations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Licence_UserId",
                table: "CodeActivations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "CodeActivations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActivationDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeActivationId = table.Column<int>(type: "int", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OS = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AppVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUsage = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivationDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivationDevices_CodeActivations_CodeActivationId",
                        column: x => x.CodeActivationId,
                        principalTable: "CodeActivations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CodeActivations_Licence_UserId",
                table: "CodeActivations",
                column: "Licence_UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CodeActivations_UserId",
                table: "CodeActivations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivationDevices_CodeActivationId",
                table: "ActivationDevices",
                column: "CodeActivationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CodeActivations_Users_Licence_UserId",
                table: "CodeActivations",
                column: "Licence_UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CodeActivations_Users_UserId",
                table: "CodeActivations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CodeActivations_Users_Licence_UserId",
                table: "CodeActivations");

            migrationBuilder.DropForeignKey(
                name: "FK_CodeActivations_Users_UserId",
                table: "CodeActivations");

            migrationBuilder.DropTable(
                name: "ActivationDevices");

            migrationBuilder.DropIndex(
                name: "IX_CodeActivations_Licence_UserId",
                table: "CodeActivations");

            migrationBuilder.DropIndex(
                name: "IX_CodeActivations_UserId",
                table: "CodeActivations");

            migrationBuilder.DropColumn(
                name: "AssignedToEmail",
                table: "CodeActivations");

            migrationBuilder.DropColumn(
                name: "AssignedToPhone",
                table: "CodeActivations");

            migrationBuilder.DropColumn(
                name: "Licence_UserId",
                table: "CodeActivations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CodeActivations");
        }
    }
}
