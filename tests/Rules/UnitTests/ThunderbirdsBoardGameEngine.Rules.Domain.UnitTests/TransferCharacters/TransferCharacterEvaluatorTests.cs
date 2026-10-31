using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.TransferCharacters
{
    public class TransferCharacterEvaluatorTests
    {
        [Fact]
        public void Evaluate_WhenMachinesAreCoLocatedAndDestinationHasCapacity_ShouldReturnValidOutcome()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new CharacterCode("character-1")
                        ]),

                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants: [])
                ],

                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                ]);

            // Act
            var result = evaluator.Evaluate(input);

            // Assert
            Assert.True(result.IsPermitted);
            Assert.Null(result.RejectionReason);
        }

        [Fact]
        public void Evaluate_WhenMachinesAreInDifferentLocations_ShouldReturnRejectedOutcome()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ]),

                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-2"),
                        Occupants: [])
                ],

                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                ]);

            // Act
            var result = evaluator.Evaluate(input);

            // Assert
            Assert.False(result.IsPermitted);
            Assert.Equal(TransferCharacterRejectionReason.MachinesNotCoLocated, result.RejectionReason);
        }

        [Fact]
        public void Evaluate_WhenDestinationThunderbirdIsAtCapacity_ShouldReturnRejectedOutcome()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();
            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 1,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-2")
                        ])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                ]);

            // Act
            var result = evaluator.Evaluate(input);

            // Assert
            Assert.False(result.IsPermitted);
            Assert.Equal(TransferCharacterRejectionReason.CharacterCapacityExceeded, result.RejectionReason);
        }

        [Fact]
        public void Evaluate_WhenDestinationSameAsSource_ShouldThrowArgumentException()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants: [])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-1")),
                ]);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evaluator.Evaluate(input));
        }

        [Fact]
        public void Evaluate_ReciprocalSwap_ShouldReturnValidOutcome()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-2")
                        ])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                    new(
                        CharacterCode: new("character-2"),
                        TargetThunderbirdCode: new("thunderbird-1"))
                ]);

            // Act
            var result = evaluator.Evaluate(input);

            // Assert
            Assert.True(result.IsPermitted);
            Assert.Null(result.RejectionReason);
        }

        [Fact]
        public void Evaluate_ReciprocalSwapWithBothMachinesAlreadyAtCapacity_ShouldReturnValidOutcome()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1"),
                            new("character-3")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-2"),
                            new("character-4")
                        ])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                    new(
                        CharacterCode: new("character-2"),
                        TargetThunderbirdCode: new("thunderbird-1"))
                ]);

            // Act
            var result = evaluator.Evaluate(input);

            // Assert
            Assert.True(result.IsPermitted);
            Assert.Null(result.RejectionReason);
        }

        [Fact]
        public void Evaluate_WhenTwoAssignmentsAreNotReciprocal_ShouldReturnRejectedOutcome()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1"),
                            new("character-2")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 3,
                        CurrentLocation: new("location-1"),
                        Occupants: [])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                    new(
                        CharacterCode: new("character-2"),
                        TargetThunderbirdCode: new("thunderbird-2"))
                ]);

            // Act
            var result = evaluator.Evaluate(input);

            // Assert
            Assert.False(result.IsPermitted);
            Assert.Equal(TransferCharacterRejectionReason.NotReciprocal, result.RejectionReason);
        }

        [Fact]
        public void Evaluate_WhenReciprocalSwapNotAtSameLocation_ShouldReturnRejectedOutcome()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-2"), // Different location
                        Occupants:
                        [
                            new("character-2")
                        ])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                    new(
                        CharacterCode: new("character-2"),
                        TargetThunderbirdCode: new("thunderbird-1"))
                ]);

            // Act
            var result = evaluator.Evaluate(input);

            // Assert
            Assert.False(result.IsPermitted);
            Assert.Equal(TransferCharacterRejectionReason.MachinesNotCoLocated, result.RejectionReason);
        }

        [Fact]
        public void Evaluate_WhenReciprocalSwapResultsInNoChange_ShouldThrowArgumentException()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-2")
                        ])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-1")),
                    new(
                        CharacterCode: new("character-2"),
                        TargetThunderbirdCode: new("thunderbird-2"))
                ]);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evaluator.Evaluate(input));
        }

        [Fact]
        public void Evaluate_ReciprocalSwapWhereOneAssignmentIsNoSwap_ShouldReturnRejectedOutcome()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();
            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-2")
                        ])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                    new(
                        CharacterCode: new("character-2"),
                        TargetThunderbirdCode: new("thunderbird-2")) // Not a swap
                ]);
            // Act
            var result = evaluator.Evaluate(input);
            // Assert
            Assert.False(result.IsPermitted);
            Assert.Equal(TransferCharacterRejectionReason.NotReciprocal, result.RejectionReason);
        }

        [Fact]
        public void Evaluate_WhenTwoAssignmentsInvolveThreeMachines_ShouldRejectAsNotReciprocal()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 1,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 1,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-2")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-3"),
                        CharacterCapacity: 1,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-3")
                        ])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                    new(
                        CharacterCode: new("character-2"),
                        TargetThunderbirdCode: new("thunderbird-3"))
                ]);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evaluator.Evaluate(input));
        }

        [Fact]
        public void Evaluate_WhenDestinationThunderbirdNotFound_ShouldThrowArgumentException()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants: [])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-3")),
                ]);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evaluator.Evaluate(input));
        }

        [Fact]
        public void Evaluate_WhenOneThunderbirdIsMissing_ShouldThrowArgumentException()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                ]);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evaluator.Evaluate(input));
        }

        [Fact]
        public void Evaluate_WhenFewerThanTwoThunderbirdsAreSupplied_ShouldThrow()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                ]);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evaluator.Evaluate(input));
        }

        [Fact]
        public void Evaluate_WhenNoAssignmentsProvided_ShouldThrowArgumentException()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants: [])
                ],
                Assignments:
                []);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evaluator.Evaluate(input));
        }

        [Fact]
        public void Evaluate_WhenThreeAssignmentsAreProvided_ShouldThrowArgumentException()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants: [])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                    new(
                        CharacterCode: new("character-2"),
                        TargetThunderbirdCode: new("thunderbird-1")),
                    new(
                        CharacterCode: new("character-3"),
                        TargetThunderbirdCode: new("thunderbird-2"))
                ]);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evaluator.Evaluate(input));
        }

        [Fact]
        public void Evaluate_WhenCharacterNotFoundInAnyThunderbird_ShouldThrowArgumentException()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();
            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants: [])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-3"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                ]);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evaluator.Evaluate(input));
        }

        [Fact]
        public void Evaluate_WhenCharacterToBeTransferredAppearsInBothVehicles_ShouldThrowArgumentException()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1") // Same character in both vehicles
                        ])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                ]);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evaluator.Evaluate(input));
        }

        [Fact]
        public void Evaluate_WhenThunderbirdsProvidedHaveSameCode_ShouldThrowArgumentException()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-1"), // Duplicate code
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants: [])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-1")),
                ]);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evaluator.Evaluate(input));
        }

        [Fact]
        public void Evaluate_WhenAssignmentsProvidedHaveSameCharacter_ShouldThrowArgumentException()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants: [])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                    new(
                        CharacterCode: new("character-1"), // Duplicate character
                        TargetThunderbirdCode: new("thunderbird-1"))
                ]);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evaluator.Evaluate(input));
        }

        [Fact]
        public void Evaluate_WhenThunderbirdCapacityIsZero_ShouldThrowArgumentException()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();
            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 0, // Invalid capacity
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1")
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants: [])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                ]);
            // Act & Assert
            Assert.Throws<ArgumentException>(() => evaluator.Evaluate(input));
        }

        [Fact]
        public void Evaluate_WhenThunderbirdCapacityAlreadyExceeded_ShouldThrowArgumentException()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 1,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1"),
                            new("character-2") // Exceeds capacity
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants: [])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                ]);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evaluator.Evaluate(input));
        }

        [Fact]
        public void Evaluate_WhenNonTransferredCharacterOccupiesBothThunderbirds_ShouldThrow()
        {
            // Arrange
            var evaluator = new TransferCharacterEvaluator();

            var input = new TransferCharacterEvaluationInput(
                Thunderbirds:
                [
                    new(
                        ThunderbirdCode: new("thunderbird-1"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-1"),
                            new("character-3") // Non-transferred character
                        ]),
                    new(
                        ThunderbirdCode: new("thunderbird-2"),
                        CharacterCapacity: 2,
                        CurrentLocation: new("location-1"),
                        Occupants:
                        [
                            new("character-3") // Same non-transferred character
                        ])
                ],
                Assignments:
                [
                    new(
                        CharacterCode: new("character-1"),
                        TargetThunderbirdCode: new("thunderbird-2")),
                ]);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evaluator.Evaluate(input));
        }
    }
}
