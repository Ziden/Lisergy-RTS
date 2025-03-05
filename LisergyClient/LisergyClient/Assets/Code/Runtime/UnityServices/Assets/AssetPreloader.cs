using ClientSDK;
using Cysharp.Threading.Tasks;
using GameAssets;
using GameData;
using UnityEngine;

public class AssetPreloader
{
    public static async UniTask StartPreload(IGameClient client, GameSpec spec)
    {
        Debug.Log("Loading Tiles");
        foreach (var t in spec.Tiles.Values)
        {
            await client.UnityServices().Assets.PreloadAsset(t.TilePrefab);
        }

        Debug.Log("Loading Units");
        foreach (var u in spec.Units.Values)
        {
            await client.UnityServices().Assets.PreloadAsset(u.Art);
        }

        await client.UnityServices().Assets.PreloadAsset(VfxPrefab.HalfPath);
        await client.UnityServices().Assets.PreloadAsset(TilePrefab.FogBlack);
        await client.UnityServices().Assets.PreloadAsset(MapObjectPrefab.TileCursor);
        await client.UnityServices().Assets.GetScreen(UIScreen.GameHud);
        await client.UnityServices().Assets.GetScreen(UIScreen.ScreenPartyActions);
        Debug.Log("Preload Complete");
    }
}