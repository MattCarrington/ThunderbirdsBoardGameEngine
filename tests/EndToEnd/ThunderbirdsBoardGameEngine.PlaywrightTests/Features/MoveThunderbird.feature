Feature: MoveThunderbird

    As a player,
    I want to be able to move Thunderbirds
    So that the dashboard reflects the state of the physical game

Scenario: A committed movement updates the game dashboard
    Given a game has been created
    And the player has opened its dashboard
    When the player moves "Thunderbird 1" to "South America"
    Then "Thunderbird 1" should be in "South America"

Scenario: A committed movement survives a browser reload
    Given a game has been created
    And the player has opened its dashboard
    When the player moves "Thunderbird 1" to "South America"
    And reloads the dashboard
    Then "Thunderbird 1" should be in "South America"
