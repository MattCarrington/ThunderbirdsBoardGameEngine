using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation
{
    public class TransferCharacterEvaluator
    {
        public TransferCharacterEvaluationResult Evaluate(TransferCharacterEvaluationInput input)
        {
            if (input.Thunderbirds.Count != 2)
            {
                throw new ArgumentException("Two thunderbirds are required for evaluation.");
            }

            foreach (var thunderbird in input.Thunderbirds)
            {
                if (thunderbird.CharacterCapacity <= 0)
                {
                    throw new ArgumentException("Every Thunderbird must have a positive character capacity.", nameof(input));
                }

                if (thunderbird.CharacterCapacity < thunderbird.Occupants.Count)
                {
                    throw new ArgumentException($"Thunderbird {thunderbird.ThunderbirdCode} capacity already exceeded. Capacity: {thunderbird.CharacterCapacity}, Occupants: {thunderbird.Occupants.Count}.");
                }
            }

            var firstThunderbird = input.Thunderbirds.First();
            var secondThunderbird = input.Thunderbirds.Skip(1).First();

            if (firstThunderbird.ThunderbirdCode == secondThunderbird.ThunderbirdCode)
            {
                throw new ArgumentException($"Thunderbirds provided must be different.");
            }

            if (input.Assignments.Count < 1 || input.Assignments.Count > 2)
            {
                throw new ArgumentException("Only one or two assignments are allowed for evaluation.");
            }

            var occupants = input.Thunderbirds
                .SelectMany(thunderbird => thunderbird.Occupants)
                .ToArray();

            if (occupants.Distinct().Count() != occupants.Length)
            {
                throw new ArgumentException(
                    "Each character must occupy exactly one Thunderbird.",
                    nameof(input));
            }

            if (firstThunderbird.CurrentLocation != secondThunderbird.CurrentLocation)
            {
                return new TransferCharacterEvaluationResult(TransferCharacterOutcome.Rejected, TransferCharacterRejectionReason.MachinesNotCoLocated); // Thunderbirds are not co-located
            }

            if (input.Assignments.Count == 1)
            {
                return EvaluateSingleTransfer(input);
            }

            return EvaluateReciprocalSwap(input);
        }

        private static TransferCharacterEvaluationResult EvaluateSingleTransfer(TransferCharacterEvaluationInput input)
        {
            var assignment = input.Assignments.Single();

            var currentOccupancy = GetCurrentOccupancyThunderbird(input, assignment.CharacterCode);

            if (assignment.TargetThunderbirdCode == currentOccupancy.ThunderbirdCode)
            {
                return new TransferCharacterEvaluationResult(TransferCharacterOutcome.NoChange); // Character is already in the target Thunderbird
            }

            var destinationThunderbird = GetDestinationThunderbird(input, assignment.TargetThunderbirdCode);

            if (destinationThunderbird.Occupants.Count + 1 > destinationThunderbird.CharacterCapacity)
            {
                // Target Thunderbird is at full capacity
                return new TransferCharacterEvaluationResult(TransferCharacterOutcome.Rejected, TransferCharacterRejectionReason.CharacterCapacityExceeded);
            }

            return new TransferCharacterEvaluationResult(TransferCharacterOutcome.Valid); // All checks passed, transfer is possible 
        }

        private static TransferCharacterEvaluationResult EvaluateReciprocalSwap(TransferCharacterEvaluationInput input)
        {
            var firstAssignment = input.Assignments.First();
            var secondAssignment = input.Assignments.Skip(1).First();

            if (firstAssignment.CharacterCode == secondAssignment.CharacterCode)
            {
                throw new ArgumentException("Reciprocal assignments must involve different characters.");
            }

            if (firstAssignment.TargetThunderbirdCode == secondAssignment.TargetThunderbirdCode)
            {
                // Both assignments target the same Thunderbird
                return new TransferCharacterEvaluationResult(TransferCharacterOutcome.Rejected, TransferCharacterRejectionReason.NotReciprocal);
            }

            var transfers = 0;

            foreach (var assignment in input.Assignments)
            {
                var destinationThunderbird = GetDestinationThunderbird(input, assignment.TargetThunderbirdCode);

                var currentOccupancy = GetCurrentOccupancyThunderbird(input, assignment.CharacterCode);

                if (assignment.TargetThunderbirdCode != currentOccupancy.ThunderbirdCode)
                {
                    transfers++;
                }
            }

            if (transfers == 0)
            {
                return new TransferCharacterEvaluationResult(TransferCharacterOutcome.NoChange); // No transfers needed, all characters are already in their target Thunderbirds
            }

            if (transfers == 1)
            {
                // Only one transfer means it is not a reciprocal swap
                return new TransferCharacterEvaluationResult(TransferCharacterOutcome.Rejected, TransferCharacterRejectionReason.NotReciprocal);
            }

            return new TransferCharacterEvaluationResult(TransferCharacterOutcome.Valid); // All checks passed, transfer is possible 
        }

        private static ThunderbirdTransferState GetDestinationThunderbird(TransferCharacterEvaluationInput input, ThunderbirdCode assignment)
        {
            return input.Thunderbirds.SingleOrDefault(t => t.ThunderbirdCode == assignment)
                ?? throw new ArgumentException($"Target Thunderbird {assignment} not found in the provided thunderbirds.");
        }

        private static ThunderbirdTransferState GetCurrentOccupancyThunderbird(TransferCharacterEvaluationInput input, CharacterCode characterCode)
        {
            var currentOccupancy = input.Thunderbirds.Where(t => t.Occupants.Contains(characterCode));

            if (!currentOccupancy.Any())
            {
                throw new ArgumentException($"Character {characterCode} not found in any of the provided thunderbirds.");
            }

            return currentOccupancy.Single();
        }
    }
}
