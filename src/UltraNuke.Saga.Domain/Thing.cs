using System;
using UltraNuke.Saga.Domain.Actor;

namespace ZCITC.Demo.Domain.AggregatesModel
{
    /// <summary>
    /// 具有名称的对象
    /// 未实现 __repr__，__str__
    /// </summary>
    public class Thing
    {
        public Thing location;
        public string name;
        public string preposition;

        /// <summary>
        /// 去掉Python中preposition
        /// </summary>
        public Thing(string name, Stage stage)
        {
            stage.objects.Add(this);
            this.name = name;
        }

        /// <summary>
        /// 将对象从当前位置移动到新位置。
        /// </summary>
        public void move_to(Thing place)
        {
            location = place;
        }

        public string status()
        {
            if (location!=null && !(location is Person))
            {
                return $"{name}在{location.name}上。";
            }
            return string.Empty;
        }
    }
}
