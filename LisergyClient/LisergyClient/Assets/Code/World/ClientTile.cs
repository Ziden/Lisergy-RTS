using Game;
using Game.Entity;
using Game.TacticsBattle;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientTile : TacticsTile, IGameObject
    {
        private GameObject _gameObj;

        public bool Decorated;

        public ClientTile(Chunk c, int x, int y) : base(c, x, y) { }

        public void SetVisible(bool visible)
        {
            if (_gameObj == null)
                return;

            if (_gameObj.activeSelf == visible)
                return;

            StackLog.Debug($"Changing activaction of {this} to {visible}");
            _gameObj.SetActive(visible);
        }

        public override void SetSeenBy(ExploringEntity entity)
        {
            base.SetSeenBy(entity);
            StackLog.Debug($"{entity} sees {this}");
            if (entity.Owner == MainBehaviour.Player)
            {
                SetVisible(true);
                foreach (var party in this.MovingEntities)
                    ((ClientParty)party).GetGameObject().SetActive(true);
                if (this.StaticEntity is IGameObject)
                    ((IGameObject)this.StaticEntity).GetGameObject().SetActive(true);
            }
        }

        public override void SetUnseenBy(ExploringEntity unexplorer)
        {
            base.SetUnseenBy(unexplorer);

            if (unexplorer.Owner != MainBehaviour.Player)
                return;

            if (!this.IsVisibleTo(MainBehaviour.Player))
            {
                SetVisible(false);
                foreach (var party in this.MovingEntities)
                    ((ClientParty)party).GetGameObject().SetActive(false);
                if (this.StaticEntity is IGameObject)
                    ((IGameObject)this.StaticEntity).GetGameObject().SetActive(false);
            }
        }

        public virtual void Decorate()
        {
            var tileBhv = _gameObj.GetComponent<TileRandomizerBehaviour>();
            tileBhv.CreateTileDecoration(this);
        }

        public virtual string GetPrefabFolder()
        {
            return "tiles";
        }

        public virtual void RenderTile(byte tileID)
        {
            var tileSpec = StrategyGame.Specs.GetTileSpec(tileID);
            foreach (var art in tileSpec.Arts)
            {
                if (_gameObj == null)
                {
                    var prefab = PrefabCache.GetArt(GetPrefabFolder(), art);
                    if(prefab == null)
                    {
                        Log.Error("Prefab Not Found " + GetPrefabFolder() + art.Name);
                        continue;
                    }
                    var parent = ((ClientChunk)this.Chunk).GetGameObject().transform;
                    _gameObj = MainBehaviour.Instantiate(prefab, parent) as GameObject;
                    _gameObj.name = $"Tile_{X}-{Y}";
                    _gameObj.transform.position = new Vector3(X, 0, Y);
                    Decorate();
                    base.TileId = tileID;
                    return;
                }
            }
        }

        public GameObject GetGameObject() => _gameObj;

        public override byte TileId
        {
            get { return base.TileId; }
            set
            {
                StackLog.Debug($"Updating {this} tileid to {value}");
                RenderTile(value);
                base.TileId = value;
            }
        }

        public override WorldEntity StaticEntity 
        {
            get { return base.StaticEntity; }
            set
            {
                if (value != null && value is IGameObject)
                {
                    var gameObject = ((IGameObject)value).GetGameObject();
                    gameObject.transform.position = new Vector3(X, 0, Y);
                }
                using (new StackLog($"[Static Entity] New {value} on {this}"))
                {
                    base.StaticEntity = value;
                }
            }
        }
    }
}
