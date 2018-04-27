using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Whiteparse;

namespace CCC_Linz18Spring
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            for (var i = 0; i <= 4; i++)
            {
                var obj = JsonConvert.DeserializeObject<RootObject>(
                    File.ReadAllText($"C:\\data\\Dropbox\\Projekte\\Code\\CCC_Linz18Spring\\data\\level1\\lvl1-{i}.json"));
                string solved = SolveLevel(obj);
                Console.WriteLine(solved);
                File.WriteAllText($"C:\\data\\Dropbox\\Projekte\\Code\\CCC_Linz18Spring\\data\\level1\\solution-{i}.txt", solved);
            }
        }

        private static string SolveLevel(RootObject rootObject)
        {
            var sortedList = new List<int>();
            
            foreach (Image img in rootObject.images)
            {
                bool foundAsteroid = img.rows.SelectMany(e => e).Count(e => e > 0) > 0;
                if (foundAsteroid) sortedList.Add(img.timestamp);
            }
            
            sortedList.Sort();

            return string.Join("\n", sortedList);
        }
    }

    public class Image
    {
        public int timestamp { get; set; }
        public int rowcount { get; set; }
        public int colcount { get; set; }
        public List<List<int>> rows { get; set; }
    }

    public class RootObject
    {
        public int start { get; set; }
        public int end { get; set; }
        public int imagecount { get; set; }
        public List<Image> images { get; set; }
    }
}