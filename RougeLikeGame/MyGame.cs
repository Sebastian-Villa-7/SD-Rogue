using RogueLib.Dungeon;
using RogueLib.Engine;
using RogueLib.Utilities;
using SandBox01.Levels;

namespace RlGameNS;


public class MyGame : Game
{

    private void init()
    {
        // To create a new game just need to 
        // 'inject' an IRenderWindow to draw the game one
        // 'inject' a Player, the player lives outside or the Scene's because the 
        // player visits all the scenes and takes their inventory with them. 
        // you must load the first level, and your level or your game must manage 
        // the level switching. 

        Console.WindowWidth = 78;
        Console.WindowHeight = 40;

        _window = new ScreenBuff(78, 40);
        _player = new Rogue();
        _currentLevel = new Level(_player, this);

    }

    public MyGame()
    {
        // init level on construction 
        init();
    }

}