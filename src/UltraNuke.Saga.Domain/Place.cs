namespace UltraNuke.Saga.Domain;


/// <summary>
/// Place 从来没有位置，也不会在世界描述中打印出来。
/// </summary>
public class Place : Thing
{
    public Place(Stage stage, string name) : base(stage, name)
    {
        stage.places.Add(this);
    }
    public bool is_open { set; get; } = true;
    public bool is_openable { set; get; } = false;
}
