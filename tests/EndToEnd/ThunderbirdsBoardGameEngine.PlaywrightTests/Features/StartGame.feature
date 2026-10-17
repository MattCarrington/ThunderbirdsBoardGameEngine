Feature: StartGame

	As a player,
	I want to be able to start a new game
	So that the game can be tracked

Scenario: Start a new game
    Given the companion application is open
    When the player starts a new game
    Then the game dashboard should be displayed
