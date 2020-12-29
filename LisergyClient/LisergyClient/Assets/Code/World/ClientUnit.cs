using Game;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientUnit : Unit
    {
        public bool HaveInfo = false;

        private static GameObject _unitsNode;
        private GameObject _unitObj;

        public ClientUnit(Unit u) : base(u.SpecID, null, u.Id)
        {
            this.Owner = FindOwner(u);
            StackLog.Debug($"Created new unit instance {this}");
        }

        public override Tile Tile
        {
            get => base.Tile; set
            {
                base.Tile = value;
                StackLog.Debug($"Updating unit {this} tile to {value} {X}-{Y}");
                if (_unitObj == null)
                {
                    var art = StrategyGame.Specs.Units[this.SpecID].Art;
                    var prefab = Resources.Load("prefabs/units/" + art.Name);
                    _unitObj = MainBehaviour.Instantiate(prefab, UnitsContainerNode.transform) as GameObject;
                    _unitObj.transform.position = new Vector3(X, 0, Y);
                    _unitObj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    _unitObj.name = $"Unit Spec {this.SpecID} from {this.OwnerID}";
                }
            }
        }

        public GameObject UnitsContainerNode
        {
            get
            {
                if (_unitsNode == null)
                    _unitsNode = new GameObject("Units Container");
                return _unitsNode;
            }
        }

        public PlayerEntity FindOwner(Unit u)
        {
            return MainBehaviour.StrategyGame.GetWorld().GetOrCreateClientPlayer(u.OwnerID);
        }
    }
}
