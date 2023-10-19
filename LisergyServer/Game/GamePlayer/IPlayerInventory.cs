using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Game.GamePlayer
{
    public interface IPlayerInventory
    {
        public Task<IReadOnlyDictionary<int, int>> GetItems();
        public Task AddItem(int id, int amt);
        public Task<bool> ConsumeItem(int id, int amt);
    }

}
