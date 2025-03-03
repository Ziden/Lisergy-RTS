using Game;

namespace WebPlayerLogic
{
    public class CommandContext
    {
        public string PlayerId;
        public LisergyGame Game;
    }

    public class SetupPlayerCommand
    {
        public async Task Run(CommandContext context)
        {

            //var tile = 
            //context.Game.EntityLogic().Player.PlaceNewPlayer()

            //var dg = context.Game.Entities.CreateEntity(GameId.ZERO, EntityType.DeepDungeon);
            //var tavern = context.Game.Entities.CreateEntity(GameId.ZERO, EntityType.Tavern);
            //var c = tavern.Get<TavernComponent>();
            //c.Bartender = new Unit(context.Game.Specs.InitialUnit);
            //tavern.Save(c);
        }
    }
}

