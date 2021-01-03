using Game.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.World
{
    public class ClientParty : Party
    {
        public ClientParty(Party p) : base(p.Owner, p.PartyID)
        {
            _x = (ushort)p.X;
            _y = (ushort)p.Y;
            for (var x = 0; x < p.Units.Length; x++)
                Units[x] = p.Units[x] == null ? null : new ClientUnit(p.Units[x]);
            StackLog.Debug($"Created new party instance {this}");
            this.Owner = MainBehaviour.StrategyGame.GetWorld().GetOrCreateClientPlayer(p.OwnerID);
        }

        public void Render()
        {
            foreach (var unit in this.Units)
                if (unit != null)
                    ((ClientUnit)unit).Render(X, Y);
        }


    }
}
