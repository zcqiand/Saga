using System;
using System.Collections.Generic;
using System.Text;
using UltraNuke.Saga.Domain.Actor;

namespace ZCITC.Demo.Domain.AggregatesModel
{
    public class Tool
    {
        public const int DEFAULT_SHERIFF_DELAY = 20;
        public const int DEFAULT_NUM_BULLETS = 5;
        public const int DEFAULT_HEALTH = 5;
        public const int MAX_SCENES = 350;  //~150 words per scene

        public const int HIGH_INITIATIVE = 30;
        public const int MEDIUM_INITIATIVE = 20;
        public const int DEFAULT_INITIATIVE = 10;



        public static void cmd(string cmdKey, object cmdInstance, object[] cmdParam)
        {
            Tool.debug("Running queued command:{cmdKey}{param}");
            switch (cmdKey)
            {
                case "take":
                    ((Person)cmdInstance).take((Thing)cmdParam[0]);
                    break;
                case "shoot":
                    ((Person)cmdInstance).shoot((Person)cmdParam[0], (bool)cmdParam[1]);
                    break;
                case "drop":
                    ((Person)cmdInstance).drop((Thing)cmdParam[0], (Thing)cmdParam[1]);
                    break;
                case "drink":
                    ((Container)cmdInstance).drink((Person)cmdParam[0]);
                    break;
                case "pour":
                    ((Container)cmdInstance).pour((Container)cmdParam[0]);
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
}
