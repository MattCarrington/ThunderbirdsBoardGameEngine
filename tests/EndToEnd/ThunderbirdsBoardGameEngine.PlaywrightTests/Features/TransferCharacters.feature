Feature: TransferCharacters

	As a player,
    I want to be able to transfer characters between Thunderbirds
    So that the dashboard reflects the state of the physical game

Scenario: A committed transfer updates the game dashboard
	Given a game has been created
	And the player has opened its dashboard
	When the player transfers the single occupant "Gordon" from "Thunderbird 4" to "Thunderbird 2"
	Then "Gordon" should be aboard "Thunderbird 2"
	And "Thunderbird 4" cannot move because it has no occupants
	And "Thunderbird 4" cannot transfer as it has no occupants

Scenario: A committed transfer survives a browser reload
	Given a game has been created
	And the player has opened its dashboard
	When the player transfers the single occupant "Alan" from "Thunderbird 3" to "Thunderbird 2"
	And reloads the dashboard
	Then "Alan" should be aboard "Thunderbird 2"
	And "Thunderbird 3" cannot move because it has no occupants
	And "Thunderbird 3" cannot transfer as it has no occupants