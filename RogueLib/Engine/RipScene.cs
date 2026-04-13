using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.Engine;

public class RipScene : Scene
{
    private Game? _game;

    private string _ripText = """
        ╔═════════════════════════════════════════════╗
        ║                                             ║
        ║                                             ║
        ║               ░░░░░░░░░░░░░░░░              ║
        ║             ░░               ░░             ║
        ║            ░░    R . I . P    ░░            ║
        ║            ░░                 ░░            ║
        ║            ░░  You have died! ░░            ║
        ║            ░░                 ░░            ║
        ║            ░░░░░░░░░░░░░░░░░░░░░            ║
        ║                                             ║
        ║              Press q to quit...             ║
        ║                                             ║
        ╚═════════════════════════════════════════════╝
        """;

    public RipScene(Game game)
    {
        _game = game;
        _levelActive = true;
        RegisterCommand(ConsoleKey.Enter, "quit");
        RegisterCommand(ConsoleKey.Spacebar, "quit");
        RegisterCommand(ConsoleKey.Escape, "quit");
        RegisterCommand(ConsoleKey.Q, "quit");
    }

    public override void Draw(IRenderWindow disp)
    {
        disp.Draw(_ripText, ConsoleColor.Red);
    }

    public override void DoCommand(Command command)
    {
        if (command.Name == "quit")
            _levelActive = false;
    }

    public override void Update() { }
}