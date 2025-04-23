using Game.Engine.DataTypes;
using Game.Engine.ECLS;

namespace Game.Systems.Player
{
    /// <summary>
    ///     Wraps a player entity
    public class PlayerModel
	{
		public PlayerModel(IGame game, IEntity e)
		{
			Game = game;
			PlayerEntity = e;
		}

		public IEntity PlayerEntity { get; protected set; }

		public GameId EntityId => GetFromEntity<PlayerProfileComponent>().PlayerId;
		public IGame Game { get; }

		public ComponentSet Components => PlayerEntity.Components;
		public PlayerLogic EntityLogic => Game.Logic.GetEntityLogic(PlayerEntity).Player;

		public override string ToString()
		{
			return $"<Player id={EntityId}>";
		}

		public T GetFromEntity<T>() where T : IComponent
		{
			return PlayerEntity.Components.Get<T>();
		}

		public void SaveInEntity<T>(in T c) where T : IComponent
		{
			PlayerEntity.Components.Save(c);
		}
	}
}