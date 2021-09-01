using System;
using System.Collections.Generic;
using System.Text;

namespace Game.BlockChain
{
    public class TestChain : IChain
    {
        private Dictionary<string, PlayerEntity> _players = new Dictionary<string, PlayerEntity>();
        private Dictionary<string, Unit> _units = new Dictionary<string, Unit>();

        public void CreateUnit(Unit u)
        {
            _units[u.Id] = u;
        }

        public Unit GetUnit(string id)
        {
            Unit p;
            _units.TryGetValue(id, out p);
            return p;
        }

        public PlayerEntity GetPlayer(string userId)
        {
            PlayerEntity p;
            _players.TryGetValue(userId, out p);
            return p;
        }


        public void UpdatePlayer(PlayerEntity player)
        {
            _players[player.UserID] = player;
        }
    }
}
