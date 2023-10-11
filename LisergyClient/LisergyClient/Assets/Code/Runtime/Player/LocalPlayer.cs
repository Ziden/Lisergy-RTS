
using Assets.Code.Entity;
using Assets.Code.Views;
using Assets.Code.World;
using Game;
using Game.DataTypes;
using Game.ECS;
using Game.Network;
using Game.Systems.Party;
using Game.Systems.Player;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Code
{
    public class LocalPlayer : PlayerEntity
    {
        public Dictionary<GameId, BaseEntity> KnownEntities = new Dictionary<GameId, BaseEntity>();


        public bool ViewBattles = true;

        public LocalPlayer(GameId id) : base(null)
        {
            
        }

        private void OnPartyUpdated(PartyView view)
        {
            if (view.Entity.IsMine())
            {
                //GetParty(view.Entity.PartyIndex) = view.Entity;
            }
        }

        private void OnBuildingViewUpdated(PlayerBuildingView view)
        {
            if (view.Entity.IsMine() && !Data.Buildings.Any(b => b.EntityId == view.Entity.EntityId))
            {
                Data.Buildings.Add(view.Entity);
                //if (view.Entity.SpecID == Game.Specs.InitialBuilding)
                //    CameraBehaviour.FocusOnTile(view.Entity.Tile);
            }
        }

        public void SetupLocalPlayer()
        {
            ClientEvents.OnPartyViewUpdated += OnPartyUpdated;
            ClientEvents.OnBuildingViewUpdated += OnBuildingViewUpdated;
        }
     
        public BaseEntity GetKnownEntity(GameId id)
        {
            BaseEntity e;
            KnownEntities.TryGetValue(id, out e);
            return e;
        }

        public void KnowAbout(BaseEntity e)
        {
            KnownEntities[e.EntityId] = e;
        }
    }
}
