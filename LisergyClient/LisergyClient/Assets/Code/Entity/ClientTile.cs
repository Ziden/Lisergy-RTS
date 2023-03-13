using Game;
using Game.Entity;
using UnityEngine;

namespace Assets.Code.World
{
    public class ClientTile : Tile, IGameObject
    {
        private GameObject _gameObj;

        public bool Decorated;

        public ClientTile(ClientChunk c, int x, int y) : base(c, x, y) { }

        public ClientChunk ClientChunk => Chunk as ClientChunk;

        public void SetVisible(bool visible)
        {
            if (_gameObj == null)
                return;

            if(visible == false)
            {
                SetColor(new Color(0.5f, 0.5f, 0.5f, 1.0f));
            } else
            {
                SetColor(new Color(1f, 1f, 1f, 1.0f));
                _gameObj.SetActive(visible);
            }  
        }

        private void SetColor(Color c)
        {
            foreach(Transform child in _gameObj.transform)
            {
                var rend = child.GetComponent<Renderer>();
                if(rend != null)
                {
                    rend.material.color = c;
                }
            }
        }

        public void UpdateFrom(Tile serverTile)
        {
            TileId = serverTile.TileId;
            ResourceID = serverTile.ResourceID;
            SetVisible(true);
        }

        public override void SetSeenBy(ExploringEntity entity)
        {
            var a = this;
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

        public void AddToScene(byte tileID)
        {
            var tileSpec = StrategyGame.Specs.Tiles[tileID];
            foreach (var art in tileSpec.Arts)
            {
                if (_gameObj == null)
                {
                    var prefab = Resources.Load("prefabs/tiles/" + art.Name);
                    var parent = ((ClientChunk)this.Chunk).GetGameObject().transform;
                    _gameObj = MainBehaviour.Instantiate(prefab, parent) as GameObject;
                    _gameObj.name = $"Tile_{X}-{Y}";
                    _gameObj.transform.position = new Vector3(X, 0, Y);
                    var tileBhv = _gameObj.GetComponent<TileRandomizerBehaviour>();
                    base.TileId = tileID;
                    tileBhv.CreateTileDecoration(this);
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
                AddToScene(value);
                base.TileId = value;
            }
        }

        public override StaticEntity StaticEntity 
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
