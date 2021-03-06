using UnityEngine;


public class Sprite3D : MonoBehaviour
{

    public Camera m_Camera;
    public bool amActive = false;
    public Sprite[] Sprites;
    public Sprite Face => Sprites[7]; 

    void Awake()
    {
        m_Camera = Camera.main;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.back, m_Camera.transform.rotation * Vector3.up);
    }
}