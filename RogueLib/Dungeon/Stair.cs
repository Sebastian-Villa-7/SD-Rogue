using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RlGameNS;

public class Stairs : Item
{
    public Stairs(Vector2 pos) : base('>', pos)  // > symbol for stairs going down
    {
    }

    public override void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, ConsoleColor.Cyan);  // Cyan colored stairs
    }
}
