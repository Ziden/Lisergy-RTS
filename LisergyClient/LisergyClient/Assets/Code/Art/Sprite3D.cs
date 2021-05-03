using UnityEngine;


public class Sprite3D : MonoBehaviour
{
    public static readonly int FACE = 7;

    public Camera m_Camera;
    public bool amActive = false;
    public Sprite[] Sprites;

    void Awake()
    {
        m_Camera = Camera.main;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.back, m_Camera.transform.rotation * Vector3.up);
    }
}