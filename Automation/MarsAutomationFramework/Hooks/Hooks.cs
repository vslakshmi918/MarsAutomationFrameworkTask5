using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using MarsAutomationFramework.Models;
using MarsAutomationFramework.Pages;
using MarsAutomationFramework.Utilities;
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

        // Track data added during the scenario
        private static readonly List<LanguageModel> _languagesAdded = new();
        private static readonly List<SkillModel> _skillsAdded = new();

        public Hooks(ScenarioContext context) => _context = context;

        public static void AddToCleanupList(object data)
        {
            if (data is LanguageModel e) _languagesAdded.Add(e);
            else if (data is SkillModel c) _skillsAdded.Add(c);

        }

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

        private static void LoginAutomatically()
        {
            var login = new LoginHelper(_driver!);
            login.GoToLoginPage("http://localhost:5003");
            login.Login("subhavangalapudi@gmail.com", "Subha1982");
            new WebDriverWait(_driver!, TimeSpan.FromSeconds(5)).Until(d => d.Url.Contains("/Account/Profile"));
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _driver = WebDriverMngr.Instance.GetDriver();

            _scenario = _extent!.CreateTest(_context.ScenarioInfo.Title);
            // Auto login
            LoginAutomatically();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            try
            {
                if (_languagesAdded.Count > 0)
                    CleanupLanguages();
                else if (_skillsAdded.Count > 0)
                    CleanupSkills();

                // Screenshot on failure
                if (_context.TestError != null)
                {
                    string path = ScreenshotHelper.TakeScreenshot(
                        _driver!, _context.ScenarioInfo.Title);

                    _scenario!.Fail(_context.TestError.Message)
                              .AddScreenCaptureFromPath(Path.GetFullPath(path));
                }
            }
            finally
            {
                WebDriverMngr.Instance.Quit();
            }
        }

        private bool ShouldSkipCleanup()
        {
            string[] skipTags = { "Delete", "Invalid" };
            return _context.ScenarioInfo.Tags.Any(t => skipTags.Contains(t));
        }

        private void CleanupLanguages()
        {
            if (ShouldSkipCleanup()) return;
            var page = new LanguagePage(_driver!);

            foreach (var record in _languagesAdded)
            {
                try
                {
                    page.DeleteLanguage(record);
                    _scenario!.Log(Status.Info, $"Deleted Education: {record.Language}");
                }
                catch { }
            }
        }

        private void CleanupSkills()
        {
            if (ShouldSkipCleanup()) return;
            var page = new SkillPage(_driver!);

            foreach (var record in _skillsAdded)
            {
                try
                {
                    page.DeleteSkill(record);
                    _scenario!.Log(Status.Info, $"Deleted Skills: {record.Skill}");
                }
                catch { }
            }
        }

        [AfterTestRun]
        public static void AfterTestRun() => _extent!.Flush();
    }
}
