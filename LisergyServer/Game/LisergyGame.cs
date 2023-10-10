
using Game.ECS;
using Game.Events;
using Game.Events.Bus;
using Game.Network;
using Game.Scheduler;
using Game.Systems.Player;
using Game.World;
using GameData;

namespace Game
{
    public interface IGame
    {
        public GameSpec Specs { get; }
        public IGameNetwork Network { get; }
        public IGameScheduler Scheduler { get; }
        public IGameEntities Entities { get; }
        public IGamePlayers Players { get; }
        public IGameWorld GameWorld { get; }
        public ISystems Systems { get; }
        public IGameLogic Logic { get; }
        public EventBus<IBaseEvent> Events { get; }
    }

    public class LisergyGame : IGame
    {
        public GameSpec Specs { get; private set; }
        public GameWorld World { get; private set; }
        public IGameScheduler Scheduler { get; private set; }
        public IGameEntities Entities { get; private set; }
        public IGameWorld GameWorld => World;
        public ISystems Systems { get; private set; }
        public IGamePlayers Players => World.Players;
        public IGameLogic Logic { get; private set; }
        public IEntityLogic EntityLogic(IEntity e) => Logic.GetEntityLogic(e);
        public IGameNetwork Network { get; private set; }
        public EventBus<IBaseEvent> Events { get; private set; } = new EventBus<IBaseEvent>();

        public LisergyGame(GameSpec specs)
        {
            Specs = specs;
        }

        public void SetWorld(GameWorld world)
        {
            Network = new GameNetwork(this);
            Entities = new GameEntities(this);
            Systems = new GameSystems(this);
            Logic = new GameLogic(Systems);
            Scheduler = new GameScheduler();
            world.Game = this;
            World = world;
        }
    }
}
