using RogueLib.Dungeon;
using RogueLib.Interfaces;
using RogueLib.Utilities;

namespace RogueLib.Engine;

public class Combat
{
    private readonly Random _rng = new Random();
    private const int _playerMissChance = 20;
    private const int _enemyMissChance = 40;

    public void Attack(IDamageable attacker, IDamageable defender, bool isPlayer)
    {
        int missChance = isPlayer ? _playerMissChance : _enemyMissChance;

        if (IsMiss(missChance))
        {
            MessageLog.Instance.Add($"{attacker.Name} missed!");
            return;
        }

        var damage = _rng.Next(1, Math.Max(2, attacker.Attack));
        defender.TakeDamage(damage);

        if (defender.IsDead)
            MessageLog.Instance.Add($"{attacker.Name} killed {defender.Name} for {damage} damage!");
        else
            MessageLog.Instance.Add($"{attacker.Name} hit {defender.Name} for {damage} damage!");
    }

    private bool IsMiss(int chance)
        => _rng.Next(100) < chance;
}