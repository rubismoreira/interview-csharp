using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortenerService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ResetDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Urls_ShortUrl",
                table: "Urls");

            migrationBuilder.DropColumn(
                name: "ShortUrl",
                table: "Urls");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortUrl",
                table: "Urls",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Urls_ShortUrl",
                table: "Urls",
                column: "ShortUrl",
                unique: true);
        }
    }
}
