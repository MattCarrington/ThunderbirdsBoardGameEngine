Feature: MoveThunderbird

	As a player
    I want to explore and commit Thunderbird movements
    So that the companion application matches the board

@tag1
Scenario: A player moves a Thunderbird and revisits the game
    Given a new game has been created
    When the player moves "Thunderbird 3" to "The Sun"
    Then "Thunderbird 3" should be at "The Sun"
