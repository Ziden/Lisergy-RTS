using Game;
using Game.Entity;

namespace Assets.Code.World
{
    public class ClientParty : Party
    {
        public ClientTile ClientTile { get => (ClientTile)this.Tile; }

        public ClientParty(PlayerEntity owner, Party partyFromNetwork) : base(owner, partyFromNetwork.PartyIndex)
        {
            _x = (ushort)partyFromNetwork.X;
            _y = (ushort)partyFromNetwork.Y;
            foreach (var unit in partyFromNetwork.GetUnits())
                    this.AddUnit(new ClientUnit(owner, unit));
            Render();
            StackLog.Debug($"Created new party instance {this}");
        }

        public bool IsMine()
        {
            return this.OwnerID == MainBehaviour.Player.UserID;
        }

        public override void AddUnit(Unit u)
        {
            base.AddUnit(u);
        }

        public void Render()
        {
            foreach(var unit in GetUnits())
                ((ClientUnit)unit).Render(X, Y);
        }


    }
}
