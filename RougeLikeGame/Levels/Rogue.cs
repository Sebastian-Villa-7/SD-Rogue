using RogueLib.Utilities;

namespace RlGameNS;

public class Rogue : Player
{
    public override string HUD =>
        $"Level:{_level}  Gold: {Gold,4}  Hp: {_hp}({_maxHp})" +
        $"  Str: {_str}({_maxStr})" +
        $"  Arm: {_arm}   Exp: {_exp}/{10} Turn: {_turn}";

    public override void Update()
    {
        base.Update();
    }
}