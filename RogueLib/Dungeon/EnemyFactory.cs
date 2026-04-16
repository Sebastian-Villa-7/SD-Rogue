using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLib.Dungeon
{
    public abstract class EnemyFactory
    {
        // factory method - subclasses decide which enemy to create
        public abstract Enemy CreateEnemy(Vector2 pos);

        // common spawning logic shared by all factories
        public Enemy SpawnEnemy(HashSet<Vector2> floor, Vector2 playerPos, int minDistance)
        {
            var rng = new Random();
            Vector2 spawnPos;

            do
            {
                spawnPos = floor.ElementAt(rng.Next(floor.Count));
            } while ((spawnPos - playerPos).KingLength < minDistance);

            return CreateEnemy(spawnPos);
        }
    }
}
