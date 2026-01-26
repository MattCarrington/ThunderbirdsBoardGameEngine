Feature: CalculateRescueBonus

	The rescue target is calculated based on the selected disaster
	and any applicable rescue bonuses.

Scenario: Calculate Rescue Bonus for a Disaster with character who provides a bonus
	Given the disaster "Terror in New York City" is selected
	And "Gordon" is performing the rescue
	And the "Virgil" bonus is marked as being present
	And the "Firefly" bonus is marked as being present
	When the rescue target is calculated
	Then the minimum required roll should be 3

Scenario: Calculate Rescue Bonus for a Disaster with character who does not provide a bonus
	Given the disaster "Terror in New York City" is selected
	And "Alan" is performing the rescue
	And the "Virgil" bonus is marked as being present
	And the "Firefly" bonus is marked as being present
	When the rescue target is calculated
	Then the minimum required roll should be 6
