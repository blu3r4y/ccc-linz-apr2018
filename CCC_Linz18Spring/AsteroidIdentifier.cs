using System.Collections.Generic;

namespace CCC_Linz18Spring
{
    public static class AsteroidIdentifier
    {
        public static List<string> SeenAsteroids = new List<string>();

        public static string GetIdentifier(Asteroid asteroid)
        {
            string id0 = Asteroid.GetUnrotatedShapeId(asteroid.Rotate90(0));
            if (SeenAsteroids.Contains(id0)) return id0;
            
            string id1 = Asteroid.GetUnrotatedShapeId(asteroid.Rotate90(1));
            if (SeenAsteroids.Contains(id1)) return id1;
            
            string id2 = Asteroid.GetUnrotatedShapeId(asteroid.Rotate90(2));
            if (SeenAsteroids.Contains(id2)) return id2;
            
            string id3 = Asteroid.GetUnrotatedShapeId(asteroid.Rotate90(3));
            if (SeenAsteroids.Contains(id3)) return id3;
            
            // add new seen asteroid
            SeenAsteroids.Add(id0);
            return id0;
        }
    }
}