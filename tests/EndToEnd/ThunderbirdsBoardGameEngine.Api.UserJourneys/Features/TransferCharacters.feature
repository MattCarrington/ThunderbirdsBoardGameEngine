Feature: TransferCharacters

	As a player,
	I want to commit character transfers
	So that the companion application matches the board

Scenario: A player makes a transfer and revisits the game
	Given a new game has been created
	When the player transfers "Gordon" to "Thunderbird 2"
	Then "Gordon" should be occupying "Thunderbird 2"

Scenario: A player makes an invalid transfer
	Given a new game has been created
	When the player attempts to transfer "Virgil" to "Thunderbird 4"
	Then an error should be returned indicating that the transfer is invalid

Scenario: A transfer cannot be made as a player has moved the Thunderbird away
	Given a new game has been created
	When the player moves "Thunderbird 4" to "Asia"
	And the player attempts to transfer "Gordon" to "Thunderbird 1"
	Then an error should be returned indicating that the transfer is invalid

Scenario: A player makes a reciprocal swap and revisits the game
	Given a new game has been created
	When the player swaps "Gordon" in "Thunderbird 4" with "Virgil" in "Thunderbird 2"
	Then "Gordon" should be occupying "Thunderbird 2"
	And "Virgil" should be occupying "Thunderbird 4"

Scenario: A player moves a Thunderbird to do a transfer
	Given a new game has been created
	When the player attempts to transfer "Lady Penelope" to "Thunderbird 1"
	Then an error should be returned indicating that the transfer is invalid
	When the player moves "Thunderbird 1" to "Europe"
	And the player transfers "Lady Penelope" to "Thunderbird 1"
	Then "Lady Penelope" should be occupying "Thunderbird 1"
