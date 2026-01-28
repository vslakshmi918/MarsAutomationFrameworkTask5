using MarsAutomationFramework.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.Log;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAutomationFramework.Pages
{
    public class SkillPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly WebDriverWait _waitForToaster;

        public SkillPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            _waitForToaster = new WebDriverWait(driver, TimeSpan.FromSeconds(1));
        }

        public void NavigateToSkillTab()
        {
            var tab = _wait.Until(d => d.FindElement(By.CssSelector("a[data-tab='second']")));
            tab.Click();
            bool status = tab.Displayed;
            // Wait until Skills tab is active
            _wait.Until(d => tab.GetAttribute("class")!.Contains("active"));

            // Wait until Add New button in Skills tab is visible
            _wait.Until(d =>
                d.FindElement(By.XPath("//div[@data-tab='second']//div[text()='Add New' and contains(@class, 'button')]")).Displayed);
        }

        public void AddSkill(SkillModel data, bool isCleanupNeeded = false)
        {
            try
            {
                var langugaeInput = _driver.FindElement(By.XPath("//input[@placeholder='Add Skill']"));
                langugaeInput.Clear();
                langugaeInput.SendKeys(data.Skill!);

                var dropdown = _driver.FindElement(By.Name("level"));
                var options = dropdown.FindElements(By.TagName("option"));

                foreach (var option in options)
                {
                    if (option.Text.Trim().Equals(data.Level, StringComparison.OrdinalIgnoreCase))
                    {
                        option.Click();
                        break;
                    }
                }

                if (isCleanupNeeded == true)
                {
                    Hooks.Hooks.AddToCleanupList(data);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding certification.", ex);
            }

        }

        public void ClickAddNew()
        {
            try
            {
                var addNewButton = _wait.Until(d =>
                    d.FindElement(By.XPath("//div[@data-tab='second']//div[text()='Add New' and contains(@class, 'button')]")));
                addNewButton.Click();
            }
            catch (WebDriverTimeoutException ex)
            {
                throw new Exception("Timed out waiting for Add New button.", ex);
            }
            catch (NoSuchElementException ex)
            {
                throw new Exception("Add New button not found.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error clicking Add New button.", ex);
            }
        }

        public void ClickAdd()
        {
            var addButton = _driver.FindElement(By.XPath("//input[@type='button' and @value='Add']"));
            addButton.Click();
            _wait.Until(driver =>
                driver.FindElements(By.XPath("//table[@class='ui fixed table']//tr")).Count > 0);
        }

        public void ClickUpdate()
        {
            try
            {
                var updateButton = _wait.Until(d =>
                    d.FindElement(By.XPath("//input[@type='button' and @value='Update']")));
                updateButton.Click();
            }
            catch (NoSuchElementException ex)
            {
                throw new Exception("Update button not found.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error clicking Update button.", ex);
            }
        }

        public bool IsAddSkillFormVisible()
        {
            try
            {
                var langugaeInput = _driver.FindElement(By.XPath("//input[@placeholder='Add Skill']"));
                var dropdown = _driver.FindElement(By.Name("level"));

                return langugaeInput.Displayed && dropdown.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsSkillAdded(string skill, string level)
        {
            try
            {
                var row = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
                    By.XPath($"//table[@class='ui fixed table']//tr[td[contains(text(), '{skill}')] and td[contains(text(), '{level}')]]")));

                return row != null;
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine($"Skill record '{skill} - {level}' not found within timeout.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error while verifying Skill record: {ex.Message}");
                return false;
            }
        }

        public bool IsSkillListed(SkillModel data)
        {
            try
            {
                // Wait for either the record to appear or timeout
                var rows = _wait.Until(driver =>
                    driver.FindElements(By.XPath(
                        $"//table[@class='ui fixed table']//tr[td[text()='{data.Skill}'] and " +
                        $"td[text()='{data.Level}']]"))
                );

                return rows.Count > 0;
            }
            catch (WebDriverTimeoutException)
            {
                // Timeout means record is not listed
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if Skill record is listed: {ex.Message}");
                return false;
            }
        }

        public void EditSkill(SkillModel oldData, SkillModel newData)
        {
            try
            {
                // Wait for the correct row to be present before clicking edit
                var editIcon = _wait.Until(driver =>
                    driver.FindElement(By.XPath(
                        $"//table[@class='ui fixed table']//tr[td[text()='{oldData.Skill}'] and " +
                        $"td[text()='{oldData.Level}']]//i[contains(@class,'write')]")));

                editIcon.Click();

                if (IsAddSkillFormVisible())
                {
                    AddSkill(newData, isCleanupNeeded: true);
                    ClickUpdate();
                }
            }
            catch (NoSuchElementException)
            {
                throw new Exception($"Edit icon not found for record: {oldData.Skill}, {oldData.Level}");
            }
            catch (WebDriverTimeoutException)
            {
                throw new Exception("Timed out waiting for the Skill edit form to appear.");
            }
        }

        public void DeleteSkill(SkillModel data)
        {
            try
            {
                var deleteIcon = _driver.FindElement(By.XPath(
                        $"//table[@class='ui fixed table']//tr[td[text()='{data.Skill}'] and " +
                        $"td[text()='{data.Level}']]//i[contains(@class,'remove')]"));

                deleteIcon.Click();

                // Wait until the specific row is no longer present
                _wait.Until(driver =>
                    driver.FindElements(By.XPath(
                        $"//table[@class='ui fixed table']//tr[td[text()='{data.Skill}'] and " +
                        $"td[text()='{data.Level}']]")).Count == 0);
            }
            catch (NoSuchElementException)
            {
                throw new Exception($"Delete icon not found for record: {data.Skill}, {data.Level}");
            }
            catch (WebDriverTimeoutException)
            {
                throw new Exception($"Record was not removed from the table after clicking delete: {data.Skill} - {data.Level}");
            }
        }

        public bool IsErrorMessageDisplayed(string message)
        {
            try
            {

                var element = _waitForToaster.Until(driver =>
                {
                    var elements = driver.FindElements(By.CssSelector("div.ns-box-inner"));
                    return elements.FirstOrDefault(); // returns null until at least one is found
                });

                if (element == null)
                    return false;

                string data = element.Text.Trim();
                return data.Contains(message);
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Toast did not appear in time.");
                return false;
            }
            catch (StaleElementReferenceException)
            {
                Console.WriteLine("Toast disappeared before reading text.");
                return false;
            }
        }
    }
}
