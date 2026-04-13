using System;
using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace SandBox01.Levels;

public class Gold : Item
{
    public int Amount { get; set; }

    public Gold(Vector2 pos, int amount) : base('*', pos)
    {
        Amount = amount;
    }

    public override void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, ConsoleColor.Yellow);
    }
}
