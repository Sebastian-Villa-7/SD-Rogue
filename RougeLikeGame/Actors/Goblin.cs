using RogueLib.Dungeon;
using RogueLib.Utilities;


namespace SandBox01.Actors
{
    public class Goblin : Enemy
    {

        private static readonly Random _rng = new Random();

        private static readonly Vector2[] _directions =
        {
            Vector2.N, Vector2.S, Vector2.E, Vector2.W
        };

        public Goblin(Vector2 pos) : base(pos, 'g', ConsoleColor.Green, 6, 3, "Goblin") { }

        public override void Act(Vector2 playerPos, HashSet<Vector2> walkable)
        {
            var dir = _directions[_rng.Next(_directions.Length)];
            var newPos = Pos + dir;

            if (walkable.Contains(newPos))
                Pos = newPos;
        }


        public override void Draw(IRenderWindow disp)
        {
            disp.Draw(Glyph, Pos, Color);
        }
    }
}
