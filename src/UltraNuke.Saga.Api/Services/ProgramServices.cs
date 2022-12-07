using UltraNuke.Saga.Domain;
using UltraNuke.Saga.Domain.Actors;
using UltraNuke.Saga.Domain.Places;
using UltraNuke.Saga.Domain.Props;

namespace UltraNuke.Saga.Api.Services;

public class ProgramServices
{
    private readonly ILogger _logger;
    private Stage stage;

    public ProgramServices(ILogger logger)
    {
        _logger = logger;
        stage = new Stage();
    }

    public void Main()
    {
        var delay = Tool.DEFAULT_SHERIFF_DELAY;
        for (int i = 0; i < Tool.MAX_SCENES; i++)
        {
            init(delay);
        }
    }

    /// <summary>
    /// 对于每个演员，找出下一个要移动的人
    /// </summary>
    /// <param name="actors"></param>
    /// <returns></returns>
    public Person check_initiative(IList<Person> actors)
    {
        return actors.OrderByDescending(s => s.initiative()).First();
    }

    public Person action(Person actor)
    {
        if (actor is Sheriff)
        {
            actor = (Sheriff)actor;
        }
        else if (actor is Robber)
        {
            actor = (Robber)actor;
        }
        Tool.debug($"Starting action for actor {actor.name}");
        actor.set_starting_location(actor.default_location);
        actor.act();

        stage.elapsed_time += 1;

        //确定下一步行动
        var next_actor = check_initiative(stage.actors);
        if (next_actor.escaped)
        {
            return next_actor;
        }

        //如果是同一位演员，请再次致电
        if (next_actor == actor)
        {
            return action(actor);
        }

        return next_actor;
    }

    public void init(int delay)
    {
        //# Humans
        var robber = new Robber(stage, "强盗");
        var robber_gun = new Gun(stage, "枪");
        robber_gun.move_to(robber.right_hand);
        var money = new Thing(stage, "钱");
        money.move_to(robber.left_hand);
        var robber_holster = new Holster(stage, "皮衣");
        robber_holster.move_to(robber.body);
        robber.stage = stage;  //掌握世界状态的机制

        var sheriff = new Sheriff(stage, "警察", delay);
        var sheriff_gun = new Gun(stage, "警枪");
        sheriff_gun.move_to(sheriff.right_hand);
        var holster = new Holster(stage, "警服");
        holster.move_to(sheriff.body);

        sheriff.stage = stage;
        robber.enemy = sheriff;
        sheriff.enemy = robber;

        //# 
        var window = new Place(stage, "窗口");
        var table = new Place(stage, "桌子");
        var door = new Door(stage, "门");
        var corner = new Place(stage, "角落");

        sheriff.default_location = door;
        robber.path.Add("角落");
        robber.default_location = window;
        robber.path.Add("门");
        robber.path.Add("角落");

        //# Objects
        var glass = new Container(stage, "酒杯");
        var bottle = new Container(stage, "酒瓶");
        bottle.volume = 10;
        glass.move_to(table);
        bottle.move_to(table);

        stage.current_scene += 1;

        loop();
    }

    public void loop()
    {
        Tool.print($"***场景{stage.current_scene}***");
        foreach (var obj in stage.objects)
        {
            if (!(obj is Person) && !string.IsNullOrEmpty(obj.status()))
            {
                Tool.print(obj.status());
            }
        }
        var next_actor = stage.actors[0];
        while (true)
        {
            Tool.print("");
            Tool.print($"***{next_actor.name}***");
            next_actor = action(next_actor);
            if (next_actor.escaped)
            {
                Tool.print("");
                Tool.print($"***拉幕***");
                break;
            }
        }
    }

}
