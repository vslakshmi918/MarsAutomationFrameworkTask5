using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using MarsAutomationFramework.WebDriver;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAutomationFramework.Hooks
{
    [Binding]
    public sealed class Hooks
    {
        private static ExtentReports? _extent;
        private static ExtentTest? _scenario;
        private static IWebDriver? _driver;
        private readonly ScenarioContext _context;

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            string currentDir = Environment.CurrentDirectory;
            // Move up three levels: bin → Debug → net8.0
            string projectRoot = Directory.GetParent(currentDir)!.Parent!.Parent!.FullName;
            string dir = Path.Combine(projectRoot, "Reports");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var html = new ExtentSparkReporter(Path.Combine(dir, $"ExtentReport_{timeStamp}.html"));
            _extent = new ExtentReports();
            _extent.AttachReporter(html);
            _extent.AddSystemInfo("Environment", "QA");
            _extent.AddSystemInfo("Browser", "Chrome");
            _extent.AddSystemInfo("Tester", Environment.MachineName);


        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _driver = WebDriverMngr.Instance.GetDriver();

            _scenario = _extent!.CreateTest(_context.ScenarioInfo.Title);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            
        }

        [AfterTestRun]
        public static void AfterTestRun() => _extent!.Flush();
    }
}
