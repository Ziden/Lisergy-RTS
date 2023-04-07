/* AUTO GENERATED CODE BY AddressableGeneration.cs */
using System.Collections.Generic;
namespace GameAssets
{

	public enum TilePrefab
	{
		Cloud = 0,
		Fog = 1,
		Forest = 2,
		Mountain = 3,
		Plains = 4,
		Plane = 5,
		Tree = 6,
		Water = 7,
		Floor = 8,
	}


	public enum SoundFX
	{
		Buttonclick = 9,
	}


	public enum BuildingPrefab
	{
		Dungeon = 10,
		SimpleHouse = 11,
	}


	public enum UnitPrefab
	{
		Mage = 12,
	}


	public enum MapFX
	{
		BattleEffect = 13,
		HalfPath = 14,
	}


	public enum SpritePrefab
	{
		Faces = 15,
	}


	public enum MapObjectPrefab
	{
		Cursor = 16,
		UnitCursor = 17,
	}


	public enum UIScreen
	{
		LoginScreen = 18,
		PartySelectBar = 19,
	}

	public static class AddressIdMap
	{
		public static IReadOnlyDictionary<int, string> IdMap = new Dictionary<int, string>() {
			{ 0, "Assets/Addressables/Prefabs/Tiles/Cloud.prefab"},
			{ 1, "Assets/Addressables/Prefabs/Tiles/Fog.prefab"},
			{ 2, "Assets/Addressables/Prefabs/Tiles/Forest.prefab"},
			{ 3, "Assets/Addressables/Prefabs/Tiles/Mountain.prefab"},
			{ 4, "Assets/Addressables/Prefabs/Tiles/Plains.prefab"},
			{ 5, "Assets/Addressables/Prefabs/Tiles/Plane.prefab"},
			{ 6, "Assets/Addressables/Prefabs/Tiles/Tree.prefab"},
			{ 7, "Assets/Addressables/Prefabs/Tiles/Water.prefab"},
			{ 8, "Assets/Addressables/Prefabs/Tiles/Floor.prefab"},
			{ 9, "Assets/Addressables/Audio/Sfx/button_click.mp3"},
			{ 10, "Assets/Addressables/Prefabs/Buildings/dungeon.prefab"},
			{ 11, "Assets/Addressables/Prefabs/Buildings/SimpleHouse.prefab"},
			{ 12, "Assets/Addressables/Prefabs/Units/Mage.prefab"},
			{ 13, "Assets/Addressables/Prefabs/Effects/BattleEffect.prefab"},
			{ 14, "Assets/Addressables/Prefabs/Effects/HalfPath.prefab"},
			{ 15, "Assets/Addressables/Sprites/Faces.png"},
			{ 16, "Assets/Addressables/Prefabs/MapObjects/Cursor.prefab"},
			{ 17, "Assets/Addressables/Prefabs/MapObjects/UnitCursor.prefab"},
			{ 18, "Assets/Addressables/UIScreens/LoginScreen.uxml"},
			{ 19, "Assets/Addressables/UIScreens/PartySelectBar.uxml"},
		};
	}
}
