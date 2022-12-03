using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UltraNuke.Saga.Domain.Actor;

namespace ZCITC.Demo.Domain.AggregatesModel
{
    public class Stage
    {
        public IList<Thing> objects = new List<Thing>();
        public IList<Place> places = new List<Place>();
        public IList<Person> actors = new List<Person>();

        public int elapsed_time = 0;
        public int current_scene = 0;

        public Thing find(string name)
        {
            var t = objects.Where(w => w.name == name).FirstOrDefault();
            return t;
        }
    }
}
