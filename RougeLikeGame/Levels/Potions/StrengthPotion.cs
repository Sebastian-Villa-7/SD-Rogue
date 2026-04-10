using System;
using System.Collections.Generic;
using System.Text;
using RogueLib.Utilities;

namespace SandBox01.Levels.Potions;

public class StrengthPotion : Potion
{
    public int StrengthBonus { get; private set; }
    public int Duration { get; private set; }

    public StrengthPotion(Vector2 pos, int strengthBonus = 5, int duration = 20) : base('S', pos, "Strength Potion", ConsoleColor.Red)
    {
        StrengthBonus = strengthBonus;
        Duration = duration;
    }

    public override void ApplyEffect(Rogue player)
    {
        player.AddStrengthBuff(StrengthBonus, Duration);
    }
}
