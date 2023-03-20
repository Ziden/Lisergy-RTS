using Assets.Code.Entity;
using Assets.Code.Views;
using Game;
using Game.Building;
using Game.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.World
{
    public class PlayerBuildingView : EntityView<PlayerBuildingEntity>
    {
        public override bool Instantiated => GameObject != null;
        public override PlayerBuildingEntity Entity { get; }
        public override GameObject GameObject { get; set; }

        public PlayerBuildingView(PlayerBuildingEntity e)
        {
            Entity = e;
        }

        public override void OnUpdate(PlayerBuildingEntity serverEntity, List<IComponent> syncedComponents)
        {
            Entity.Tile = GameView.World.GetTile(serverEntity);
            if (!Instantiated)
            {
                Instantiate();
            }   
        }

        public override void Instantiate()
        {
            var prefab = Resources.Load("prefabs/buildings/" + Entity.SpecID);
            var tile = GameView.World.GetTile(Entity);
            var tileView = GameView.GetOrCreateTileView(tile, true);
            GameObject = MainBehaviour.Instantiate(prefab, tileView.GameObject.transform) as GameObject;
            GameObject.transform.localPosition = Vector3.zero;
            this.GameObject.SetActive(true);
            if (Entity.SpecID == StrategyGame.Specs.InitialBuilding && Entity.IsMine())
                CameraBehaviour.FocusOnTile(tile);
        }
    }
}
