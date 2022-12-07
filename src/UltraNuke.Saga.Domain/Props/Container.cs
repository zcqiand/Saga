namespace UltraNuke.Saga.Domain.Props;

/// <summary>
/// 容器
/// </summary>
public class Container : Thing
{
    public int volume = 0;

    public Container(Stage stage, string name) : base(stage, name) { }

    public bool full
    {
        get
        {
            return volume > 0;
        }
    }

    /// <summary>
    /// 倒
    /// 从满的容器倒入空的容器会使另一个容器充满。
    /// 它不会减少源容器的数量。
    /// 如果源容器为空，则返回False。 如果倒入成功，则返回True。
    /// </summary>
    /// <param name=""></param>
    public bool pour(Container new_container)
    {
        if (full)
        {
            Tool.print($"从{name}倒酒");
            new_container.volume = 3;
            return true;
        }
        return false;
    }
    /// <summary>
    /// 喝
    /// 从满满的容器中喝酒会改变演员的醉酒状态。 
    /// 从空杯子喝酒没有任何作用。 
    /// 如果饮料成功，则返回True。
    /// </summary>
    /// <returns></returns>
    public bool drink(Person actor)
    {
        if (full)
        {
            Tool.print($"从{name}喝一口酒");
            actor.inebriation += 1;
            volume -= 1;
            return true;
        }
        return false;
    }
}
