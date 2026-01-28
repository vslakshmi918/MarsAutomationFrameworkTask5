using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAutomationFramework.Utilities
{
    public static class JsonHelper
    {
        public static List<T> ReadJsonList<T>(string fileName, string folder)
        {
            string root = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
            string jsonPath = Path.Combine(root, "TestData", folder, fileName);
            string json = File.ReadAllText(jsonPath);
            return JsonConvert.DeserializeObject<List<T>>(json)!;
        }

        public static T ReadJson<T>(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json)!;
        }
    }
}
