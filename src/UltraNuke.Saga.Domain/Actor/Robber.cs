using System;
using System.Collections.Generic;
using System.Text;
using UltraNuke.Saga.Domain.Actor;

namespace ZCITC.Demo.Domain.AggregatesModel
{
    /// <summary>
    /// 强盗
    /// </summary>
    public class Robber : Person
    {
        public Robber(string name, Stage stage) : base(name, stage)
        {
        }

        public int initiative()
        {
            var actor_initiative = base.initiative();

            //如果强盗有钱而警察还活着，强盗想把钱丢到角落里
            if(get_if_held("钱")!=null && enemy.is_alive)
            {
                actor_initiative += Tool.HIGH_INITIATIVE;
            }

            Tool.debug($"{name}is returning initiative{actor_initiative}");

            return actor_initiative;
        }

        public bool act()
        {
            if(location.name == "角落" && get_if_held("钱")!=null && enemy.is_alive)
            {
                var money = get_if_held("钱");
                drop(money, location);
                return true;
            }
            return base.act();
        }

        public int starting_hit_weight()
        {
            //如果他喝醉了，强盗（但不是警长）是一个更好的选择
            return inebriation + 2;
        }
    }
}
