Feature: CalculateRescueBonus

	The rescue target is calculated based on the selected disaster
	and the player’s selected inputs.

Scenario: Player can calculate a rescue target
	Given the disaster "Terror in New York City" is selected
	And the player selects the character "Gordon" 
	And the player marks "Virgil" as being present
	And the player marks "Firefly" as being present
	When the rescue target is calculated
	Then the rescue target should be displayed
