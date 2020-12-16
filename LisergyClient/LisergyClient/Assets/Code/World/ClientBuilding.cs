using Game;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientBuilding : Building
    {
        public GameObject Object;

        public ClientBuilding(byte id, ClientPlayer owner): base(id, owner)
        {
            var prefab = Resources.Load("prefabs/buildings/"+id);
            Log.Debug("-----> Instantiating BUILDING " + (prefab!=null));
            Object = MainBehaviour.Instantiate(prefab) as GameObject;
        }
    }
}
