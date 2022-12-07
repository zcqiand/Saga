namespace UltraNuke.Saga.Domain;

public class Stage
{
    public Stage()
    {
        objects = new List<Thing>();
        places = new List<Place>();
        actors = new List<Person>();
    }

    public int elapsed_time { set; get; } = 0;
    public int current_scene { set; get; } = 0;
    public IList<Thing> objects { set; get; }
    public IList<Place> places { set; get; }
    public IList<Person> actors { set; get; }

    /// <summary>
    /// 在世界中按名称查找一个对象并返回该对象
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Thing Find(string name)
    {
        var t = objects.Where(w => w.name == name).First();
        return t;
    }
}
