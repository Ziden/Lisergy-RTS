
using Game.Engine;
using Game.Engine.Events;
using Game.Engine.Events.Bus;
using Game.Engine.Scheduler;
using Game.Entities;
using Game.World;
using GameData;
using System;

namespace Game
{
    public interface IGame
    {
        public GameSpec Specs { get; }
        public IGameNetwork Network { get; }
        public IGameScheduler Scheduler { get; }
        public GameEntities Entities { get; }
        public IGamePlayers Players { get; }
        public IGameWorld World { get; }
        public IGameLogic Logic { get; }
        public EventBus<IBaseEvent> Events { get; }
        public IGameLog Log { get; }
        public DateTime GameTime { get; }
        public bool IsClientGame { get; }
    }

    public class LisergyGame : IGame
    {
        public GameSpec Specs { get; private set; }
        public IGameWorld World { get; private set; }
        public IGameScheduler Scheduler { get; private set; }
        public GameEntities Entities { get; private set; }
        public IGamePlayers Players => World.Players;
        public IGameLogic Logic { get; private set; }
        public IGameNetwork Network { get; private set; }
        public IGameLog Log { get; private set; }
        public EventBus<IBaseEvent> Events { get; private set; } = new EventBus<IBaseEvent>();
        public DateTime GameTime => Scheduler.Now;
        public bool IsClientGame { get; private set; }

        public LisergyGame(GameSpec specs, IGameLog log, IGameNetwork network = null, bool isClientGame = false)
        {
            IsClientGame = isClientGame;
            Specs = specs;
            Log = log;
            Network = network ?? new GameServerNetwork(this);
            Entities = new GameEntities(this);
            Logic = new GameLogic(this);
            Scheduler = new GameScheduler();
            if (IsClientGame)
            {
                Network.DeltaCompression.Enabled = false;
                Log.Info("Disabled delta compression");
            }
        }

        public void SetupWorld(GameWorld world)
        {
            world.Game = this;
            World = world;
            World.Populate();
            Network.SetupGame(world.Game);
            Network.DeltaCompression.ClearDeltas();

            Log.Debug($"World {World.TilemapDimensions.x}x{World.TilemapDimensions.y} ready");
        }
    }
}
