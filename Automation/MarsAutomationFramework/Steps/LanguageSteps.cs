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
    [Binding, Scope(Feature = "Manage Languages")]
    public class LanguageSteps
    {
        private readonly IWebDriver _driver;
        private readonly LanguagePage _languagePage;

        public LanguageSteps()
        {
            _driver = WebDriverMngr.Instance.GetDriver();
            _languagePage = new LanguagePage(_driver);
        }

        #region Add Language
        [When(@"I navigate to the language tab")]
        public void GivenINavigateToLanguageTab()
        {
            _languagePage.NavigateToLanguageTab();
        }
        [When(@"I click on the Add New button")]
        public void WhenIClickOnAddNewButton() => _languagePage.ClickAddNew(); // ensure active tab

        [When(@"I add a new language record from file ""(.*)"" JSON index (.*)")]
        public void WhenIAddLanguageRecord(string fileName, int index)
        {
            _languagePage.AddLanguage(JsonHelper.ReadJsonList<LanguageModel>(fileName, "Languages")[index], isCleanupNeeded: true);
        }

        [When(@"I click on the Add button")]
        public void WhenIClickAddButton() => _languagePage.ClickAdd();

        [Then(@"I should see the language record from file ""(.*)"" JSON index (.*) in my profile")]
        public void ThenIShouldSeeLanguageRecord(string fileName, int index)
        {
            var data = JsonHelper.ReadJsonList<LanguageModel>(fileName, "Languages")[index];
            Assert.That(_languagePage.IsLanguageAdded(data.Language!, data.Level!), Is.True,
                $"Record '{data.Language} - {data.Level}' not visible!");
        }
        #endregion

        #region Edit Multiple
        [Given(@"I have added language record for edit from file ""(.*)"" JSON index (.*)")]
        public void GivenIHaveAddedLanguageRecordForEdit(string fileName, int index)
        {
            _languagePage.NavigateToLanguageTab();
            _languagePage.ClickAddNew();
            _languagePage.AddLanguage(JsonHelper.ReadJsonList<LanguageModel>(fileName, "Languages")[index], isCleanupNeeded: false);
            _languagePage.ClickAdd();
        }

        [When(@"I edit the language record from JSON index (.*) to updated JSON index (.*) file ""(.*)""")]
        public void WhenIEditLanguageRecord(int oldIndex, int newIndex, string fileName)
        {
            _languagePage.EditLanguage(JsonHelper.ReadJsonList<LanguageModel>(fileName, "Languages")[oldIndex], JsonHelper.ReadJsonList<LanguageModel>(fileName, "Languages")[newIndex]);
        }

        [Then(@"I should see the updated language record from file ""(.*)"" JSON index (.*) in my profile")]
        public void ThenIShouldSeeUpdatedLanguage(string fileName, int index)
        {
            var data = JsonHelper.ReadJsonList<LanguageModel>(fileName, "Languages")[index];
            Assert.That(_languagePage.IsLanguageAdded(data.Language!, data.Level!), Is.True);
        }
        #endregion

        #region Delete certification 
        [Given(@"I have added Language record for delete from file ""(.*)"" JSON index (.*)")]
        public void GivenIHaveAddedLanguageRecordForDelete(string fileName, int index)
        {
            _languagePage.NavigateToLanguageTab();
            _languagePage.ClickAddNew();
            _languagePage.AddLanguage(JsonHelper.ReadJsonList<LanguageModel>(fileName, "Languages")[index], isCleanupNeeded: false);
            _languagePage.ClickAdd();
        }
        [When(@"I delete the language record from file ""(.*)"" JSON index (.*)")]
        public void WhenIDeleteLanguageRecord(string fileName, int index) => _languagePage.DeleteLanguage(JsonHelper.ReadJsonList<LanguageModel>(fileName, "Languages")[index]);

        [Then(@"the language record from file ""(.*)"" JSON index (.*) should not be listed in my languages")]
        public void ThenLanguageShouldNotBeListed(string fileName, int index)
        {
            var data = JsonHelper.ReadJsonList<LanguageModel>(fileName, "Languages")[index];
            Assert.That(_languagePage.IsLanguageListed(data), Is.False, $"Record '{data.Language} - {data.Level}' should be deleted but is still listed.");
        }
        #endregion

        #region duplicate and Invalid data
        [Given(@"I have added language record for duplication from file ""(.*)"" JSON index (.*)")]
        public void GivenIHaveAddedLanguageRecordForDuplication(string fileName, int index)
        {
            _languagePage.NavigateToLanguageTab();
            _languagePage.ClickAddNew();
            _languagePage.AddLanguage(JsonHelper.ReadJsonList<LanguageModel>(fileName, "Languages")[index], isCleanupNeeded: true);
            _languagePage.ClickAdd();
        }

        [When(@"I add a new language record with duplicate from file ""(.*)"" JSON index (.*)")]
        public void WhenIAddLanguageRecordWithDuplicate(string fileName, int index)
        {
            _languagePage.AddLanguage(JsonHelper.ReadJsonList<LanguageModel>(fileName, "Languages")[index], isCleanupNeeded: false);
        }

        [Then(@"I should see an error message indicating duplicate language not allowed")]
        public void ThenDuplicateError() =>
            _languagePage.IsErrorMessageDisplayed("This language record already exists.");

        [When(@"I add a new language record with invalid data from file ""(.*)"" JSON index (.*)")]
        public void WhenIAddLanguageRecordWithInvalidData(string fileName, int index)
        {
            var data = JsonHelper.ReadJsonList<LanguageModel>(fileName, "Languages")[index];
            var incompleteData = new LanguageModel
            {
                Language = data.Language,
                Level = data.Level
            };
            _languagePage.AddLanguage(incompleteData, isCleanupNeeded: false);
        }

        [Then(@"I should see an error message indicating field is required")]
        public void ThenFieldRequiredError() =>
            _languagePage.IsErrorMessageDisplayed("Field is required.");
        #endregion
    }
}
