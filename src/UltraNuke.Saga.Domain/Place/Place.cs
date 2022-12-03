using System;
using System.Collections.Generic;
using System.Text;

namespace ZCITC.Demo.Domain.AggregatesModel
{
    public class Place : Thing
    {
        public bool is_openable = false;
        public bool is_open = true;

        public Place(string name, Stage stage): base(name, stage)
        {            
            stage.places.Add(this);
        }
    }
}
