using RogueLib.Dungeon;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandBox01.Actors
{
    public class GoblinFactory : EnemyFactory
    {
        public override Enemy CreateEnemy(Vector2 pos) => new Goblin(pos);
    }
}
