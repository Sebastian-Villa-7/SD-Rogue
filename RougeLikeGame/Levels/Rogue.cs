using RogueLib.Utilities;
using SandBox01.Levels.Weapons;

namespace SandBox01.Levels;

public class Rogue : Player
{
    // The fields for buffing stats
    private int _strengthBonus = 0;
    private int _strengthBonusTurnsLeft = 0;
    private int _armorBonus = 0;
    private int _armorBonusTurnsLeft = 0;

    // Equipment
    public Weapon? EquippedWeapon { get; set; }

    // Override to add the buff
    public override int Strength => _str + _strengthBonus;
    public override int Armor => _arm + _armorBonus;

    // Override Attack to include weapon damage
    public override int Attack => _str + _strengthBonus + (EquippedWeapon?.DamageBonus ?? 0);

    public void AddStrengthBuff(int bonus, int duration)
    {
        _strengthBonus = bonus;
        _strengthBonusTurnsLeft = duration;
    }

    public void AddArmourBuff(int bonus, int duration)
    {
        _armorBonus = bonus;
        _armorBonusTurnsLeft = duration;
    }

    public void Heal(int amount)
    {
        int oldHp = _hp;
        _hp = Math.Min(_hp + amount, _maxHp);
        int actualHeal = _hp - oldHp;
    }

    // Equip a weapon
    public void EquipWeapon(Weapon newWeapon)
    {
        EquippedWeapon = newWeapon;
    }

    // Updated HUD to show equipped weapon
    public override string HUD =>
        $"G:{_gold} HP:{_hp}/{_maxHp} " +
        $"Str:{Strength}(+{_strengthBonus}) " +
        $"Arm:{Armor}(+{_armorBonus}) " +
        $"Wpn:{(EquippedWeapon?.WeaponType?[..2] ?? "--")} " +
        $"Turn:{_turn}";

    public override void Update()
    {
        base.Update();

        // Decrease buff timers
        if (_strengthBonusTurnsLeft > 0)
        {
            _strengthBonusTurnsLeft--;
            if (_strengthBonusTurnsLeft == 0)
                _strengthBonus = 0;
        }

        if (_armorBonusTurnsLeft > 0)
        {
            _armorBonusTurnsLeft--;
            if (_armorBonusTurnsLeft == 0)
                _armorBonus = 0;
        }
    }
}