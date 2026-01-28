using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAutomationFramework.Utilities
{
    /// <summary>
    /// Provides generic login utilities usable across all tests (Education, Certification, etc.).
    /// </summary>
    public class LoginHelper
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly WebDriverWait _waitForToaster;
        private readonly string _baseUrl;

        public LoginHelper(IWebDriver driver)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
            _waitForToaster = new WebDriverWait(_driver, TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// Navigates to the login page using the BaseUrl from configuration.
        /// </summary>
        public void GoToLoginPage(string url)
        {
            try
            {
                _driver.Navigate().GoToUrl(url);
                OpenLoginModal();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to navigate to the login page.", ex);
            }
        }

        /// <summary>
        /// Clicks the Sign In link and waits for the login form modal to appear.
        /// </summary>
        public void OpenLoginModal()
        {
            try
            {
                var signIn = _wait.Until(d => d.FindElement(By.LinkText("Sign In")));
                signIn.Click();
                _wait.Until(d => d.FindElement(By.Name("email")).Displayed);
            }
            catch (NoSuchElementException)
            {
                throw new Exception("Sign In link not found on the page.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error while opening the login modal.", ex);
            }
        }

        /// <summary>
        /// Logs in using provided credentials. This version handles the Mars modal dialog.
        /// </summary>
        public void Login(string email, string password)
        {
            try
            {
                var activeModal = _wait.Until(d => d.FindElement(By.CssSelector("div.ui.modal.transition.visible.active")));

                var emailInput = activeModal.FindElement(By.Name("email"));
                var passwordInput = activeModal.FindElement(By.Name("password"));
                var loginButton = activeModal.FindElement(By.XPath(".//button[normalize-space(.)='Login']"));

                emailInput.Clear();
                emailInput.SendKeys(email);
                passwordInput.Clear();
                passwordInput.SendKeys(password);
                loginButton.Click();

                //// Wait until home page is visible or profile tab appears
                //_wait.Until(d => d.Url.Contains("/Account/Profile"));
            }
            catch (NoSuchElementException)
            {
                throw new Exception("One or more login elements not found in the modal.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error performing login action.", ex);
            }
        }
    }
}
