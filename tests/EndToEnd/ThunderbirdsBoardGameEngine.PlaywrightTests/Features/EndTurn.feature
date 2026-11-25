Feature: EndTurn

	As a player
	I want to be able to end the current turn
	so that the next character in the game can make actions and operations

Scenario: A turn can be ended successfully
	Given a game has been created with the following characters:
		| Character Code | 
		| john           | 
		| virgil         | 
		| scott          | 
	And the player has opened its dashboard
	And "John" is the active character
	When the player ends the turn
	Then "Virgil" should be the active character
