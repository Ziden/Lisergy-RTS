using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    public class WorldPlayers
    {
        public int MaxPlayers { get; private set; }

        private Dictionary<GameId, PlayerEntity> _players = new Dictionary<GameId, PlayerEntity>();

        public WorldPlayers(int maxPlayers)
        {
            MaxPlayers = maxPlayers;
        }

        public void Add(PlayerEntity p)
        {
            if (_players.ContainsKey(p.UserID))
                return;

            if(_players.Count >= MaxPlayers)
            {
                throw new Exception("Max player limit reached");
            }
            _players[p.UserID] = p;
        }

        public int PlayerCount { get => _players.Count; }

        public bool GetPlayer(GameId id, out PlayerEntity player)
        {
            if (id == GameId.ZERO)
            {
                player = null;
                return false;
            }
            return _players.TryGetValue(id, out player);
        }

        public PlayerEntity GetPlayer(GameId id)
        {
            PlayerEntity player = null;
            _players.TryGetValue(id, out player);
            return player;
        }
    }
}
