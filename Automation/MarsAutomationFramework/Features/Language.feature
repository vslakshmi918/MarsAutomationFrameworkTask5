Feature: Manage Languages
  A user can add, edit, or delete languages after logging in in a sequential flow.

  @Add
  @Positive
  Scenario Outline: Add a single language record
    When I navigate to the language tab
    And I click on the Add New button
    And I add a new language record from file "Add_TC01.json" JSON index 0
    And I click on the Add button
    Then I should see the language record from file "Add_TC01.json" JSON index 0 in my profile

  @Add
  @Positive
  Scenario Outline: Add multiple languages using JSON data
    When I navigate to the language tab
    And I click on the Add New button
    And I add a new language record from file "ADD_Multi_TC02.json" JSON index <index>
    And I click on the Add button
    Then I should see the language record from file "ADD_Multi_TC02.json" JSON index <index> in my profile
    Examples:
      | index |
      | 0 |
      | 1 |
      | 2 |

  @Edit
  @Positive
  Scenario Outline: Edit multiple languages using JSON data
    # Setup: Add the language first
    Given I have added language record for edit from file "Edit_Multi_TC03.json" JSON index <index>
    # Step 2: Edit the record using updated JSON data
    When I edit the language record from JSON index <index> to updated JSON index <updateIndex> file "Edit_Multi_TC03.json"
    Then I should see the updated language record from file "Edit_Multi_TC03.json" JSON index <updateIndex> in my profile
    Examples:
      | index | updateIndex |
      | 0 | 2 |
      | 1 | 3 |

  @Delete
  @Positive
  Scenario Outline: Delete multiple languages using JSON data
    # Setup: Add the language first
    Given I have added Language record for delete from file "Delete_Multi_TC04.json" JSON index <index>
    # Step 2: Delete the record using updated JSON data
    When I delete the language record from file "Delete_Multi_TC04.json" JSON index <index>
    Then the language record from file "Delete_Multi_TC04.json" JSON index <index> should not be listed in my languages
    Examples:
      | index |
      | 0 |
      | 1 |
      | 2 |
      | 3 |

  @Duplicate
  @Negative
  Scenario: Add duplicate language
    # Setup: Add the language first
    Given I have added language record for duplication from file "Duplicate_TC05.json" JSON index 0
    # Test: Try to add duplicate
    When I click on the Add New button
    And I add a new language record with duplicate from file "Duplicate_TC05.json" JSON index 0
    And I click on the Add button
    Then I should see an error message indicating duplicate language not allowed

  @Invalid
  @Negative
  Scenario: Add language invalid data
    When I navigate to the language tab
    And I click on the Add New button
    And I add a new language record with invalid data from file "Invalid_TC06.json" JSON index 0
    And I click on the Add button
    Then I should see an error message indicating field is required
