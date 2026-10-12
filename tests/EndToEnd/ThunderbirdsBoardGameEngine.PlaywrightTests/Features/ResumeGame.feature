Feature: ResumeGame

    As a player,
    I want to be able to resume my previous game
    So that I can continue playing where I left off

Scenario: Resume an existing game
    Given a game has been created
    And the companion application is open
    When the player resumes the game using its identifier
    Then the game dashboard should be displayed
