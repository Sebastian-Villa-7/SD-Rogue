using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLib.Interfaces
{
    public interface IDamageable
    {
        int Hp { get; }
        int Attack { get; }
        bool IsDead { get; }
        string Name { get; }
        void TakeDamage(int damage);
    }
}
