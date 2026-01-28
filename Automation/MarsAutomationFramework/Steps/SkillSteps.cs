using MarsAutomationFramework.Models;
using MarsAutomationFramework.Pages;
using MarsAutomationFramework.Utilities;
using MarsAutomationFramework.WebDriver;
using OpenQA.Selenium;
using Reqnroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAutomationFramework.Steps
{
    [Binding, Scope(Feature = "Manage Skills")]
    public class SkillSteps
    {
        private readonly IWebDriver _driver;
        private readonly SkillPage _skillsPage;

        public SkillSteps()
        {
            _driver = WebDriverMngr.Instance.GetDriver();
            _skillsPage = new SkillPage(_driver);
        }

        #region Add Skill
        [When(@"I navigate to the skill tab")]
        public void GivenINavigateToSkillTab()
        {
            _skillsPage.NavigateToSkillTab();
        }
        [When(@"I click on the Add New button")]
        public void WhenIClickOnAddNewButton() => _skillsPage.ClickAddNew(); // ensure active tab

        [When(@"I add a new skill record from file ""(.*)"" JSON index (.*)")]
        public void WhenIAddSkillRecord(string fileName, int index)
        {
            _skillsPage.AddSkill(JsonHelper.ReadJsonList<SkillModel>(fileName, "Skills")[index], isCleanupNeeded: true);
        }

        [When(@"I click on the Add button")]
        public void WhenIClickAddButton() => _skillsPage.ClickAdd();

        [Then(@"I should see the skill record from file ""(.*)"" JSON index (.*) in my profile")]
        public void ThenIShouldSeeSkillRecord(string fileName, int index)
        {
            var data = JsonHelper.ReadJsonList<SkillModel>(fileName, "Skills")[index];
            Assert.That(_skillsPage.IsSkillAdded(data.Skill!, data.Level!), Is.True,
                $"Record '{data.Skill} - {data.Level}' not visible!");
        }
        #endregion

        #region Edit Multiple
        [Given(@"I have added skill record for edit from file ""(.*)"" JSON index (.*)")]
        public void GivenIHaveAddedSkillRecordForEdit(string fileName, int index)
        {
            _skillsPage.NavigateToSkillTab();
            _skillsPage.ClickAddNew();
            _skillsPage.AddSkill(JsonHelper.ReadJsonList<SkillModel>(fileName, "Skills")[index], isCleanupNeeded: false);
            _skillsPage.ClickAdd();
        }

        [When(@"I edit the skill record from JSON index (.*) to updated JSON index (.*) file ""(.*)""")]
        public void WhenIEditSkillRecord(int oldIndex, int newIndex, string fileName)
        {
            _skillsPage.EditSkill(JsonHelper.ReadJsonList<SkillModel>(fileName, "Skills")[oldIndex], JsonHelper.ReadJsonList<SkillModel>(fileName, "Skills")[newIndex]);
        }

        [Then(@"I should see the updated skill record from file ""(.*)"" JSON index (.*) in my profile")]
        public void ThenIShouldSeeUpdatedSkill(string fileName, int index)
        {
            var data = JsonHelper.ReadJsonList<SkillModel>(fileName, "Skills")[index];
            Assert.That(_skillsPage.IsSkillAdded(data.Skill!, data.Level!), Is.True);
        }
        #endregion

        #region Delete certification 
        [Given(@"I have added skill record for delete from file ""(.*)"" JSON index (.*)")]
        public void GivenIHaveAddedSkillRecordForDelete(string fileName, int index)
        {
            _skillsPage.NavigateToSkillTab();
            _skillsPage.ClickAddNew();
            _skillsPage.AddSkill(JsonHelper.ReadJsonList<SkillModel>(fileName, "Skills")[index], isCleanupNeeded: false);
            _skillsPage.ClickAdd();
        }
        [When(@"I delete the skill record from file ""(.*)"" JSON index (.*)")]
        public void WhenIDeleteSkillRecord(string fileName, int index) => _skillsPage.DeleteSkill(JsonHelper.ReadJsonList<SkillModel>(fileName, "Skills")[index]);

        [Then(@"the skill record from file ""(.*)"" JSON index (.*) should not be listed in my skills")]
        public void ThenSkillShouldNotBeListed(string fileName, int index)
        {
            var data = JsonHelper.ReadJsonList<SkillModel>(fileName, "Skills")[index];
            Assert.That(_skillsPage.IsSkillListed(data), Is.False, $"Record '{data.Skill} - {data.Level}' should be deleted but is still listed.");
        }
        #endregion

        #region duplicate and Invalid data
        [Given(@"I have added skill record for duplication from file ""(.*)"" JSON index (.*)")]
        public void GivenIHaveAddedSkillRecordForDuplication(string fileName, int index)
        {
            _skillsPage.NavigateToSkillTab();
            _skillsPage.ClickAddNew();
            _skillsPage.AddSkill(JsonHelper.ReadJsonList<SkillModel>(fileName, "Skills")[index], isCleanupNeeded: true);
            _skillsPage.ClickAdd();
        }

        [When(@"I add a new skill record with duplicate from file ""(.*)"" JSON index (.*)")]
        public void WhenIAddSkillRecordWithDuplicate(string fileName, int index)
        {
            _skillsPage.AddSkill(JsonHelper.ReadJsonList<SkillModel>(fileName, "Skills")[index], isCleanupNeeded: false);
        }

        [Then(@"I should see an error message indicating duplicate skill not allowed")]
        public void ThenDuplicateError() =>
            _skillsPage.IsErrorMessageDisplayed("This skill record already exists.");

        [When(@"I add a new skill record with invalid data from file ""(.*)"" JSON index (.*)")]
        public void WhenIAddSkillRecordWithInvalidData(string fileName, int index)
        {
            var data = JsonHelper.ReadJsonList<SkillModel>(fileName, "Skills")[index];
            var incompleteData = new SkillModel
            {
                Skill = data.Skill,
                Level = data.Level
            };
            _skillsPage.AddSkill(incompleteData, isCleanupNeeded: false);
        }

        [Then(@"I should see an error message indicating field is required")]
        public void ThenFieldRequiredError() =>
            _skillsPage.IsErrorMessageDisplayed("Field is required.");
        #endregion
    }
}
