using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechTalkBot.Migrations
{
    /// <inheritdoc />
    public partial class unique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Videos_Url_Name",
                table: "Videos",
                columns: new[] { "Url", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Videos_Url_Name",
                table: "Videos");
        }
    }
}
