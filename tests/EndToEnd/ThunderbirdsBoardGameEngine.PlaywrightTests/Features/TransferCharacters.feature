Feature: TransferCharacters

	As a player,
    I want to be able to transfer characters between Thunderbirds
    So that the dashboard reflects the state of the physical game

Scenario: A commited transfer updates the game dashboard
	Given a game has been created
	And the player has opened its dashboard
	When the player transfers the single occupant from "Thunderbird 4" to "Thunderbird 2"
	Then "Gordon" should be inside "Thunderbird 2"
	And "Thunderbird 4" cannot move because it has no occupants
	And "Thunderbird 4" cannot transfer as it has no occupants
