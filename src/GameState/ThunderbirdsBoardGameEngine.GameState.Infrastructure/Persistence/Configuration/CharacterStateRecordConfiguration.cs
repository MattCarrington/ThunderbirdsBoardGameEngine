using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Records;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Configuration
{
    internal sealed class CharacterStateRecordConfiguration : IEntityTypeConfiguration<CharacterStateRecord>
    {
        public void Configure(
            EntityTypeBuilder<CharacterStateRecord> builder)
        {
            builder.ToTable("game_character_assignments");

            builder.HasKey(character => new
            {
                character.GameId,
                character.CharacterCode
            });

            builder.Property(character => character.CharacterCode)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(character => character.ThunderbirdCode)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
