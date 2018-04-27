using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
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
                var obj = JsonConvert.DeserializeObject<Input>(
                    File.ReadAllText($"C:\\data\\Dropbox\\Projekte\\Code\\CCC_Linz18Spring\\data\\level2\\lvl2-{i}.json"));
                string solved = SolveLevel(obj);
                Console.WriteLine(solved);
                File.WriteAllText($"C:\\data\\Dropbox\\Projekte\\Code\\CCC_Linz18Spring\\data\\level2\\solution-{i}.txt", solved);
            }
        }

        private static string SolveLevel(Input input)
        {
            var observations = new List<Observation>();

            foreach (Image img in input.images)
            {
                if (img.HasAsteroid())
                {
                    // search for existing observations
                    var match = false;
                    foreach (Observation obsv in observations)
                    {
                        if (Image.EqualAsteroid(obsv.asteroid, img.GetAsteroid()))
                        {
                            obsv.count++;
                            if (img.timestamp > obsv.lastSeen)
                                obsv.lastSeen = img.timestamp;

                            match = true;
                        }
                    }

                    if (!match)
                    {
                        // append new
                        observations.Add(
                            new Observation {firstSeen = img.timestamp, lastSeen = img.timestamp, asteroid = img.GetAsteroid()});
                    }
                }
            }

            return string.Join("\n", observations.OrderBy(e => e.firstSeen).Select(e => $"{e.firstSeen} {e.lastSeen} {e.count}"));
        }
    }

    public class Observation
    {
        public int? firstSeen;
        public int? lastSeen;
        public int count = 1;

        public Matrix<double> asteroid;
    }
}