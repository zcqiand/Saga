namespace UltraNuke.Saga.Domain.Actors;

/// <summary>
/// 警察
/// 警察想杀死强盗并带走钱。
/// 他没有喝酒奖金，并且延迟到达。
/// </summary>
public class Sheriff : Person
{
    public int delay;

    public Sheriff(Stage stage, string name, int delay) : base(stage, name)
    {
        this.delay = delay;
    }

    public int initiative()
    {
        var actor_initiative = base.initiative();

        if (stage.elapsed_time < delay)
        {
            actor_initiative = 0;
        }
        else if (location == null)
        {
            actor_initiative += Tool.HIGH_INITIATIVE;
        }

        Tool.debug($"{name}is returning initiative{actor_initiative}");

        return actor_initiative;
    }

    //警察想马上进屋
    public bool act()
    {
        if (location == null)
        {
            path.Clear();
            path.Add("窗口");
            path.Add("门");
            //path = new[] { "窗口", "门" };
        }
        return base.act();
    }

    /// <summary>
    /// 警察（但不是强盗）如果受伤了会更好
    /// </summary>
    /// <returns></returns>
    public int starting_hit_weight()
    {
        var weight = 1;
        if (health < Tool.DEFAULT_HEALTH)
        {
            weight += 3;
        }
        return weight;
    }
}
