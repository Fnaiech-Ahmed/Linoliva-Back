using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tech_software_engineer_consultant_int_backend.Migrations
{
    /// <inheritdoc />
    public partial class actifproduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Actif",
                table: "Products",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Actif",
                table: "Products");
        }
    }
}
