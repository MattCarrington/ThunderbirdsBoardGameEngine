using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Records;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Configuration
{
    internal sealed class ThunderbirdMachineStateRecordConfiguration : IEntityTypeConfiguration<ThunderbirdMachineStateRecord>
    {
        public void Configure(
            EntityTypeBuilder<ThunderbirdMachineStateRecord> builder)
        {
            builder.ToTable("game_machine_positions");

            builder.HasKey(machine => new
            {
                machine.GameId,
                machine.ThunderbirdCode
            });

            builder.Property(machine => machine.ThunderbirdCode)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(machine => machine.LocationCode)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
