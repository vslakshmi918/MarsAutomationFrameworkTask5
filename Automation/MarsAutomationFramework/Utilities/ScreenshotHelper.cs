using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAutomationFramework.Utilities
{
    public static class ScreenshotHelper
    {
        public static string TakeScreenshot(IWebDriver driver, string name)
        {

            string currentDir = Environment.CurrentDirectory;
            // Move up three levels: bin → Debug → net8.0
            string projectRoot = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
            string folder = Path.Combine(projectRoot, "Reports", "Screenshots");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string filePath = Path.Combine(folder, $"{name}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            var ss = ((ITakesScreenshot)driver).GetScreenshot();
            ss.SaveAsFile(filePath);

            return filePath;
        }
    }
}
