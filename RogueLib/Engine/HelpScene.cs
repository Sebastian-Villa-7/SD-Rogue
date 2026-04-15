using RogueLib.Dungeon;
using System;
using System.Collections.Generic;
using System.Text;
using RogueLib.Utilities;

namespace RogueLib.Engine;

public class HelpScene : Scene
{
    private Scene? _previousScene;  // Reference to the level we came from
    private Game? _game;

    private string _helpText = """"
        ╔═════════════════════════════════════════════════════════════════════╗
        ║                         ROGUE HELP MANUAL                           ║
        ║  MOVEMENT:                                                          ║
        ║    Arrow Keys / WASD  - Move in cardinal directions                 ║
        ║                                                                     ║
        ║  COMMANDS:                                                          ║
        ║    H            - Show this help screen                             ║
        ║    R            - Rest and recover HP                               ║
        ║    Q            - Quit the game                                     ║
        ║                                                                     ║
        ║  OBJECTIVE:                                                         ║
        ║    Navigate the dungeon, collect gold, and survive!                 ║
        ║    Avoid enemies and find your way deeper into the depths.          ║
        ║                                                                     ║
        ║  SYMBOLS:                                                           ║
        ║    @            - You (the player)                                  ║
        ║    *            - Gold                                              ║
        ║    S/H/A        - Potions (Strength, Health, Armour)                ║
        ║    /, ;         - Weapons (Sword, axe)                              ║
        ║    G/O/T        - Enemies (Goblin, Orc, Troll)                      ║
        ║    . / # / +    - Walkables (Floor, Tunnel, Door)                   ║
        ║    | - / walls  - Walls (not walkable)                              ║
        ║    >            - Staricase (Touch to go to next lvl)               ║
        ║    &            - The amulet (Touch to win)                         ║                                              
        ║                                                                     ║
        ║  Press H or ESCAPE to return to the game...                         ║
        ║                                                                     ║
        ╚═════════════════════════════════════════════════════════════════════╝

        """";

    public HelpScene(Game game, Scene previousScene)
    {
        _game = game;
        _previousScene = previousScene;
        _levelActive = true;

        // Register a command to return (any key will do, but let's make it explicit)
        RegisterCommand(ConsoleKey.H, "return");
        RegisterCommand(ConsoleKey.Escape, "return");
        RegisterCommand(ConsoleKey.Spacebar, "return");
        RegisterCommand(ConsoleKey.Enter, "return");
    }

    public override void Draw(IRenderWindow disp)
    {
        // Draw the help text
        disp.Draw(_helpText, ConsoleColor.Cyan);
    }

    public override void DoCommand(Command command)
    {
        if (command.Name == "return")
        {
            // Return to the previous scene (the level)
            _levelActive = false;
            if (_game != null && _previousScene != null)
            {
                _game.CurrentLevel = _previousScene;
            }
        }
    }

    public override void Update()
    {
        // Help scene doesn't need updates
    }
}