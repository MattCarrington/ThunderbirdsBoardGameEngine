@restart
Feature: GameConnectivity

	As a player,
	I want to be able to retrieve my game after something happens on the server,
	So that I can continue playing

Scenario: A player returns after the application restarts
    Given a new game has been created
    And the player moves "Thunderbird 1" to "Europe"
    When the application is restarted
    And the player returns to the game
    Then "Thunderbird 1" should be at "Europe"

Scenario: A newly created game survives an application restart
    Given a new game has been created
    When the application is restarted
    And the player returns to the game
    Then the game should still be available
    And its Thunderbird positions should match the recorded positions
