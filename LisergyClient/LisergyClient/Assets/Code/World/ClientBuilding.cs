using Assets.Code.Views;
using Game;
using Game.ECS;
using GameData;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientBuilding : Building, IGameObject, IClientEntity<Building, ClientBuilding>
    {
        public GameObject GameObject { get; set; }

        public ClientBuilding(PlayerEntity owner): base(owner)
        {
           
        }

        public ClientBuilding UpdateData(Building building)
        {
            this.SpecID = building.SpecID;
            this.Id = building.Id;
            this.Tile = GameView.World.GetTile(building);
            return this;
        }

        public void InstantiateInScene(Building serverEntity)
        {
            var prefab = Resources.Load("prefabs/buildings/" + serverEntity.SpecID);
            var tile = GameView.World.GetTile(serverEntity);
            var tileView = GameView.GetView<TileView>(tile);
            GameObject = MainBehaviour.Instantiate(prefab, tileView.GameObject.transform) as GameObject;
            GameObject.transform.localPosition = Vector3.zero;
            this.GameObject.SetActive(true);
            UpdateData(serverEntity);
            if (serverEntity.SpecID == StrategyGame.Specs.InitialBuilding && this.IsMine())
                CameraBehaviour.FocusOnTile(tile);
        }

        public override BuildingSpec GetSpec()
        {
            return StrategyGame.Specs.Buildings[this.SpecID];
        }

        public bool IsMine()
        {
            return Owner == MainBehaviour.Player;
        }
    }
}
