using System;
using System.Collections.Generic;
using System.Text;
using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace SandBox01.Levels.Potions;

public abstract class Potion : Item
{
    public string Name { get; protected set; }
    public ConsoleColor Color { get; protected set; }

    protected Potion(char glyph, Vector2 pos, string name, ConsoleColor color) : base(glyph, pos)
    {
        Name = name;
        Color = color;
    }

    public abstract void ApplyEffect(Rogue player);

    public override void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, Color);
    }
}
