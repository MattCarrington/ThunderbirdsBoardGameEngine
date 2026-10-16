using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGameStateVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StateVersion",
                table: "games",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StateVersion",
                table: "games");
        }
    }
}
