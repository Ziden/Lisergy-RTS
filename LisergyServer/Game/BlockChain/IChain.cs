using System;
using System.Collections.Generic;
using System.Text;

namespace Game.BlockChain
{
    public interface IChain
    {
        PlayerEntity GetPlayer(string userId);

        void UpdatePlayer(PlayerEntity player);

        void CreateUnit(Unit u);

        Unit GetUnit(string id);
    }
}
