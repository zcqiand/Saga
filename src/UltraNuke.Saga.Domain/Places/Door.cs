namespace UltraNuke.Saga.Domain.Places;

/// <summary>
/// 门是可以打开或关闭的地方。 如果它是开放的，当演员穿过它时，我们会打印一条与普通地方不同的消息
/// </summary>
public class Door : Place
{
    public Door(Stage stage, string name) : base(stage, name) { }

    public bool is_open { set; get; } = false;
    public bool is_openable { set; get; } = true;

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
