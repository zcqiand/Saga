using ZCITC.Demo.Domain.AggregatesModel;

namespace UltraNuke.Saga.Domain.Actor;

public class Person : Thing
{
    public Stage stage;
    public Person enemy;
    public Thing default_location;
    public int health = 0;  //-1已经死了，但是我们会在初始化时恢复它们
    public bool is_dead = false;
    public int inebriation = 0;
    public IList<string> path = new List<string>();//该人当前正在走的地方的路径
    public Queue<(string, object, object[])> queue = new Queue<(string, object, object[])>();//下一个要调用的函数队列
    public Thing right_hand;
    public Thing left_hand;
    public Thing body;
    public IList<Thing> parts = new List<Thing>();
    public bool escaped;//最终的比赛状态


    public Person(string name, Stage stage) : base(name, stage)
    {
        health = Tool.DEFAULT_HEALTH;
        //self.path = []  # A path of Places the person is currently walking
        //self.queue = []  # A queue of functions to call next
        right_hand = new Thing($"{name}的右手", stage);
        left_hand = new Thing($"{name}的左手", stage);
        body = new Thing($"{name}的身体", stage);
        parts.Add(left_hand);
        parts.Add(right_hand);
        parts.Add(body);
        escaped = false;
        stage.actors.Add(this);
    }

    public bool is_alive
    {
        get
        {
            return health > 0;
        }
    }

    /// <summary>
    /// 返回一个值，该值表示此参与者根据自己的状态要执行多少操作
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public int initiative()
    {
        if (is_dead)  //如果他们已经死了，他们就会缺乏主动权
        {
            return -9999;
        }

        if (health < 0)  //如果他们死了，给他们巨大的主动奖金，以便我们“切入”他们的生活
        {
            return 9999;
        }

        var actor_initiative = RandomUtil.Next(0, Tool.DEFAULT_INITIATIVE);
        if (path.Count > 0)  //演员真的很想去某个地方
        {
            actor_initiative += Tool.HIGH_INITIATIVE;
            Tool.debug($"{name} init change for path movement: {Tool.HIGH_INITIATIVE},{actor_initiative}");
        }

        var injury_bonus = Tool.DEFAULT_HEALTH - health;
        actor_initiative += injury_bonus;
        Tool.debug($"{name} init change for injury bonus: {injury_bonus},{actor_initiative}");

        var gun = (Gun)get_if_held(typeof(Gun));
        if (gun != null)
        {
            var bullet_bonus = gun.num_bullets == 1 ? 10 : 0;
            actor_initiative += bullet_bonus;
            Tool.debug($"{name} init change for bullet bonus: {bullet_bonus},{actor_initiative}");
        }

        return actor_initiative > 1 ? actor_initiative : 1;
    }

    /// <summary>
    /// 做下一个排队的事件是什么
    /// </summary>
    public bool act()
    {
        if (health <= 0)
        {
            Console.WriteLine($"{name}死了。");
            var obj = get_held_obj(right_hand);
            if (obj != null)
            {
                drop(obj, location);
            }
            obj = get_held_obj(left_hand);
            if (obj != null)
            {
                drop(obj, location);
            }
            obj = get_held_obj(body);
            if (obj != null)
            {
                drop(obj, location);
            }
            is_dead = true;
            return false;
        }

        //如果有排队事件，请执行事件
        if (queue.Count > 0)
        {
            var (cmdKey, cmdInstance, cmdParam) = queue.Dequeue();
            Tool.cmd(cmdKey, cmdInstance, cmdParam);
            return false;
        }

        //如果有目标位置，请尝试去那里
        if (path.Count > 0)
        {
            Tool.debug("Got a path event, walking it");
            var next_location = stage.places.Where(w => w.name == path[0]).First();
            if (go(next_location))
            {
                path.RemoveAt(0);
            }
            return false;
        }

        Gun gun;
        //如果敌人死了，就拿钱跑
        if (enemy.is_dead)
        {
            gun = (Gun)get_if_held(typeof(Gun));
            var holster = (Holster)get_if_held(typeof(Holster));
            if (gun != null && !(gun.location is Holster && (Holster)gun.location == holster))
            {
                Tool.print("吹灭枪口");
                queue.Enqueue(("drop", this, new object[] { gun, holster }));
                return true;
            }

            Tool.debug("*** Trying to get the money");
            var money = stage.find("钱");
            if (location == money.location)
            {
                return take(money);
            }

            //结束游戏！带着钱逃跑！
            if (get_if_held("钱") != null)
            {
                path.Clear();
                path.Add("门");
                //path = new[] { "门" };
                escaped = true;
            }
        }

        //随机行为
        var weighted_choice = new (string, int)[]
        {
            ("shoot", 3),
            ("drink", 3),
            ("wander", 3),
            ("check", 1),
            ("lean", 1),
            ("count", 1),
            ("drop", 1)
        };

        IList<string> weighted_choice_list = new List<string>();
        foreach (var (name, weight) in weighted_choice)
        {
            for (var i = 0; i < weight; i++)
            {
                weighted_choice_list.Add(name);
            }
        }
        var choice = weighted_choice_list[RandomUtil.Next(0, weighted_choice_list.Count - 1)];

        if (choice == "shoot")
        {
            //如果存在敌人，请尝试杀死他们！
            if (enemy_is_present())
            {
                //如果我们没有枪，那就去找吧！
                if (this is Sheriff)
                {
                    gun = (Gun)stage.find("警枪");
                }
                else
                {
                    gun = (Gun)stage.find("枪");
                }

                if (get_if_held(gun.name) != null)
                {
                    shoot(enemy);
                }
                else
                {
                    if (gun.location is Place)
                    {
                        var target_location = (Place)gun.location;
                        go(target_location);
                        queue.Enqueue(("shoot", this, new object[] { enemy }));
                        queue.Enqueue(("take", this, new object[] { gun }));
                    }
                }
            }
            return false;
        }
        else if (choice == "drink")
        {
            var glass = (Container)stage.find("酒杯");
            if (get_if_held("酒杯") != null)
            {
                if (glass.full)
                {
                    glass.drink(this);
                    return true;
                }
                else
                {
                    var bottle = (Container)stage.find("酒瓶");
                    if (get_if_held("酒瓶") != null)
                    {
                        bottle.pour(glass);
                        queue.Enqueue(("drink", glass, new object[] { this }));
                        queue.Enqueue(("take", this, new object[] { glass }));
                        return true;
                    }
                    else
                    {
                        if (can_reach_obj(bottle))
                        {
                            take(bottle);
                            queue.Enqueue(("drink", glass, new object[] { this }));
                            queue.Enqueue(("take", this, new object[] { glass }));
                            queue.Enqueue(("pour", bottle, new object[] { glass }));
                            return true;
                        }
                    }
                }
            }
            else
            {
                if (can_reach_obj(glass))
                {
                    take(glass);
                    return true;
                }
            }
        }
        else if (choice == "wander")
        {
            return go_to_random_location();
        }
        else if (choice == "check")
        {
            if (get_if_held(typeof(Gun)) != null)
            {
                Tool.print("检查枪");
                return true;
            }

        }
        else if (choice == "count")
        {
            if (can_reach_obj(stage.find("钱")))
            {
                Tool.print("数钱");
                return true;
            }
        }
        else if (choice == "lean")
        {
            if (location == stage.find("窗口"))
            {
                Tool.print("靠在窗户上看");
                return true;
            }

        }
        else if (choice == "drop") //放下不是枪的随机物体
        {
            var obj = get_held_obj(right_hand);
            if (obj != null && !(obj is Gun))
            {
                drop(obj, location);
                return true;
            }
            else
            {
                obj = get_held_obj(left_hand);
                if (obj != null && !(obj is Gun))
                {
                    drop(obj, location);
                    return true;
                }

            }
        }

        //如果我们摔倒了却什么也没做，请再试一次
        return act();
    }

    /// <summary>
    /// 如果人员可以到达相关对象，则为true。 该对象必须直接位于同一位置，或者必须位于该位置的可见支撑物上
    /// </summary>
    /// <param name="obj"></param>
    public bool can_reach_obj(Thing obj)
    {
        if (obj != null)
        {
            if (location == obj.location)
            {
                return true;
            }

            if (obj.location != null && obj.location.location == location)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 尝试拿一个物体。 如果没有可用的手，则放下一个对象并排队拿走该对象。 如果已取走该对象，则返回True；否则如果没有手，则返回False。
    /// </summary>
    /// <param name=""></param>
    public bool take(Thing obj)
    {
        var free_hand = this.free_hand();
        if (free_hand != null)
        {
            Tool.print($"用{free_hand.name}捡起{obj.name}");
            obj.move_to(free_hand);
            return true;
        }
        else
        {
            var hands = new List<Thing>
            {
                right_hand,
                left_hand
            };
            drop(get_held_obj(hands[RandomUtil.Next(0, hands.Count - 1)]), location);
            queue.Enqueue(("take", this, new object[] { obj }));
            return false;
        }
    }

    /// <summary>
    /// 随机去一个不是当前位置的地方
    /// </summary>
    public bool go_to_random_location()
    {
        IList<Place> location_list = new List<Place>();
        foreach (var place in stage.places)
        {
            if (place != this.location && !(place is Door))
            {
                location_list.Add(place);
            }
        }
        var location = location_list[RandomUtil.Next(0, location_list.Count - 1)];
        return go(location);
    }

    /// <summary>
    /// 敌人可见并且可以被射杀吗？
    /// </summary>
    /// <returns></returns>
    public bool enemy_is_present()
    {
        return enemy.location != null && enemy.is_alive;
    }

    /// <summary>
    /// 先射击，再也不问问题
    /// </summary>
    public bool shoot(Person target, bool aimed = false)
    {
        var gun = (Gun)get_if_held(typeof(Gun));
        if (gun != null)
        {
            //通常我们会瞄准然后开火，有时我们会开火
            if (!aimed)
            {
                if (RandomUtil.Next(0, 5) > 1)
                {
                    Tool.print($"瞄准{target.name}");
                    queue.Enqueue(("shoot", this, new object[] { target, true }));
                    return false;
                }
            }
            Tool.print($"向{target.name}开火");
            Tool.debug("{name} is trying to shoot {target.name}");
            var hit_weight = starting_hit_weight();
            if (gun.num_bullets == 1)
            {
                hit_weight += 1;
                if (health < Tool.DEFAULT_HEALTH)
                {
                    hit_weight += 1;
                }
            }

            var weighted_hit_or_miss = new (string, int)[]
            {
                    ("miss", 3),
                    ("nick", 3 * hit_weight),
                    ("hit", 1 * hit_weight)
            };

            IList<string> weighted_hit_or_miss_list = new List<string>();
            foreach (var (name, weight) in weighted_hit_or_miss)
            {
                for (var i = 0; i < weight; i++)
                {
                    weighted_hit_or_miss_list.Add(name);
                }
            }
            var hit_or_nick = weighted_hit_or_miss_list[RandomUtil.Next(0, weighted_hit_or_miss_list.Count - 1)];

            switch (hit_or_nick)
            {
                case "miss":
                    Tool.print($"未打中{target.name}");
                    target.health += 0;
                    break;
                case "nick":
                    Tool.print($"划伤{target.name}");
                    target.health += -1;
                    break;
                case "hit":
                    Tool.print($"击中{target.name}");
                    target.health += -2;
                    break;
            }
            gun.num_bullets -= 1;
            return true;
        }
        return false;
    }

    //返回与状态有关的初始权重，该权重可以增加或减少演员成功射击的可能性。
    public int starting_hit_weight()
    {
        return 1;
    }

    /// <summary>
    /// 去新地点
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    public bool go(Place location)
    {
        if (location.is_openable && !location.is_open)
        {
            var door = (Door)location;
            door.open();
            return false;
        }

        if (location.is_openable && location.is_open)
        {
            var door = (Door)location;
            Tool.print($"经过{door.name}");
            queue.Enqueue(("open", door, null));
        }
        else
        {
            Tool.print($"来到{location.name}");
        }
        this.location = location;
        return true;
    }

    public Thing get_if_held(Type type)
    {
        foreach (var obj in stage.objects)
        {
            if (obj.GetType() == type)
            {
                var part = parts.Where(w => w.name == obj.location.name).FirstOrDefault();
                if (part != null)
                {
                    return obj;
                }
            }
        }
        return null;
    }

    public Thing get_if_held(string obj_name)
    {
        var obj = stage.find(obj_name);
        if (obj != null)
        {
            if (obj.location != null)
            {
                var part = parts.Where(w => w.name == obj.location.name).FirstOrDefault();
                if (part != null)
                {
                    return obj;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 获取给定身体部位持有的对象。 如果身体部位没有任何东西，则返回null
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public Thing get_held_obj(Thing part)
    {
        return stage.objects.Where(w => w.location != null && w.location.name == part.name).FirstOrDefault();
    }

    /// <summary>
    /// 返回不握任何东西的手
    /// </summary>
    public Thing free_hand()
    {
        if (right_hand != null)
        {
            return right_hand;
        }

        if (left_hand != null)
        {
            return left_hand;
        }
        return null;
    }

    /// <summary>
    /// 设置起始位置会更改世界模型，并打印一条明确的消息。 这是幂等的，因此可以安全地循环调用，因为我很懒
    /// </summary>
    /// <param name=""></param>
    public void set_starting_location(Thing location)
    {
        if (location != null && this.location == null)
        {
            this.location = location;
            Tool.print($"({name}在{location.name}。)");
        }
    }

    /// <summary>
    /// 将物体放在某个地方或支撑物体上。 如果演员没有对象，则为空。
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    public void drop(Thing obj, Thing target)
    {
        if (get_if_held(obj.name) != null && target != null)
        {
            Tool.print($"把{obj.name}放在{target.name}上");
            obj.move_to(target);
        }
    }
}
