using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HueShifter : MonoBehaviour
{
    public float Speed = 1;
    private Renderer rend;
    private Image img;

    void Start()
    {
        rend = GetComponent<Renderer>();
        img = GetComponent<Image>();
        img.material.color = Color.white;
    }

    void Update()
    {
        //if(rend!=null)
        //    rend.material.SetColor("_Color", HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * Speed, 1), 1, 1)));
        //else if(img != null)
            img.color = HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * Speed, 1), 1, 1));
    }
}
