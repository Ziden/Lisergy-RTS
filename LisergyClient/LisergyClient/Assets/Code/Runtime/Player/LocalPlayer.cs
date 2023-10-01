
using Assets.Code.Entity;
using Assets.Code.Views;
using Assets.Code.World;
using Game;
using Game.DataTypes;
using Game.ECS;
using Game.Network;
using Game.Player;
using System.Collections.Generic;

namespace Assets.Code
{
    public class LocalPlayer : PlayerEntity
    {
        public Dictionary<GameId, WorldEntity> KnownEntities = new Dictionary<GameId, WorldEntity>();

        public bool ViewBattles = true;

        public LocalPlayer(GameId id) : base()
        {
            UserID = id;
        }

        private void OnPartyCreated(PartyView view)
        {
            if (view.Entity.IsMine())
            {
                Parties[view.Entity.PartyIndex] = view.Entity;
            }
        }

        private void OnBuildingViewCreated(PlayerBuildingView view)
        {
            if (view.Entity.IsMine())
            {
                Buildings.Add(view.Entity);
                if (view.Entity.SpecID == StrategyGame.Specs.InitialBuilding)
                    CameraBehaviour.FocusOnTile(view.Entity.Tile);
            }
        }

        public void SetupLocalPlayer()
        {
            EntityListener.OnPartyViewCreated += OnPartyCreated;
            EntityListener.OnBuildingViewCreated += OnBuildingViewCreated;
        }
     
        public WorldEntity GetKnownEntity(GameId id)
        {
            WorldEntity e;
            KnownEntities.TryGetValue(id, out e);
            return e;
        }

        public void KnowAbout(WorldEntity e)
        {
            KnownEntities[e.Id] = e;
        }

        public override bool Online()
        {
            return true;
        }

        public override void Send<T>(T ev) { }
    }
}
