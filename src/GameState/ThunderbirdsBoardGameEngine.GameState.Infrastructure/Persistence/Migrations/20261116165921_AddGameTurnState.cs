using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGameTurnState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveTurnIndex",
                schema: "game_state",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "game_turn_order",
                schema: "game_state",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    CharacterCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_turn_order", x => new { x.GameId, x.Position });
                    table.ForeignKey(
                        name: "FK_game_turn_order_games_GameId",
                        column: x => x.GameId,
                        principalSchema: "game_state",
                        principalTable: "games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_game_turn_order_GameId_CharacterCode",
                schema: "game_state",
                table: "game_turn_order",
                columns: new[] { "GameId", "CharacterCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_turn_order",
                schema: "game_state");

            migrationBuilder.DropColumn(
                name: "ActiveTurnIndex",
                schema: "game_state",
                table: "games");
        }
    }
}
