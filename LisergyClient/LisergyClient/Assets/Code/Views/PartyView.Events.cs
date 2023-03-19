using Game;
using Game.Events.GameEvents;

namespace Assets.Code.World
{
    public partial class PartyView 
    {
        public void RegisterEvents()
        {
            Entity.Components.RegisterExternalComponentEvents<PartyView, EntityMoveInEvent>(OnMoveIn);
        }

        private static void OnMoveIn(PartyView view, EntityMoveInEvent ev)
        {
            Log.Debug("MOVE IN");
            if(!view.Entity.BattleLogic.BattleID.IsZero())
            {
                Effects.BattleEffect(ev.ToTile);
            }
            ClientEvents.PartyFinishedMove(view.Entity, ev.FromTile, ev.ToTile);
        }

        /*
        public override GameId BattleID
        {
            get => base.BattleID; set {

                if(this.Tile != null && !value.IsZero() && BattleID.IsZero())
                    Effects.BattleEffect(this.Tile);
                if (!this.BattleID.IsZero() && value.IsZero())
                    Effects.StopEffect(this.Tile);
                base.BattleID = value;
            }
        }
        */

    }
}
