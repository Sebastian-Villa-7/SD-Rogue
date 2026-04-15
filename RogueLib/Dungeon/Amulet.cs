using System;
using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace SandBox01.Levels;

public class Amulet : Item
{
    public Amulet(Vector2 pos) : base('&', pos) 
    {
    }

    public override void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, ConsoleColor.Magenta);  
    }
}