using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    public class WorldPlayers
    {
        public int MaxPlayers { get; private set; }

        private Dictionary<string, PlayerEntity> _players = new Dictionary<string, PlayerEntity>();

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

        public bool GetPlayer(string id, out PlayerEntity player)
        {
            if (id == null)
            {
                player = null;
                return false;
            }
            return _players.TryGetValue(id, out player);
        }

        public PlayerEntity GetPlayer(string id)
        {
            PlayerEntity player = null;
            _players.TryGetValue(id, out player);
            return player;
        }
    }
}
