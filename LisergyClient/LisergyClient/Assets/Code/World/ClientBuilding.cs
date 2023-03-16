using Game;
using Game.ECS;
using GameData;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientBuilding : Building, IGameObject, IClientEntity<Building, ClientBuilding>
    {
        private GameObject _gameObject;

        public GameObject GetGameObject() => _gameObject;

        public ClientBuilding(PlayerEntity owner): base(owner)
        {
           
        }

        public ClientBuilding UpdateData(Building building)
        {
            this.SpecID = building.SpecID;
            this.Id = building.Id;
            this.Tile = ClientStrategyGame.ClientWorld.GetClientTile(building);
            return this;
        }

        public void InstantiateInScene(Building serverEntity)
        {
            var prefab = Resources.Load("prefabs/buildings/" + serverEntity.SpecID);
            var tile = ClientStrategyGame.ClientWorld.GetClientTile(serverEntity);
            _gameObject = MainBehaviour.Instantiate(prefab, tile.GetGameObject().transform) as GameObject;
            _gameObject.transform.localPosition = Vector3.zero;
            this.GetGameObject().SetActive(true);
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
