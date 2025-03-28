using Godot;
using System.Collections.Generic;
using static Godot.Node;

namespace LisergyGodotClient.Src.Utils
{
    /// <summary>
    /// Simple object pooling system for Godot nodes to improve performance by reusing objects
    /// instead of creating and destroying them.
    /// </summary>
    public class GodotObjectPool
    {
        private List<Node> _inactive = new List<Node>();
        private List<Node> _active = new List<Node>();

        /// <summary>
        /// Add a newly created object to the active pool
        /// </summary>
        public void AddNew(Node active)
        {
            _active.Add(active);
        }

        /// <summary>
        /// Release an object back to the inactive pool
        /// </summary>
        public void Release(Node obj)
        {
            if (_active.Remove(obj))
            {
                _inactive.Add(obj);
                obj.ProcessMode = ProcessModeEnum.Disabled;
                if(obj is Node3D n)
                {
                    n.Visible = false;
                }
              
            }
        }

        /// <summary>
        /// Get an object from the pool or null if none are available
        /// </summary>
        public Node Obtain()
        {
            if (_inactive.Count > 0)
            {
                var pooled = _inactive[0];
                _inactive.RemoveAt(0);
                _active.Add(pooled);
                pooled.ProcessMode = ProcessModeEnum.Inherit;
                if (pooled is Node3D n)
                {
                    n.Visible = false;
                }
                return pooled;
            }
            return null;
        }

        /// <summary>
        /// Get an object from the pool with the specified type or null if none are available
        /// </summary>
        public T Obtain<T>() where T : Node
        {
            return Obtain() as T;
        }

        /// <summary>
        /// Clear all objects from the pool
        /// </summary>
        public void Clear()
        {
            foreach (var node in _inactive)
            {
                node.QueueFree();
            }
            _inactive.Clear();

            foreach (var node in _active)
            {
                node.QueueFree();
            }
            _active.Clear();
        }

        /// <summary>
        /// Get count of available objects in the pool
        /// </summary>
        public int AvailableCount => _inactive.Count;

        /// <summary>
        /// Get count of active objects from the pool
        /// </summary>
        public int ActiveCount => _active.Count;
    }
}
