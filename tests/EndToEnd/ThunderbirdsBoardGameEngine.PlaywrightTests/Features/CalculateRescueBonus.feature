Feature: CalculateRescueBonus

The player can see the minimum required roll needed to resolve a disaster.

@tag1
Scenario: Calculate Rescue Bonus for a Disaster
	Given Given the "Sun Probe" disaster is selected
	And the "Scott" bonus is marked as present
	And the "Transmitter Truck" bonus is marked as present
	When the rescue target is calculated
	Then the minimum required roll should be "6"
