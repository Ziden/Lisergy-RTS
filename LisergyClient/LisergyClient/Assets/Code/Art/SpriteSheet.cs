using UnityEngine;

public class SpriteSheet : MonoBehaviour
{
    private MeshRenderer _renderer;


    public void SetCoords(int x, int y)
    {
        _renderer.material.mainTextureOffset = new Vector2(x * 0.05f, y * 0.05f);
    }

    // Start is called before the first frame update
    void Start()
    {
        _renderer = this.GetComponent<MeshRenderer>();
        _renderer.material.SetTextureScale("_MainTex", new Vector2(0.05f, 0.05f));
        SetCoords(2, 2);
       

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
