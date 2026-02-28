namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation
{
    public sealed class SnapshotValidator
    {
        public void Validate(CompilationContext context)
        {
            foreach (var disaster in context.Disasters)
            {
                if (string.IsNullOrWhiteSpace(disaster.Name))
                {
                    throw new Exception("Disaster name cannot be empty.");
                }

                if (disaster.DifficultyNumber <= 0)
                {
                    throw new Exception($"Invalid difficulty for {disaster.Name}");
                }

                if (!disaster.Bonuses.Any())
                {
                    throw new Exception($"{disaster.Name} must have at least one bonus.");
                }

                if (!disaster.Rewards.Any())
                {
                    throw new Exception($"{disaster.Name} must have at least one reward.");
                }
            }

            EnsureUniqueDisasterNames(context);
        }

        private static void EnsureUniqueDisasterNames(CompilationContext context)
        {
            var duplicates = context.Disasters
                .GroupBy(d => d.Name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Any())
            {
                throw new Exception($"Duplicate disasters found: {string.Join(", ", duplicates)}");
            }
        }
    }
}
