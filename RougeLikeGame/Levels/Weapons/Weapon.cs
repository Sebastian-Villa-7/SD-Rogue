using System;
using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace SandBox01.Levels.Weapons;

public class Weapon : Item
{
    public string Name { get; private set; }
    public int DamageBonus { get; private set; }
    public ConsoleColor Color { get; private set; }
    public string WeaponType { get; private set; }

    public Weapon(Vector2 pos, string weaponType, int damageBonus, ConsoleColor color)
        : base(GetGlyph(weaponType), pos)
    {
        WeaponType = weaponType;
        Name = $"{weaponType} (+{damageBonus})";
        DamageBonus = damageBonus;
        Color = color;
    }

    private static char GetGlyph(string weaponType)
    {
        switch (weaponType)
        {
            case "Sword":
                return '/';
            case "Axe":
                return '!';
            case "Dagger":
                return ';';
            default:
                return '?';
        }
    }

    public override void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, Color);
    }
}