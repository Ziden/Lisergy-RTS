using ClientSDK.Data;
using Game.Systems.Dungeon;
using UnityEngine;

namespace Assets.Code.World
{
    public class DungeonView : UnityEntityView<DungeonEntity>
    {
        protected override void CreateView()
        {
            var spec = Client.Game.Specs.Dungeons[Entity.SpecId];
            State = EntityViewState.RENDERING;
            IGameClientServices.Assets.CreatePrefab(spec.Art, new Vector3(Entity.Tile.X, 0, Entity.Tile.Y), Quaternion.identity, o =>
            {
                GameObject = o;
                GameObject.transform.position = Entity.UnityPosition();
                GameObject.isStatic = true;
                State = EntityViewState.RENDERED;
            });
        }
    }
}
