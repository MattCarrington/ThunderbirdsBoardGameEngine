using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveGameStateTablesToDedicatedSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "game_state");

            migrationBuilder.RenameTable(
                name: "games",
                newName: "games",
                newSchema: "game_state");

            migrationBuilder.RenameTable(
                name: "game_machine_positions",
                newName: "game_machine_positions",
                newSchema: "game_state");

            migrationBuilder.RenameTable(
                name: "game_character_assignments",
                newName: "game_character_assignments",
                newSchema: "game_state");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "games",
                schema: "game_state",
                newName: "games");

            migrationBuilder.RenameTable(
                name: "game_machine_positions",
                schema: "game_state",
                newName: "game_machine_positions");

            migrationBuilder.RenameTable(
                name: "game_character_assignments",
                schema: "game_state",
                newName: "game_character_assignments");
        }
    }
}
