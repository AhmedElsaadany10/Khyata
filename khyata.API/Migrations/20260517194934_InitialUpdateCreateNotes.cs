using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace khyata.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialUpdateCreateNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExtraNotes",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraNotes",
                table: "Orders");
        }
    }
}
