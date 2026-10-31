using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation
{
    public class TransferCharacterEvaluator
    {
        public TransferCharacterEvaluationResult Evaluate(TransferCharacterEvaluationInput input)
        {
            if (input.Thunderbirds.Count != 2)
            {
                throw new ArgumentException(
                    "Exactly two Thunderbird Machines are required for evaluation.",
                    nameof(input));
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
                return TransferCharacterEvaluationResult.Rejected(TransferCharacterRejectionReason.MachinesNotCoLocated);
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
                throw new ArgumentException(
                    "Every proposed assignment must move a character to another Thunderbird.",
                    nameof(input));
            }

            var destinationThunderbird = GetDestinationThunderbird(input, assignment.TargetThunderbirdCode);

            if (destinationThunderbird.Occupants.Count + 1 > destinationThunderbird.CharacterCapacity)
            {
                return TransferCharacterEvaluationResult.Rejected(TransferCharacterRejectionReason.CharacterCapacityExceeded);
            }

            return TransferCharacterEvaluationResult.Permitted();
        }

        private static TransferCharacterEvaluationResult EvaluateReciprocalSwap(
    TransferCharacterEvaluationInput input)
        {
            var firstAssignment = input.Assignments.First();
            var secondAssignment = input.Assignments.Skip(1).First();

            if (firstAssignment.CharacterCode == secondAssignment.CharacterCode)
            {
                throw new ArgumentException(
                    "Reciprocal assignments must involve different characters.",
                    nameof(input));
            }

            var firstSource = GetCurrentOccupancyThunderbird(input, firstAssignment.CharacterCode);

            var secondSource = GetCurrentOccupancyThunderbird(input, secondAssignment.CharacterCode);

            var firstDestination = GetDestinationThunderbird(input, firstAssignment.TargetThunderbirdCode);

            var secondDestination = GetDestinationThunderbird(input, secondAssignment.TargetThunderbirdCode);

            var bothAssignmentsMakeNoChange = firstSource.ThunderbirdCode == firstDestination.ThunderbirdCode
                && secondSource.ThunderbirdCode == secondDestination.ThunderbirdCode;

            if (bothAssignmentsMakeNoChange)
            {
                throw new ArgumentException(
                    "Every proposed assignment must move a character to another Thunderbird.",
                    nameof(input));
            }

            var isReciprocal = firstSource.ThunderbirdCode == secondDestination.ThunderbirdCode
                && secondSource.ThunderbirdCode == firstDestination.ThunderbirdCode;

            if (!isReciprocal)
            {
                return TransferCharacterEvaluationResult.Rejected(TransferCharacterRejectionReason.NotReciprocal);
            }

            return TransferCharacterEvaluationResult.Permitted();
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
