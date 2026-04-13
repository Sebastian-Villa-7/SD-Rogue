using RogueLib.Dungeon;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandBox01.Actors
{
    internal class Troll : Enemy
    {
        public Troll(Vector2 pos) : base(pos, 'T', ConsoleColor.DarkRed, 20, 10) { }

        public override void Act(Vector2 playerPos, HashSet<Vector2> walkable)
        {
            if (_turnCounter % 2 != 0)
                return;

            var dx = Math.Sign(playerPos.X - Pos.X);
            var dy = Math.Sign(playerPos.Y - Pos.Y);
            var newPos = Pos + new Vector2(dx, dy);

            if (walkable.Contains(newPos))
                Pos = newPos;
        }

        public override void Draw(IRenderWindow disp)
        {
            disp.Draw(Glyph, Pos, Color);
        }
    }
}
