using RogueLib.Utilities;

namespace SandBox01.Levels;

public class Rogue : Player
{
    // The fields for buffing stats
    private int _strengthBonus = 0;
    private int _strengthBonusTurnsLeft = 0;
    private int _armorBonus = 0;
    private int _armorBonusTurnsLeft = 0;

    // Override to add the buff
    public override int Strength => _str + _strengthBonus;
    public override int Armor => _arm + _armorBonus;

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