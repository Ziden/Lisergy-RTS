/* AUTO GENERATED CODE BY AddressableGeneration.cs */
using System.Collections.Generic;
namespace GameAssets
{

	public enum SoundFX
	{
		Buttonclick = 0,
	}


	public enum SpritePrefab
	{
		Faces = 1,
	}


	public enum UIScreen
	{
		LoginScreen = 2,
		PartySelectBar = 3,
		UnitActions = 4,
	}


	public enum UISetting
	{
		PanelSettings = 5,
	}


	public enum BuildingPrefab
	{
		Dungeon = 6,
		SimpleHouse = 7,
	}


	public enum MapFX
	{
		BattleEffect = 8,
		HalfPath = 9,
	}


	public enum MapObjectPrefab
	{
		Cursor = 10,
		UnitCursor = 11,
	}


	public enum TilePrefab
	{
		Cloud = 12,
		Floor = 13,
		Fog = 14,
		Forest = 15,
		Mountain = 16,
		Plains = 17,
		Plane = 18,
		Tree = 19,
		Water = 20,
	}


	public enum UnitPrefab
	{
		Mage = 21,
	}

	public static class AddressIdMap
	{
		public static IReadOnlyDictionary<int, string> IdMap = new Dictionary<int, string>() {
			{ 0, "Assets/Addressables/Audio/Sfx/button_click.mp3"},
			{ 1, "Assets/Addressables/Sprites/Faces.png"},
			{ 2, "Assets/Addressables/UIScreens/LoginScreen.uxml"},
			{ 3, "Assets/Addressables/UIScreens/PartySelectBar.uxml"},
			{ 4, "Assets/Addressables/UIScreens/UnitActions.uxml"},
			{ 5, "Assets/Addressables/UISettings/PanelSettings.asset"},
			{ 6, "Assets/Addressables/Prefabs/Buildings/dungeon.prefab"},
			{ 7, "Assets/Addressables/Prefabs/Buildings/SimpleHouse.prefab"},
			{ 8, "Assets/Addressables/Prefabs/Effects/BattleEffect.prefab"},
			{ 9, "Assets/Addressables/Prefabs/Effects/HalfPath.prefab"},
			{ 10, "Assets/Addressables/Prefabs/MapObjects/Cursor.prefab"},
			{ 11, "Assets/Addressables/Prefabs/MapObjects/UnitCursor.prefab"},
			{ 12, "Assets/Addressables/Prefabs/Tiles/Cloud.prefab"},
			{ 13, "Assets/Addressables/Prefabs/Tiles/Floor.prefab"},
			{ 14, "Assets/Addressables/Prefabs/Tiles/Fog.prefab"},
			{ 15, "Assets/Addressables/Prefabs/Tiles/Forest.prefab"},
			{ 16, "Assets/Addressables/Prefabs/Tiles/Mountain.prefab"},
			{ 17, "Assets/Addressables/Prefabs/Tiles/Plains.prefab"},
			{ 18, "Assets/Addressables/Prefabs/Tiles/Plane.prefab"},
			{ 19, "Assets/Addressables/Prefabs/Tiles/Tree.prefab"},
			{ 20, "Assets/Addressables/Prefabs/Tiles/Water.prefab"},
			{ 21, "Assets/Addressables/Prefabs/Units/Mage.prefab"},
		};
	}
}
