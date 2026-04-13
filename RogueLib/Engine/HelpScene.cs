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

    private string _helpText = """
        ╔══════════════════════════════════════════════════════════════════════════════╗
        ║                              ROGUE HELP MANUAL                               ║
        ╠══════════════════════════════════════════════════════════════════════════════╣
        ║                                                                              ║
        ║  MOVEMENT:                                                                   ║
        ║    Arrow Keys  - Move in cardinal directions                                 ║
        ║    W/A/S/D     - Alternative movement controls                               ║
        ║                                                                              ║
        ║  COMMANDS:                                                                   ║
        ║    H            - Show this help screen                                      |
        ║    R            - Rest (press 5 times to recover 1 HP                        |
        ║    Q            - Quit the game                                              ║
        ║                                                                              ║
        ║  OBJECTIVE:                                                                  ║
        ║    Navigate the dungeon, collect gold, and survive!                          ║
        ║    Avoid enemies and find your way deeper into the depths.                   ║
        ║                                                                              ║
        ║  SYMBOLS:                                                                    ║
        ║    @            - You (the player)                                           ║
        ║    *            - Gold                                                       |
        ║    g            - Goblin (weak, fast)                                        |
        |    o            - Orc (medium, chases when close)                            |
        |    T            - Troll (slow, always chases)                                |
        ║    .            - Floor (walkable)                                           ║
        ║    #            - Tunnel (walkable)                                          ║
        ║    +            - Door (walkable)                                            ║
        ║    | - / walls  - Walls (not walkable)                                       ║
        ║                                                                              ║
        ║  Press ANY KEY to return to the game...                                      ║
        ║                                                                              ║
        ╚══════════════════════════════════════════════════════════════════════════════╝
        """;

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