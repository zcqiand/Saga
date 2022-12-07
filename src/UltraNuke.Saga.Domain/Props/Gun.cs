namespace UltraNuke.Saga.Domain.Props;

public class Gun : Thing
{
    public int num_bullets = 0;

    public Gun(Stage stage, string name) : base(stage, name)
    {
        num_bullets = Tool.DEFAULT_NUM_BULLETS;
    }
}
