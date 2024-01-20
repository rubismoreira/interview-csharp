using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortenerService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShortUrlProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortUrl",
                table: "Urls",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortUrl",
                table: "Urls");
        }
    }
}
