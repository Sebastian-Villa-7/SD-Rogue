using RogueLib.Dungeon;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandBox01.Actors
{
    public class TrollFactory : EnemyFactory
    {
        public override Enemy CreateEnemy(Vector2 pos) => new Troll(pos);
    }
}
