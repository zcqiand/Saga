using System;
using System.Collections.Generic;
using System.Text;

namespace ZCITC.Demo.Domain.AggregatesModel
{
    public class Gun : Thing
    {
        public int num_bullets = 0;

        public Gun(string name, Stage stage) : base(name, stage)
        {
            num_bullets = Tool.DEFAULT_NUM_BULLETS;
        }
    }
}
