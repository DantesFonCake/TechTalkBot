using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TechTalkBot.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActivePollId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Polls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    WinnerName = table.Column<string>(type: "text", nullable: true),
                    WinnerUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Polls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    WasInPoll = table.Column<bool>(type: "boolean", nullable: false),
                    Watched = table.Column<bool>(type: "boolean", nullable: false),
                    PollId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => new { x.Name, x.Url });
                    table.ForeignKey(
                        name: "FK_Videos_Polls_PollId",
                        column: x => x.PollId,
                        principalTable: "Polls",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ActivePollId",
                table: "Chats",
                column: "ActivePollId");

            migrationBuilder.CreateIndex(
                name: "IX_Polls_WinnerName_WinnerUrl",
                table: "Polls",
                columns: new[] { "WinnerName", "WinnerUrl" });

            migrationBuilder.CreateIndex(
                name: "IX_Videos_PollId",
                table: "Videos",
                column: "PollId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Polls_ActivePollId",
                table: "Chats",
                column: "ActivePollId",
                principalTable: "Polls",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Polls_Videos_WinnerName_WinnerUrl",
                table: "Polls",
                columns: new[] { "WinnerName", "WinnerUrl" },
                principalTable: "Videos",
                principalColumns: new[] { "Name", "Url" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Polls_PollId",
                table: "Videos");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "Polls");

            migrationBuilder.DropTable(
                name: "Videos");
        }
    }
}
