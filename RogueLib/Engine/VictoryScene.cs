using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.Engine;

public class VictoryScene : Scene
{
    private Game? _game;

    private string _victoryText = """
        ╔═════════════════════════════════════════════════════════════════════╗
        ║                                                                     ║
        ║                            VICTORY!                                 ║
        ║                                                                     ║
        ║                   You have claimed the Amulet                       ║
        ║                    and escaped the dungeon!                         ║
        ║                                                                     ║
        ║                                                                     ║
        ║                            You Win!                                 ║
        ║                                                                     ║
        ║                                                                     ║
        ║                    Press Q to quit the game...                      ║
        ║                                                                     ║
        ╚═════════════════════════════════════════════════════════════════════╝
        """;

    public VictoryScene(Game game, Scene previousScene)
    {
        _game = game;
        _levelActive = true;
        RegisterCommand(ConsoleKey.Q, "quit");
        RegisterCommand(ConsoleKey.Escape, "quit");
    }

    public override void Draw(IRenderWindow disp)
    {
        disp.Draw(_victoryText, ConsoleColor.Yellow);
    }

    public override void DoCommand(Command command)
    {
        if (command.Name == "quit")
            _levelActive = false;
    }

    public override void Update() { }
}