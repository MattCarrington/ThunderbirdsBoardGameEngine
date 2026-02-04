Feature: CalculateRescueBonus

	The rescue target is calculated based on the selected disaster
	and any applicable rescue bonuses.

Scenario: Calculate Rescue Bonus for a Disaster with character who provides a bonus
	Given the disaster "Terror in New York City" is selected
	And "Gordon" is selected as performing the rescue
	And the "Virgil" bonus is marked as being present
	And the "Firefly" bonus is marked as being present
	When the rescue target is calculated
	Then the rescue tarrget should be displayed
