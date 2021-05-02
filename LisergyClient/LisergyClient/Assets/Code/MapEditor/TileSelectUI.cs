
using Assets.Code;
using Game;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TileSelectUI : MonoBehaviour
{
    public UnityEngine.Object TileButtonPrefab;
    public Transform Content;

    private bool _rendered = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void RenderTiles()
    {
        if (_rendered)
            return;
        var x = 0f;
        var y = 0f;
        foreach(var tileID in StrategyGame.Specs.Tiles.Keys)
        {
            var tileSpec = StrategyGame.Specs.Tiles[tileID];
            var obj = (GameObject)Instantiate(TileButtonPrefab, Content);
            var img = obj.GetComponentInChildren<RawImage>();
            var button = obj.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => {
                MapEditEvents.SelectTileID(tileID);
                Close();
            });
            x += img.rectTransform.sizeDelta.x;
            obj.transform.position += new Vector3(x, 0, 0);
            img.texture = AssetPreview.GetAssetPreview(PrefabCache.GetArt("tacticstiles", tileSpec.Arts[0]));
        }
        _rendered = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        Log.Debug("Showing TileUI");
        gameObject.SetActive(true);
        RenderTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
