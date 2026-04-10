using System;
using System.Collections.Generic;
using System.Text;
using RogueLib.Utilities;

namespace SandBox01.Levels.Potions;

public class ArmourPotion : Potion
{
    public int ArmourBonus { get; private set; }
    public int Duration { get; private set; }

    public ArmourPotion(Vector2 pos, int armorBonus = 3, int duration = 20) : base('A', pos, "Armour Potion", ConsoleColor.Cyan)
    {
        ArmourBonus = armorBonus;
        Duration = duration;
    }

    public override void ApplyEffect(Rogue player)
    {
        player.AddArmourBuff(ArmourBonus, Duration);
    }
}
