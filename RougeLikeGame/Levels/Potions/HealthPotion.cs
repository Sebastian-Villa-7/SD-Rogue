using System;
using System.Collections.Generic;
using System.Text;
using RogueLib.Utilities;

namespace SandBox01.Levels.Potions;

public class HealthPotion : Potion
{
    public int HealAmount { get; private set; }

    public HealthPotion(Vector2 pos, int healAmount = 10) : base('H', pos, "Health Potion", ConsoleColor.Magenta)
    {
        HealAmount = healAmount;
    }

    public override void ApplyEffect(Rogue player)
    {
        player.Heal(HealAmount);
    }
}
