using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Assets.Code.Runtime.Tools
{
    public class ActivationObjectPool
    {
        public List<GameObject> _inactive = new List<GameObject>();
        public List<GameObject> _active = new List<GameObject>();

        public void AddNew(GameObject active)
        {
            _active.Add(active);
        }

        public void Release(GameObject obj)
        {
            if(_active.Remove(obj))
            {
                _inactive.Add(obj);
                obj.SetActive(false);
            }
        }

        public GameObject Obtain()
        {
            if(_inactive.Count > 0)
            {
                var pooled = _inactive[0];
                _inactive.RemoveAt(0);
                _active.Add(pooled);
                pooled.SetActive(true);
                return pooled;
            }
            return null;
        }
    }
}
