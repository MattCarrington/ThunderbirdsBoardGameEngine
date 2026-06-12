Feature: ValidateMovement

	The movement is validated based on the selected Thunderbird and the chosen start and destination locations.
	Vehicles can only travel certain routes, so not all routes will be valid.

@tag1
Scenario: Player can validate movement route is valid
	Given "Thunderbird 4" is selected
	And "Europe" is selected as the start location
	And "South Pacific" is selected as the destination
	When the movement is validated
	Then the validation success result should be displayed

Scenario: Player can validate movement route is invalid
	Given "Thunderbird 2" is selected
	And "The Sun" is selected as the start location
	And "The Moon" is selected as the destination
	When the movement is validated
	Then the validation failure result should be displayed
