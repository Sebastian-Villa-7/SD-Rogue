using RogueLib.Dungeon;
using RogueLib.Engine;
using RogueLib.Utilities;
using SandBox01.Levels;
using SandBox01.Levels.Potions;
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
    protected string? _map;
    protected int _senseRadius = 4;

    // --- Tile Sets -----
    // used to keep track of state of tiles on the map
    protected TileSet _walkables; // walkable tiles 
    protected TileSet _floor;
    protected TileSet _tunnel;
    protected TileSet _door;
    protected TileSet _decor; // walls and other decorations, always visible once discovered

    protected TileSet _discovered; // tiles the player has seen
    protected TileSet _inFov;      // current fov of player

    protected List<Item> _item;

    public Level(Player p, string map, Game game)
    {
        if (game == null || p == null || map == null)
            throw new ArgumentNullException("game, player, or map cannot be null");

        _player = p;
        _player.Pos = new Vector2(4, 12); // random, or at stairs
        _map = map;
        _game = game;
        _item = new List<Item>();

        initMapTileSets(map);
        updateDiscovered();
        registerCommandsWithScene();
        spreadGold();
        spreadPotions();
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

            // Randomly choose potion type
            int potionType = rng.Next(3);  // 0 = Strength, 1 = Shield, 2 = HP

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
        // foreach item update
        // foreach NPC update 
        // check for player death -- on death build RIP message
    }

    public override void Draw(IRenderWindow? disp)
    {
        // Draw all discovered tiles in dark gray
        disp.fDraw(_discovered, _map, ConsoleColor.DarkGray);

        // Draw current FOV tiles in bright gray on top (painter's algorithm)
        disp.fDraw(_inFov, _map, ConsoleColor.Gray);

        var rng = new Random();
        if (_player.Turn % 5 == 0)
            _player._color = (ConsoleColor)rng.Next(10, 16);
        _player!.Draw(disp);
        // disp.Draw(_player!.Glyph, _player!.Pos, ConsoleColor.Cyan);

        drawItems(disp);
        drawEnemies(disp);
        disp.Draw(_player.HUD, new Vector2(0, 24), ConsoleColor.Green);
    }

    public override void DoCommand(Command command)
    {
        // player ctl  
        if (command.Name == "up")
        {
            MovePlayer(Vector2.N);
        }
        else if (command.Name == "down")
        {
            MovePlayer(Vector2.S);
        }
        else if (command.Name == "left")
        {
            MovePlayer(Vector2.W);
        }
        else if (command.Name == "right")
        {
            MovePlayer(Vector2.E);
        }
        else if (command.Name == "help")
        {
            // Switch to help scene
            var helpScene = new HelpScene(_game!, this);
            _game!.CurrentLevel = helpScene;
        }
        else if (command.Name == "quit")
        {
            _levelActive = false;
        }
    }

    // -------------------------------------------------------------------------

    private void drawItems(IRenderWindow disp)
    {
        foreach (var item in _item)
        {
            if (_discovered.Contains(item.Pos))
            {
                item.Draw(disp);
            }
        }
    }

    private void drawEnemies(IRenderWindow disp) { }

    private void initMapTileSets(string map)
    {
        var lines = map.Split('\n');

        // ------ rules for map ------
        // . - floor, walkable and transparent.
        // + - door, walkable and transparent // # - tunnel, walkable and transparent
        // ' ' - solid stone, not walkable, not transparent.
        // '|' - wall, not walkable, not transparent, but discoverable.'
        //  others are treated the same as wall.
        // tunnel, wall, and doorways are decor, once discovered they are visible.

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

        //      for (int row = 0; row < lines.Length; ++row) {
        //         for (int col = 0; col < lines[row].Length; ++col) {
        //            char tile = lines[row][col];
        //
        //            if (tile == '.' || tile == '+' || tile == '#') {
        //               _walkables.Add(new Vector2(col, row));
        //               _decor.Add(new Vector2(col, row));
        //            } else if (tile != ' ') {
        //               _decor.Add(new Vector2(col, row));
        //            }
        //         }
        //      }
    }

    // ------------------------------------------------------
    // Commands 
    // ------------------------------------------------------


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

        RegisterCommand(ConsoleKey.H, "help");
        RegisterCommand(ConsoleKey.Q, "quit");
    }


    public void MovePlayer(Vector2 delta)
    {
        var newPos = _player!.Pos + delta;

        if (_walkables.Contains(newPos))
        {
            Item? itemHere = null;
            foreach (var item in _item)
            {
                if (item.Pos == newPos)
                {
                    itemHere = item;
                    break;
                }
            }

            if (itemHere != null)
            {
                if (itemHere is Gold gold)
                {
                    if (_player is Rogue rogue)
                        _player.Gold += gold.Amount;
                }
                else if (itemHere is Potion potion)
                {
                    if (_player is Rogue rogue)
                        potion.ApplyEffect(rogue);
                }

                _item.Remove(itemHere);
            }

            var oldPos = _player!.Pos;
            _player!.Pos = newPos;
            _walkables.Remove(newPos); // new tile is now occupied
            _walkables.Add(oldPos);    // old tile is now free
        }
    }

    public void QuitLevel()
    {
        _levelActive = false;
    }
}