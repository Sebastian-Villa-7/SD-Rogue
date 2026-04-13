using RogueLib.Dungeon;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandBox01.Actors
{
    public class Orc : Enemy
    {
        private static readonly Random _rng = new Random();
        private const int _chaseRadius = 6;

        private static readonly Vector2[] _directions =
        {
        Vector2.N, Vector2.S, Vector2.E, Vector2.W
        };

        public Orc(Vector2 pos) : base(pos, 'o', ConsoleColor.DarkGreen, 12, 6) { }
        public override void Act(Vector2 playerPos, HashSet<Vector2> walkable)
        {
            var distanceToPlayer = (Pos - playerPos).KingLength;

            Vector2 newPos;

            if (distanceToPlayer <= _chaseRadius)
            {
                // chase the player
                var dx = Math.Sign(playerPos.X - Pos.X);
                var dy = Math.Sign(playerPos.Y - Pos.Y);
                newPos = Pos + new Vector2(dx, dy);
            }
            else
            {
                // wander randomly
                var dir = _directions[_rng.Next(_directions.Length)];
                newPos = Pos + dir;
            }

            if (walkable.Contains(newPos))
                Pos = newPos;
        }

        public override void Draw(IRenderWindow disp)
        {
            disp.Draw(Glyph, Pos, Color);
        }
    }
}
