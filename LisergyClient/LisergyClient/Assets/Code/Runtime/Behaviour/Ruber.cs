using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruber : MonoBehaviour
{
    private float SpeedVelocity = 0.1f;

    private float CurrentVelocity = 0;
    private float size;
    private float Power;

    private Vector3 InitialPos;

    public static void Boing(GameObject obj, float size, float power)
    {
        var rub = obj.AddComponent<Ruber>();
        rub.size = size;
        rub.Power = power;
        rub.InitialPos = rub.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentVelocity += SpeedVelocity;
        transform.position += new Vector3(CurrentVelocity, 0, 0);
       // if(transform.position.x - InitialPos.x )

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
