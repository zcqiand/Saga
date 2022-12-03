using System;
using System.Collections.Generic;
using System.Text;

namespace ZCITC.Demo.Domain.AggregatesModel
{
    public class Door : Place
    {
        public bool is_openable = true;
        public bool is_open = false;

        public Door(string name, Stage stage) : base(name, stage) { }

        public void close()
        {
            Console.WriteLine("关门");
            is_open = false;
        }
        public void open()
        {
            Console.WriteLine("开门");
            is_open = true;
        }
    }
}
