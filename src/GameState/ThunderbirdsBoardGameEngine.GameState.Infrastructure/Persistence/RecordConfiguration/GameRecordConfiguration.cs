using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Records;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.RecordConfiguration
{
    internal class GameRecordConfiguration : IEntityTypeConfiguration<GameRecord>
    {
        public void Configure(EntityTypeBuilder<GameRecord> builder)
        {
            builder.ToTable("games");

            builder.HasKey(game => game.Id);

            builder.Property(game => game.Id)
                .ValueGeneratedNever();

            builder.Property(game => game.CreatedAtUtc)
                .IsRequired();

            builder.Property(game => game.SetupVersion)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasMany(game => game.ThunderbirdMachineStates)
                .WithOne(machine => machine.Game)
                .HasForeignKey(machine => machine.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(game => game.CharacterStates)
                .WithOne(character => character.Game)
                .HasForeignKey(character => character.GameId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
