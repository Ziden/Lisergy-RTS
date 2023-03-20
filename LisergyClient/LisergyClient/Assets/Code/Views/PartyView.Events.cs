using Assets.Code.Entity;
using Game;
using Game.Battler;
using Game.DataTypes;
using Game.Events.GameEvents;
using Game.Party;
using UnityEngine;

namespace Assets.Code.World
{
    public partial class PartyView 
    {
        public void RegisterEvents()
        {
            Entity.Components.RegisterExternalComponentEvents<PartyView, EntityMoveInEvent>(OnMoveIn);
            Entity.BattleGroupLogic.OnBattleIdChanged += OnBattleIdChanged;
        }

        private void OnBattleIdChanged(IBattleComponentsLogic logic, GameId battleId)
        {
            if (battleId.IsZero()) EntityEffects<PartyEntity>.StopEffects(Entity);
            else EntityEffects<PartyEntity>.BattleEffect(Entity);
        }

        private static void OnMoveIn(PartyView view, EntityMoveInEvent ev)
        {
            if(view.Entity.IsMine())
            {
                ClientEvents.PartyFinishedMove(view.Entity, ev.FromTile, ev.ToTile);
            }
            
        }
    }
}
