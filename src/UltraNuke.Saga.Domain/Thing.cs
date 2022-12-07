namespace UltraNuke.Saga.Domain;

/// <summary>
/// 具有名称的对象
/// 未实现 __repr__，__str__
/// </summary>
public class Thing
{
    public Thing(Stage stage, string name, string preposition = "on")
    {
        stage.objects.Add(this);
        this.name = name;
        this.preposition = preposition;
    }

    public Thing location { set; get; }
    public string name { set; get; }
    public string preposition { set; get; }

    /// <summary>
    /// 将对象从当前位置移动到新位置。
    /// </summary>
    public void move_to(Thing place)
    {
        location = place;
    }

    public string status()
    {
        if (location != null && !(location is Person))
        {
            return $"{name}在{location.name}上。";
        }
        return string.Empty;
    }
}
