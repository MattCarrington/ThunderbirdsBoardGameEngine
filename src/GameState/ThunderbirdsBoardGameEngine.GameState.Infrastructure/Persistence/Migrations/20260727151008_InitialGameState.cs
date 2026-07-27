using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialGameState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SetupVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "game_character_assignments",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ThunderbirdCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_character_assignments", x => new { x.GameId, x.CharacterCode });
                    table.ForeignKey(
                        name: "FK_game_character_assignments_games_GameId",
                        column: x => x.GameId,
                        principalTable: "games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_machine_positions",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    ThunderbirdCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LocationCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_machine_positions", x => new { x.GameId, x.ThunderbirdCode });
                    table.ForeignKey(
                        name: "FK_game_machine_positions_games_GameId",
                        column: x => x.GameId,
                        principalTable: "games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_character_assignments");

            migrationBuilder.DropTable(
                name: "game_machine_positions");

            migrationBuilder.DropTable(
                name: "games");
        }
    }
}
