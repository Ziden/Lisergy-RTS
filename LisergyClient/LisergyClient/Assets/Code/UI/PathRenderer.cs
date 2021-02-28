using Assets.Code.World;
using Game;
using Game.World;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.UI
{
    public class PathRenderer
    {
        private List<GameObject> _pathLines = new List<GameObject>();

        private GameObject GetOrCreatePathLine()
        {
            var pooled = _pathLines.FirstOrDefault(path => !path.activeInHierarchy);
            if (pooled == null)
            {
                Log.Debug("Created new path line");
                pooled = MainBehaviour.Instantiate(Resources.Load("prefabs/HalfPath")) as GameObject;
                _pathLines.Add(pooled);
            } else
            {
                pooled.SetActive(true);
                pooled.transform.rotation = Quaternion.identity;
            }
            return pooled;
        }

        public void RenderPath(List<ClientTile> tilePath)
        {
            _pathLines.ForEach(path => path.SetActive(false));
            for (var x = 0; x < tilePath.Count; x++)
            {
                var nodeTile = tilePath[x];
                var tilePos = nodeTile.GameObj.transform.position;
                var hasNext = x < tilePath.Count - 1;
                var hasPrevious = x > 0;
                if (hasNext)
                {
                    var next = tilePath[x + 1];
                    var direction = nodeTile.GetDirection(next);
                    if(direction == Direction.SOUTH)
                    {
                        var line = GetOrCreatePathLine();
                        line.transform.position = new Vector3(tilePos.x, tilePos.y + 0.1f, tilePos.z - 0.25f);
                    }
                    else if (direction == Direction.NORTH)
                    {
                        var line = GetOrCreatePathLine();
                        line.transform.position = new Vector3(tilePos.x, tilePos.y + 0.1f, tilePos.z + 0.25f);
                    }
                    else if (direction == Direction.EAST)
                    {
                        var line = GetOrCreatePathLine();
                        line.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
                        line.transform.position = new Vector3(tilePos.x + 0.25f, tilePos.y + 0.1f, tilePos.z);
                    }
                    else if (direction == Direction.WEST)
                    {
                        var line = GetOrCreatePathLine();
                        line.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
                        line.transform.position = new Vector3(tilePos.x - 0.25f, tilePos.y + 0.1f, tilePos.z);
                    }
                }
                if (hasPrevious)
                {
                    
                    var previous = tilePath[x - 1];
                    var direction = nodeTile.GetDirection(previous);
                    if (direction == Direction.NORTH)
                    {
                        var line = GetOrCreatePathLine();
                        line.transform.position = new Vector3(tilePos.x, tilePos.y + 0.1f, tilePos.z + 0.25f);
                    } else if (direction == Direction.SOUTH)
                    {
                        var line = GetOrCreatePathLine();
                        line.transform.position = new Vector3(tilePos.x, tilePos.y + 0.1f, tilePos.z - 0.25f);
                    }
                    else if (direction == Direction.EAST)
                    {
                        var line = GetOrCreatePathLine();
                        line.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
                        line.transform.position = new Vector3(tilePos.x + 0.25f, tilePos.y + 0.1f, tilePos.z);
                    }
                    else if (direction == Direction.WEST)
                    {
                        var line = GetOrCreatePathLine();
                        line.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
                        line.transform.position = new Vector3(tilePos.x - 0.25f, tilePos.y + 0.1f, tilePos.z);
                    }
                    
                }
            }
        }
    }
}
