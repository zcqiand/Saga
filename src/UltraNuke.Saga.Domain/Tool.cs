using UltraNuke.Saga.Domain.Places;
using UltraNuke.Saga.Domain.Props;

namespace UltraNuke.Saga.Domain;

public class Tool
{
    public const int DEFAULT_SHERIFF_DELAY = 20;
    public const int DEFAULT_NUM_BULLETS = 5;
    public const int DEFAULT_HEALTH = 5;
    public const int MAX_SCENES = 1;  //~150 words per scene

    public const int HIGH_INITIATIVE = 30;
    public const int MEDIUM_INITIATIVE = 20;
    public const int DEFAULT_INITIATIVE = 10;

    public static void cmd(string cmdKey, object cmdInstance, object[] cmdParam)
    {
        debug($"Running queued command:{cmdKey}{@cmdParam}");
        switch (cmdKey)
        {
            case "take":
                if (cmdParam.Length > 0)
                {
                    ((Person)cmdInstance).take((Thing)cmdParam[0]);
                }
                break;
            case "shoot":
                if (cmdParam.Length > 1)
                {
                    ((Person)cmdInstance).shoot((Person)cmdParam[0], (bool)cmdParam[1]);
                }
                break;
            case "drop":
                if (cmdParam.Length > 1)
                {
                    ((Person)cmdInstance).drop((Thing)cmdParam[0], (Thing)cmdParam[1]);
                }
                break;
            case "drink":
                if (cmdParam.Length > 0)
                {
                    ((Container)cmdInstance).drink((Person)cmdParam[0]);
                }
                break;
            case "pour":
                if (cmdParam.Length > 0)
                {
                    ((Container)cmdInstance).pour((Container)cmdParam[0]);
                }
                break;
            case "open":
                ((Door)cmdInstance).open();
                break;
        }
    }

    public static void debug(string s)
    {
        //Console.BackgroundColor = ConsoleColor.Blue;
        //Console.ForegroundColor = ConsoleColor.White;
        //Console.WriteLine(s);
        //Console.ResetColor();
    }

    public static void print(string s)
    {
        Console.WriteLine(s);
    }
}
