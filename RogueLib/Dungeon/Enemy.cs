using RogueLib.Utilities;
using System;

using System.Text;

namespace RogueLib.Dungeon
{
    public abstract class Enemy : IActor, IDrawable
    {
        public Vector2 Pos { get; set; }

        public char Glyph { get; protected init; }

        public ConsoleColor Color { get; protected init; }

        protected int _hp;
        protected int _attack;
        protected int _turnCounter;

        protected Enemy(Vector2 pos, char glyph, ConsoleColor color, int hp, int attack)
        {
            Pos = pos;
            Glyph = glyph;
            Color = color;
            _hp = hp;
            _attack = attack;
        }

        public abstract void Act(Vector2 playerPos, HashSet<Vector2> walkable);
        public abstract void Draw(IRenderWindow disp);

        public virtual void Update() => _turnCounter++;
    }
}
