using RogueLib.Dungeon;
using RogueLib.Engine;
using RogueLib.Utilities;
using SandBox01.Actors;
using SandBox01.Levels;
using SandBox01.Levels.Potions;
using SandBox01.Levels.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

namespace RlGameNS;

// -----------------------------------------------------------------------
// The Level is the model, all the game world objects live in the model. 
// player input updates the model, the model updates the view, and the 
// controller runs the whole thing. 
//
// Scene is the base class for all game scenes (levels). Scene is an 
// abstract class that implements IDrawable and ICommandable. 
// 
// A dungeon level is a collection or rooms and tunnels in a 78x25 grid. 
// each tile is at a point, or grid location, represented by a Vector2. 
// 
// *TileSets* are HashSets of grid points, TileSets can be used to tell 
// GameScreen what tiles to draw. TileSets can be combined with Union and 
// Intersect to create complex tile sets.
// -----------------------------------------------------------------------
public class Level : Scene
{
    // ---- level config ---- 
    protected int _levelDepth = 1;
    protected string? _map;
    protected int _senseRadius = 4;

    // --- Tile Sets -----
    protected TileSet _walkables;
    protected TileSet _floor;
    protected TileSet _tunnel;
    protected TileSet _door;
    protected TileSet _decor;
    protected TileSet _discovered;
    protected TileSet _inFov;

    // --- Game Objects ---
    protected List<Item> _item;
    protected List<Enemy> _enemies;
    private Combat _combat;

    private bool _hasAmuletSpawned;

    public Level(Player p, Game game, int depth = 1) 
        : this(p, DungeonLayoutManager.GetRandomLayout(), game, depth)
    {

    }
     public Level(Player p, string map, Game game, int depth = 1)
    {
        if (game == null || p == null || map == null)
            throw new ArgumentNullException("game, player, or map cannot be null");

        _player = p;
        _map = map;
        _game = game;
        _item = new List<Item>();
        _levelDepth = depth;

        initMapTileSets(map);

        _player.Pos = _floor.ElementAt(new Random().Next(_floor.Count));

        updateDiscovered();
        registerCommandsWithScene();
        spreadGold();
        spreadPotions();
        spreadWeapons();
        spreadStairs();

        _enemies = new List<Enemy>();
        _combat = new Combat();
        spawnEnemies();

        if (_levelDepth >= 25)
        {
            spawnAmulet();
        }
    }

    private void spawnAmulet()
    {
        var rng = new Random();
        Vector2 pos;

        do
        {
            pos = _floor.ElementAt(rng.Next(_floor.Count));
        } while (pos == _player!.Pos); 

        _item.Add(new Amulet(pos));
        _hasAmuletSpawned = true;
        MessageLog.Add("You sense a powerful artifact somewhere on this level...");
    }

    private void spreadStairs()
    {
        var rng = new Random();
        Vector2 pos;
        do
        {
            pos = _floor.ElementAt(rng.Next(_floor.Count));
        } while (pos == _player.Pos);  // Don't spawn on player

        _item.Add(new Stairs(pos));
    }

    private void spreadGold()
    {
        var rng = new Random();
        var hm = rng.Next(10, 20);
        for (int i = 0; i < hm; i++)
        {
            var pos = _floor.ElementAt(rng.Next(_floor.Count));
            _item.Add(new Gold(pos, rng.Next(1, 10)));
        }
    }

    private void spreadPotions()
    {
        var rng = new Random();
        var hm = rng.Next(5, 10);
        var validFloorTiles = _floor.ToList();

        for (int i = 0; i < hm && validFloorTiles.Any(); i++)
        {
            var pos = validFloorTiles[rng.Next(validFloorTiles.Count)];
            int potionType = rng.Next(3);

            Potion potion;
            switch (potionType)
            {
                case 0:
                    potion = new StrengthPotion(pos, rng.Next(3, 8), rng.Next(15, 30));
                    break;
                case 1:
                    potion = new ArmourPotion(pos, rng.Next(2, 5), rng.Next(15, 30));
                    break;
                default:
                    potion = new HealthPotion(pos, rng.Next(5, 15));
                    break;
            }

            _item.Add(potion);
        }
    }

    private void spreadWeapons()
    {
        var rng = new Random();
        var weaponCount = rng.Next(2, 5);  // 3-6 weapons per level
        var validFloorTiles = _floor.ToList();

        // Define weapon types
        var weaponTypes = new[]
        {
        ("Sword", 3, ConsoleColor.White),
        ("Axe", 5, ConsoleColor.DarkRed),
        ("Dagger", 2, ConsoleColor.Gray)
    };

        for (int i = 0; i < weaponCount && validFloorTiles.Any(); i++)
        {
            var pos = validFloorTiles[rng.Next(validFloorTiles.Count)];
            var (type, damage, color) = weaponTypes[rng.Next(weaponTypes.Length)];

            var weapon = new Weapon(pos, type, damage, color);
            _item.Add(weapon);
        }
    }

    private void spawnEnemies()
    {
        var rng = new Random();
        var minDistance = 10;

        for (int i = 0; i < 3; i++)
        {
            Vector2 pos;
            do
            {
                pos = _floor.ElementAt(rng.Next(_floor.Count));
            } while ((pos - _player!.Pos).KingLength < minDistance);
            _enemies.Add(new Goblin(pos));
        }

        for (int i = 0; i < 2; i++)
        {
            Vector2 pos;
            do
            {
                pos = _floor.ElementAt(rng.Next(_floor.Count));
            } while ((pos - _player!.Pos).KingLength < minDistance);
            _enemies.Add(new Orc(pos));
        }

        Vector2 trollPos;
        do
        {
            trollPos = _floor.ElementAt(rng.Next(_floor.Count));
        } while ((trollPos - _player!.Pos).KingLength < minDistance);
        _enemies.Add(new Troll(trollPos));
    }

    protected void updateDiscovered()
    {
        _inFov = fovCalc(_player!.Pos, _senseRadius);

        if (_discovered is null)
            _discovered = new TileSet();

        _discovered.UnionWith(_inFov);
    }

    protected TileSet fovCalc(Vector2 pos, int sens)
       => Vector2.getAllTiles().Where(t => (pos - t).KingLength < sens).ToHashSet();

    // -----------------------------------------------------------------------
    public override void Update()
    {
        updateDiscovered();
        _player!.Update();

        foreach (var e in _enemies.ToList())
        {
            e.Update();
            e.Act(_player!.Pos, _walkables);
        }

        if (_player.IsDead)
        {
            _game!.CurrentLevel = new RipScene(_game);
            _levelActive = false;
        }
    }

    public override void Draw(IRenderWindow? disp)
    {
        disp.Draw(MessageLog.Message, new Vector2(0, 0), ConsoleColor.Yellow);

        disp.fDraw(_discovered, _map, ConsoleColor.DarkGray);
        disp.fDraw(_inFov, _map, ConsoleColor.Gray);

        var rng = new Random();
        if (_player.Turn % 5 == 0)
            _player._color = (ConsoleColor)rng.Next(10, 16);
        _player!.Draw(disp);

        drawItems(disp);
        drawEnemies(disp);

        string hud = $"Level: {_levelDepth} {_player.HUD}";
        disp.Draw(hud, new Vector2(0, 39), ConsoleColor.Green);

    }

    public override void DoCommand(Command command)
    {
        if (command.Name == "up")
            MovePlayer(Vector2.N);
        else if (command.Name == "down")
            MovePlayer(Vector2.S);
        else if (command.Name == "left")
            MovePlayer(Vector2.W);
        else if (command.Name == "right")
            MovePlayer(Vector2.E);
        else if (command.Name == "rest")
            PlayerRests();
        else if (command.Name == "help")
        {
            var helpScene = new HelpScene(_game!, this);
            _game!.CurrentLevel = helpScene;
        }
        else if (command.Name == "quit")
            _levelActive = false;
        else if (command.Name == "descend")
        {
            var nextLevel = new Level(_player, _game, _levelDepth + 1);
            _game!.CurrentLevel = nextLevel;
        }
        else if (command.Name == "win")
        {
            WinGame();
        }
        else if (command.Name == "buyHeal")
            BuyHeal();
        else if (command.Name == "buyStrength")
            BuyStrength();
        else if (command.Name == "buyArmour")
            BuyArmour();
    }
    private void WinGame()
    {
        _game!.CurrentLevel = new VictoryScene(_game, this);
        _levelActive = false;
    }

    // -------------------------------------------------------------------------

    private void drawItems(IRenderWindow disp)
    {
        foreach (var item in _item)
        {
            if (_discovered.Contains(item.Pos))
                item.Draw(disp);
        }
    }

    private void drawEnemies(IRenderWindow disp)
    {
        foreach (var enemy in _enemies)
        {
            if (_inFov.Contains(enemy.Pos))
                enemy.Draw(disp);
        }
    }

    private void initMapTileSets(string map)
    {
        _floor = new TileSet();
        _tunnel = new TileSet();
        _door = new TileSet();
        _decor = new TileSet();

        foreach (var (c, p) in Vector2.Parse(map))
        {
            if (c == '.') _floor.Add(p);
            else if (c == '+') _door.Add(p);
            else if (c == '#') _tunnel.Add(p);
            else if (c != ' ') _decor.Add(p);
        }

        _walkables = _floor.Union(_tunnel).Union(_door).ToHashSet();
    }

    private void registerCommandsWithScene()
    {
        RegisterCommand(ConsoleKey.UpArrow, "up");
        RegisterCommand(ConsoleKey.W, "up");

        RegisterCommand(ConsoleKey.DownArrow, "down");
        RegisterCommand(ConsoleKey.S, "down");

        RegisterCommand(ConsoleKey.LeftArrow, "left");
        RegisterCommand(ConsoleKey.A, "left");

        RegisterCommand(ConsoleKey.RightArrow, "right");
        RegisterCommand(ConsoleKey.D, "right");

        RegisterCommand(ConsoleKey.R, "rest");
        RegisterCommand(ConsoleKey.H, "help");
        RegisterCommand(ConsoleKey.Q, "quit");
        RegisterCommand(ConsoleKey.OemPeriod, "descend");

        RegisterCommand(ConsoleKey.D1, "buyHeal");
        RegisterCommand(ConsoleKey.NumPad1, "buyHeal");

        RegisterCommand(ConsoleKey.D2, "buyStrength");
        RegisterCommand(ConsoleKey.NumPad2, "buyStrength");

        RegisterCommand(ConsoleKey.D3, "buyArmour");
        RegisterCommand(ConsoleKey.NumPad3, "buyArmour");
    }

    public void MovePlayer(Vector2 delta)
    {
        var newPos = _player!.Pos + delta;
        var enemy = _enemies.FirstOrDefault(e => e.Pos == newPos);

        if (enemy != null)
        {
            var rng = new Random();

            if (rng.Next(2) == 0)
            {
                _combat.PlayerAttacks(_player, enemy);
                if (enemy.IsDead)
                    _enemies.Remove(enemy);
            }
            else
            {
                _combat.EnemyAttacks(enemy, _player);
            }

            if (_player.IsDead)
            {
                _game!.CurrentLevel = new RipScene(_game);
                _levelActive = false;
            }
        }
        else if (_walkables.Contains(newPos))
        {
            // check for items
            var itemHere = _item.FirstOrDefault(i => i.Pos == newPos);
            if (itemHere != null)
            {
                if (itemHere is Gold gold)
                {
                    _player.Gold += gold.Amount;
                }
                else if (itemHere is Potion potion)
                {
                    potion.ApplyEffect(_player as Rogue);
                }
                else if (itemHere is Weapon weapon)
                {
                    if (_player is Rogue rogue)
                    {
                        rogue.EquipWeapon(weapon);
                        MessageLog.Add($"You picked up {weapon.Name}!");
                    }
                }
                else if (itemHere is Stairs)
                {
                    MessageLog.Add("You descend deeper into the dungeon...");
                    var nextLevel = new Level(_player, _game, _levelDepth + 1);
                    _game!.CurrentLevel = nextLevel;
                    return;
                }

                else if (itemHere is Amulet)
                {
                    MessageLog.Add("You have found the Amulet of Yendor! You win!");
                    WinGame();
                    return;
                }
                _item.Remove(itemHere);
            }

            // move player
            var oldPos = _player!.Pos;
            _player!.Pos = newPos;
            _walkables.Remove(newPos);
            _walkables.Add(oldPos);
            updateDiscovered();
        }
    }

    private void PlayerRests()
    {
        _player!.Rest();
        _player.Update();

        foreach (var enemy in _enemies.ToList())
        {
            enemy.Update();

            if ((enemy.Pos - _player!.Pos).KingLength == 1)
            {
                _combat.EnemyAttacks(enemy, _player);
                break;
            }

            enemy.Act(_player!.Pos, _walkables);
        }

        if (_player.IsDead)
        {
            _game!.CurrentLevel = new RipScene(_game);
            _levelActive = false;
        }
    }

    private void BuyHeal()
    {
        Rogue rogue = (Rogue)_player!;

        if (rogue.Hp >= 5)
        {
            MessageLog.Add("Your HP must be less than 5 to purchase a Heal.");
            return;
        }

        if (rogue.Gold < 10)
        {
            MessageLog.Add("You don't have enough gold to purchase a Heal.");
            return;
        }

        rogue.Gold -= 10;
        rogue.Heal(1);
        MessageLog.Add("You successfully purchased a Heal for 10 gold!");

        ProcessBuyTurn();
    }

    private void BuyStrength()
    {
        Rogue rogue = (Rogue)_player!;

        if (rogue.Gold < 30)
        {
            MessageLog.Add("You don't have enough gold to purchase Strength.");
            return;
        }

        rogue.Gold -= 30;
        rogue.AddStrengthBuff(3, 20);
        MessageLog.Add("You successfully purchased Strength for 30 gold!");

        ProcessBuyTurn();
    }

    private void BuyArmour()
    {
        Rogue rogue = (Rogue)_player!;

        if (rogue.Gold < 50)
        {
            MessageLog.Add("You don't have enough gold to purchase Armour.");
            return;
        }

        rogue.Gold -= 50;
        rogue.AddArmourBuff(3, 30);
        MessageLog.Add("You successfully purchased Armour for 50 gold!");

        ProcessBuyTurn();
    }

    private void ProcessBuyTurn()
    {
        _player!.Update();

        foreach (var enemy in _enemies.ToList())
        {
            enemy.Update();

            if ((enemy.Pos - _player!.Pos).KingLength == 1)
            {
                _combat.EnemyAttacks(enemy, _player);
                break;
            }

            enemy.Act(_player!.Pos, _walkables);
        }

        if (_player.IsDead)
        {
            _game!.CurrentLevel = new RipScene(_game);
            _levelActive = false;
        }
    }

    public void QuitLevel()
    {
        _levelActive = false;
    }
}