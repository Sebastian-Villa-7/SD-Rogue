using RogueLib.Dungeon;
using RogueLib.Engine;
using RogueLib.Interfaces;
using System;


namespace RogueLib.Utilities;

public abstract class Player : IActor, IDrawable, IDamageable
{
    public string Name { get; set; }
    public Vector2 Pos;
    public char Glyph => '@';
    public ConsoleColor _color = ConsoleColor.White;

    protected int _level = 0;
    protected int _hp = 12;
    protected int _str = 12;
    protected int _arm = 4;
    protected int _exp = 0;
    protected int _gold = 0;
    protected int _maxHp = 12;
    protected int _maxStr = 14;
    protected int _turn = 0;
    private int _restCounter = 0;

    public int Turn => _turn;
    public virtual int Strength => _str;
    public virtual int Armor => _arm;
    public int Hp => _hp;

    public int Gold
    {
        get => _gold;
        set => _gold = value;
    }

    public virtual int Attack => _str;
    public bool IsDead => _hp <= 0;

    public Player()
    {
        Name = "Rogue";
        Pos = Vector2.Zero;
    }

    public virtual string HUD =>
       $"  Gold: {_gold}  Hp: {_hp}({_maxHp})" +
       $"  Str: {Strength}({_maxStr})" +
       $"  Arm: {Armor}   Exp: {_exp}/{10} Turn: {_turn}";

    public virtual void Update()
    {
        _turn++;
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;
    }

    public virtual void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, _color);
    }

    public void Rest()
    {
        if (_hp >= 5)
        {
            MessageLog.Instance.Add("Your wounds have closed, only a potion can help now!");
            return;
        }

        _restCounter++;
        MessageLog.Instance.Add($"You rest... ({_restCounter}/5)");
        if (_restCounter >= 5)
        {
            if (_hp < _maxHp)
            {
                _hp++;
                MessageLog.Instance.Add("You feel better! HP +1");
            }
            _restCounter = 0;
        }
    }
}