using Assets.Code;
using Game;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MapEditUI : MonoBehaviour
{
    public Button TileSelect;
    public Button HeightPlus;
    public Button HeightMinus;
    public TileSelectUI TileSelectUI;
    public InputField HeightText;
    private byte CurrentHeight = 1;
    private ushort SelectedTileID = 0;

    void Start()
    {
        TileSelect.onClick.AddListener(OnTileSelect);
        HeightPlus.onClick.AddListener(OnHeightPlus);
        HeightMinus.onClick.AddListener(OnHeightMinus);
        HeightText.onValueChanged.AddListener(OnHeightChange);
        MapEditEvents.OnSelectTile += OnTileSelected;
        ClientEvents.OnPlayerLogin += OnLogin;
        this.gameObject.SetActive(false);
    }

    public void OnLogin(ClientPlayer player)
    {
        this.gameObject.SetActive(true);
        MapEditEvents.SelectTileID(SelectedTileID);
    }

    public void OnTileSelected(ushort tileID)
    {
        var tileSpec = StrategyGame.Specs.GetTileSpec(tileID);
        var art = tileSpec.Arts[0];
        var prefab = PrefabCache.GetArt("tacticstiles", art);
        if(prefab != null)
        {
            var texture = AssetPreview.GetAssetPreview(prefab);
            TileSelect.GetComponent<RawImage>().texture = texture;
        }
    }

    public void OnHeightChange(string n)
    {
        try
        {
            CurrentHeight = byte.Parse(n);
            MapEditEvents.ChangeHeight(CurrentHeight);
            Log.Debug($"Height set to {CurrentHeight}");
        } catch(Exception e)
        {
            HeightText.text = CurrentHeight.ToString();
        }
    }

    public void OnHeightPlus()
    {
        HeightText.text = (CurrentHeight+1).ToString();
    }

    public void OnHeightMinus()
    {
        HeightText.text = (CurrentHeight - 1).ToString();
    }

    public void OnTileSelect()
    {
        TileSelectUI.Show();
    }
}
