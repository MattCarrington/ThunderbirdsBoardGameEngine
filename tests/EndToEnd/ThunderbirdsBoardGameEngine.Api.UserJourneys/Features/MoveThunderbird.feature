Feature: MoveThunderbird

	As a player
    I want to explore and commit Thunderbird movements
    So that the companion application matches the board

Scenario: A player moves a Thunderbird and revisits the game
    Given a new game has been created
    When the player moves "Thunderbird 3" to "The Sun"
    Then "Thunderbird 3" should be at "The Sun"

Scenario: A player makes an invalid move
	Given a new game has been created
	When the player attempts to move "Thunderbird 2" to "The Moon"
	Then an error should be returned indicating that the move is invalid