using UnityEngine;


public class Sprite3D : MonoBehaviour
{

    public Camera m_Camera;
    public bool amActive = false;
    GameObject myContainer;
    public Sprite[] Sprites;
    public Sprite Face => Sprites[7]; 

    void Awake()
    {
        m_Camera = Camera.main;
        myContainer = new GameObject();
        myContainer.name = "Sprite_" + transform.gameObject.name;
        myContainer.transform.position = transform.position;
        transform.parent = myContainer.transform;
    }

    void LateUpdate()
    {
        myContainer.transform.LookAt(myContainer.transform.position + m_Camera.transform.rotation * Vector3.back, m_Camera.transform.rotation * Vector3.up);
    }
}