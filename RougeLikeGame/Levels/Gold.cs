using RogueLib.Dungeon;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SandBox01.Levels
{
    internal class Gold : Item
    {
        public int Amount { get; set; }

        public Gold(Vector2 pos, int amount) : base('*', pos)
        {

        }
        public override void Draw(IRenderWindow disp)
        {
            disp.Draw(Glyph, Pos, ConsoleColor.Yellow);
        }
    }
}
