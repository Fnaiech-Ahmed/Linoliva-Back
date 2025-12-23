using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tech_software_engineer_consultant_int_backend.Migrations
{
    /// <inheritdoc />
    public partial class AjoutTableProduit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RapportsJournalier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RapportsJournalier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionsFinancières",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTransaction = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Montant = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RapportJournalierId = table.Column<int>(type: "int", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PosteRecetteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionsFinancières", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionsFinancières_RapportsJournalier_RapportJournalierId",
                        column: x => x.RapportJournalierId,
                        principalTable: "RapportsJournalier",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionsFinancières_RapportJournalierId",
                table: "TransactionsFinancières",
                column: "RapportJournalierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionsFinancières");

            migrationBuilder.DropTable(
                name: "RapportsJournalier");
        }
    }
}
