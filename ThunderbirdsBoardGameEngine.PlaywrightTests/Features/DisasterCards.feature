Feature: Disaster Cards

The player can see and inspect the list of disaster cards.

  Scenario: Disaster cards dropdown contains list of cards
    Given I navigate to the disaster cards page
    Then the dropdown contains a list of Disaster Cards

  Scenario: Selecting a card shows its details
    Given I navigate to the disaster cards page
	When I select the disaster card "Terror in New York City"
    Then the selected card details are displayed