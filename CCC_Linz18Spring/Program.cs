using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using MoreLinq;
using Newtonsoft.Json;
using Whiteparse;

namespace CCC_Linz18Spring
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            const int level = 3;
            for (var i = 0; i <= 5; i++)
            {
                var obj = JsonConvert.DeserializeObject<Input>(
                    File.ReadAllText($"C:\\data\\Dropbox\\Projekte\\Code\\CCC_Linz18Spring\\data\\level{level}\\lvl{level}-{i}.json"));
                string solved = SolveLevel(obj);
                Console.WriteLine(solved);
                Console.WriteLine("Success on " + i);
                File.WriteAllText($"C:\\data\\Dropbox\\Projekte\\Code\\CCC_Linz18Spring\\data\\level{level}\\solution-{i}.txt", solved);
            }
        }

        private static string SolveLevel(Input input)
        {
            var observations = new List<Observation>();

            Dictionary<string, List<Asteroid>> dict = input.images.Where(e => e.HasAsteroid())
                .Select(e => e.GetAsteroid())
                .Select(e => (e, e.GetShapeId()))
                .GroupBy(e => e.Item2)
                .ToDictionary(e => e.Key, e => e.Select(f => f.Item1)
                    .OrderBy(f => f.Image.timestamp)
                    .ToList());

            int maxPeriodicity = input.end - input.start;
            var fringe = new HashSet<int>();

            int currentPeriod = 2;
            while (currentPeriod < maxPeriodicity)
            {
                foreach (KeyValuePair<string, List<Asteroid>> pair in dict)
                {
                    Dictionary<int, Asteroid> asteroids = GetTimestampDict(pair.Value);
                    foreach (var ri in fringe) asteroids.Remove(ri);

                    while (true)
                    {
                        List<int> toRemove = null;
                        foreach (var tsPair in asteroids)
                        {
                            toRemove = IsPeriodValid(tsPair.Value, asteroids, input.start, input.end, currentPeriod);
                            if (toRemove != null) break;
                        }

                        // found something
                        if (toRemove != null)
                        {
                            // remove valid asteroid
                            foreach (int ri in toRemove)
                            {
                                fringe.Add(ri);
                                asteroids.Remove(ri);
                            }

                            // add the found observation
                            observations.Add(new Observation(pair.Value.First(), toRemove.First(), toRemove.Last(),
                                toRemove.Count, currentPeriod));
                        }
                        else
                        {
                            // brute force done without results
                            break;
                        }
                    }
                }

                currentPeriod++;
            }

            return string.Join("\n", observations.OrderBy(e => e.FirstSeen).Select(e => e.ToString()));
        }

        private static List<int> IsPeriodValid(Asteroid asteroid, Dictionary<int, Asteroid> asteroids, int start, int end, int period)
        {
            start = GetStartPeriod(start, asteroid.Image.timestamp, period);
            for (int i = start; i <= end; i += period)
            {
                if (!asteroids.ContainsKey(i))
                    return null;
            }

            // remove them
            var list = new List<int>();
            for (int i = start; i <= end; i += period)
            {
                list.Add(i);
            }

            if (list.Count < 4)
                return null;

            list.Sort();
            return list;
        }

        private static int GetStartPeriod(int start, int time, int period)
        {
            int result = time;
            while (result >= start)
                result -= period;

            return result + period;
        }

        private static Dictionary<int, Asteroid> GetTimestampDict(List<Asteroid> asteroids)
        {
            return asteroids.ToDictionary(e => e.Image.timestamp, e => e);
        }
    }

    public class Observation
    {
        public int? FirstSeen;
        public int? LastSeen;
        public int Count = 1;
        public readonly Asteroid Asteroid;
        public int Period;

        public Observation(Asteroid asteroid)
        {
            Asteroid = asteroid;
            FirstSeen = asteroid.Image.timestamp;
            LastSeen = asteroid.Image.timestamp;
        }

        public Observation(Asteroid asteroid, int firstSeen, int lastSeen, int count, int period = 0)
        {
            Asteroid = asteroid;
            FirstSeen = firstSeen;
            LastSeen = lastSeen;
            Count = count;
            Period = period;
        }

        public bool AddObservation(Asteroid other, bool skipShape = false)
        {
            if (Asteroid.EqualShape(other) || skipShape)
            {
                Count++;
                if (other.Image.timestamp > LastSeen)
                    LastSeen = other.Image.timestamp;

                return true;
            }

            return false;
        }

        public new string ToString()
        {
            return $"{FirstSeen} {LastSeen} {Count}";
        }
    }
}