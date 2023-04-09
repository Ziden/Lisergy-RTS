/* AUTO GENERATED CODE BY AddressableGeneration.cs */
using System.Collections.Generic;
namespace GameAssets
{

	public enum MapFX
	{
		HalfPath = 0,
		BattleEffect = 1,
	}


	public enum Generic
	{
		Alexander_nakarada_tavern_loop_one = 2,
		Alexander_nakarada_the_vikings = 3,
	}


	public enum SoundFX
	{
		Button_click = 4,
	}


	public enum BuildingPrefab
	{
		Castle = 5,
		Dungeon = 6,
		SimpleHouse = 7,
	}


	public enum TilePrefab
	{
		FogBorderContainer = 8,
		FogBlack = 9,
		Floor = 10,
		Forest = 11,
		Mountain = 12,
		Plains = 13,
		Plane = 14,
		Tree = 15,
		Water = 16,
		Fog50 = 17,
	}


	public enum MapObjectPrefab
	{
		Cursor = 18,
		UnitCursor = 19,
	}


	public enum UnitPrefab
	{
		Mage = 20,
	}


	public enum SpritePrefab
	{
		Faces = 21,
	}


	public enum UIScreen
	{
		LoginScreen = 22,
		PartySelectBar = 23,
		UnitActions = 24,
	}


	public enum UISetting
	{
		PanelSettings = 25,
	}

	public static class AddressIdMap
	{
		public static IReadOnlyDictionary<int, string> IdMap = new Dictionary<int, string>() {
			{ 0, "Assets/Addressables/Prefabs/Effects/HalfPath.prefab"},
			{ 1, "Assets/Addressables/Prefabs/Effects/BattleEffect.prefab"},
			{ 2, "Assets/Addressables/Audio/Bgm/alexander-nakarada-tavern-loop-one.mp3"},
			{ 3, "Assets/Addressables/Audio/Bgm/alexander-nakarada-the-vikings.mp3"},
			{ 4, "Assets/Addressables/Audio/Sfx/button_click.mp3"},
			{ 5, "Assets/Addressables/Prefabs/Buildings/Castle.prefab"},
			{ 6, "Assets/Addressables/Prefabs/Buildings/dungeon.prefab"},
			{ 7, "Assets/Addressables/Prefabs/Buildings/SimpleHouse.prefab"},
			{ 8, "Assets/Addressables/Prefabs/Tiles/FogBorderContainer.prefab"},
			{ 9, "Assets/Addressables/Prefabs/Tiles/FogBlack.prefab"},
			{ 10, "Assets/Addressables/Prefabs/Tiles/Floor.prefab"},
			{ 11, "Assets/Addressables/Prefabs/Tiles/Forest.prefab"},
			{ 12, "Assets/Addressables/Prefabs/Tiles/Mountain.prefab"},
			{ 13, "Assets/Addressables/Prefabs/Tiles/Plains.prefab"},
			{ 14, "Assets/Addressables/Prefabs/Tiles/Plane.prefab"},
			{ 15, "Assets/Addressables/Prefabs/Tiles/Tree.prefab"},
			{ 16, "Assets/Addressables/Prefabs/Tiles/Water.prefab"},
			{ 17, "Assets/Addressables/Prefabs/Tiles/Fog50.prefab"},
			{ 18, "Assets/Addressables/Prefabs/MapObjects/Cursor.prefab"},
			{ 19, "Assets/Addressables/Prefabs/MapObjects/UnitCursor.prefab"},
			{ 20, "Assets/Addressables/Prefabs/Units/Mage.prefab"},
			{ 21, "Assets/Addressables/Sprites/Faces.png"},
			{ 22, "Assets/Addressables/UIScreens/LoginScreen.uxml"},
			{ 23, "Assets/Addressables/UIScreens/PartySelectBar.uxml"},
			{ 24, "Assets/Addressables/UIScreens/UnitActions.uxml"},
			{ 25, "Assets/Addressables/UISettings/PanelSettings.asset"},
		};
	}
}
