using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Records;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.RecordConfiguration
{
    internal sealed class GameTurnOrderEntryRecordConfiguration : IEntityTypeConfiguration<GameTurnOrderEntryRecord>
    {
        public void Configure(EntityTypeBuilder<GameTurnOrderEntryRecord> builder)
        {
            builder.ToTable("game_turn_order");

            builder.HasKey(entry => new { entry.GameId, entry.Position });

            builder.HasIndex(entry => new { entry.GameId, entry.CharacterCode })
                .IsUnique();

            builder.Property(entry => entry.CharacterCode)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasOne(entry => entry.Game)
                .WithMany(game => game.TurnOrderEntries)
                .HasForeignKey(entry => entry.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
