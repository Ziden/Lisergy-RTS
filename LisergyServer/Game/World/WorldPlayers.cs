using Game.Engine.DataTypes;
using Game.Systems.Player;
using System;
using System.Collections.Generic;

namespace Game.World
{
    public interface IGamePlayers
    {
        bool GetPlayer(GameId id, out PlayerModel player);
        public void Add(PlayerModel player);
        PlayerModel GetPlayer(GameId id);
        public int MaxPlayers { get; }
        public int PlayerCount { get; }
        public PlayerModel this[GameId id] => GetPlayer(id);
        public IReadOnlyCollection<PlayerModel> AllPlayers();
    }

    /// <summary>
    /// Represents the list of players of a given world
    /// </summary>
    public class WorldPlayers : IGamePlayers
    {
        public int MaxPlayers { get; private set; }

        protected Dictionary<GameId, PlayerModel> _players;

        public WorldPlayers(int maxPlayers)
        {
            MaxPlayers = maxPlayers;
            _players = new Dictionary<GameId, PlayerModel>();
        }

        public void Free()
        {
            _players.Clear();
        }

        public void Add(PlayerModel p)
        {
            if (_players.ContainsKey(p.EntityId))
                return;

            if (_players.Count >= MaxPlayers)
            {
                throw new Exception("Max player limit reached");
            }
            _players[p.EntityId] = p;
        }

        public int PlayerCount { get => _players.Count; }

        public virtual bool GetPlayer(GameId id, out PlayerModel player)
        {
            if (id == GameId.ZERO)
            {
                player = null;
                return false;
            }
            return _players.TryGetValue(id, out player);
        }

        public virtual PlayerModel GetPlayer(GameId id)
        {
            _players.TryGetValue(id, out var player);
            return player;
        }

        public IReadOnlyCollection<PlayerModel> AllPlayers()
        {
            return _players.Values;
        }
    }
}
