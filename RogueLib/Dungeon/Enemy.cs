using RogueLib.Utilities;
using System;

using System.Text;

namespace RogueLib.Dungeon
{
    public abstract class Enemy : IActor, IDrawable
    {
        public string Name { get; protected set; }
        public Vector2 Pos { get; set; }

        public char Glyph { get; protected init; }

        public ConsoleColor Color { get; protected init; }
        public int Attack { get; protected set; }
        public bool IsDead => _hp <= 0;

        protected int _hp;
        protected int _turnCounter;

        protected Enemy(Vector2 pos, char glyph, ConsoleColor color, int hp, int attack, string name)
        {
            Pos = pos;
            Glyph = glyph;
            Color = color;
            _hp = hp;
            Attack = attack;
            Name = name;
        }

        public void TakeDamage(int damage)
        {
            _hp -= damage;
        }

        public abstract void Act(Vector2 playerPos, HashSet<Vector2> walkable);
        public abstract void Draw(IRenderWindow disp);

        public virtual void Update() => _turnCounter++;
    }
}
