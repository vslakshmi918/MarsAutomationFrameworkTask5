using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAutomationFramework.WebDriver
{
    public class WebDriverMngr
    {
        private static readonly Lazy<WebDriverMngr> lazy =
            new(() => new WebDriverMngr());

        public static WebDriverMngr Instance => lazy.Value;

        private IWebDriver? _driver;

        private WebDriverMngr() { }

        public IWebDriver GetDriver()
        {
            if (_driver == null)
            {
                var options = new ChromeOptions();
                options.AddArgument("--start-maximized");
                options.AddArgument("--disable-notifications");
                _driver = new ChromeDriver(options);
            }

            return _driver;
        }

        public void Quit()
        {
            try { _driver?.Quit(); }
            catch { }
            finally { _driver = null; }
        }
    }
}
