using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.Engine;

public class Combat
{
    private readonly Random _rng = new Random();
    private const int _missChance = 30;

    public void PlayerAttacks(Player player, Enemy enemy)
    {
        if (IsMiss())
        {
            MessageLog.Add("You missed!");
            return;
        }

        var damage = _rng.Next(1, player.Attack);
        enemy.TakeDamage(damage);

        if (enemy.IsDead)
            MessageLog.Add($"You killed the {enemy.Name} for {damage} damage!");
        else
            MessageLog.Add($"You hit the {enemy.Name} for {damage} damage!");
    }

    public void EnemyAttacks(Enemy enemy, Player player)
    {
        if (IsMiss())
        {
            MessageLog.Add($"The {enemy.Name} missed!");
            return;
        }

        var damage = _rng.Next(1, enemy.Attack);
        player.TakeDamage(damage);
        MessageLog.Add($"The {enemy.Name} hits you for {damage} damage!");
    }

    private bool IsMiss()
        => _rng.Next(100) < _missChance;
}