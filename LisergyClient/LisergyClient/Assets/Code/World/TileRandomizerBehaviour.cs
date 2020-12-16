using Game.Generator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRandomizerBehaviour : MonoBehaviour
{
    public List<GameObject> Remove50;
    public List<GameObject> Remove85;
    public List<GameObject> Remove25;
    public List<GameObject> Objects;

    public int Amount = 10;

    private static System.Random rnd = new System.Random();

    void Start()
    {
        var removed = new List<GameObject>();
        foreach(var o in Remove50)
        {
            if (rnd.NextDouble() < 0.55)
                removed.Add(o);
           
        }
        foreach (var o in Remove85)
        {
            if (rnd.NextDouble() < 0.85)
                removed.Add(o);
        }
        foreach (var o in Remove25)
        {
            if (rnd.NextDouble() < 0.25)
                removed.Add(o);
        }

        foreach (var obj in Objects)
        {
            if(removed.Contains(obj))
            {
                Destroy(obj);
            } else
            {
                float x = -0.5f + (float)rnd.NextDouble();
                float y = -0.5f + (float)rnd.NextDouble();
                obj.transform.localPosition = new Vector3(x, 0, y);
            }
           
        }
    }
}
