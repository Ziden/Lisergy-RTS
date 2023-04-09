/* AUTO GENERATED CODE BY AddressableGeneration.cs */
using System.Collections.Generic;
namespace GameAssets
{

	public enum SpritePrefab
	{
		Alchemy = 0,
		Badge_assassin = 1,
		Badge_barbarian = 2,
		Badge_hunter = 3,
		Badge_mage = 4,
		Badge_necro = 5,
		Badge_paladin = 6,
		Badge_priest = 7,
		Badge_rogue = 8,
		Badge_warrior = 9,
		Blacksmith_craft = 10,
		Carpentry = 11,
		Diplomacy = 12,
		Farming = 13,
		Fishing = 14,
		Hunting = 15,
		Mining = 16,
		Tailoring = 17,
		Faces = 18,
	}


	public enum UnitPrefab
	{
		Mage = 19,
	}


	public enum TilePrefab
	{
		Floor = 20,
		Fog50 = 21,
		FogBlack = 22,
		FogBorderContainer = 23,
		Forest = 24,
		Mountain = 25,
		Plains = 26,
		Plane = 27,
		Tree = 28,
		Water = 29,
	}

	public enum MapObjectPrefab
	{
		Cursor = 30,
		UnitCursor = 31,
	}

	public enum MapFX
	{
		BattleEffect = 32,
		HalfPath = 33,
	}

	public enum BuildingPrefab
	{
		Castle = 34,
		Dungeon = 35,
		SimpleHouse = 36,
	}

	public enum UIScreen
	{
		LoginScreen = 37,
		PartySelectBar = 38,
		UnitActions = 39,
		BattleNotification = 40,
	}

	public enum UISetting
	{
		PanelSettings = 41,
	}


	public enum SoundFX
	{
		Button_click = 42,
	}


	public enum Generic
	{
		Alexander_nakarada_tavern_loop_one = 43,
		Alexander_nakarada_the_vikings = 44,
	}

	public static class AddressIdMap
	{
		public static IReadOnlyDictionary<int, string> IdMap = new Dictionary<int, string>() {
			{ 0, "Assets/Addressables/Sprites/Badges/alchemy.png"},
			{ 1, "Assets/Addressables/Sprites/Badges/Badge_assassin.PNG"},
			{ 2, "Assets/Addressables/Sprites/Badges/Badge_barbarian.png"},
			{ 3, "Assets/Addressables/Sprites/Badges/Badge_hunter.PNG"},
			{ 4, "Assets/Addressables/Sprites/Badges/Badge_mage.png"},
			{ 5, "Assets/Addressables/Sprites/Badges/Badge_necro.png"},
			{ 6, "Assets/Addressables/Sprites/Badges/Badge_paladin.PNG"},
			{ 7, "Assets/Addressables/Sprites/Badges/Badge_priest.PNG"},
			{ 8, "Assets/Addressables/Sprites/Badges/Badge_rogue.PNG"},
			{ 9, "Assets/Addressables/Sprites/Badges/Badge_warrior.png"},
			{ 10, "Assets/Addressables/Sprites/Badges/blacksmith_craft.PNG"},
			{ 11, "Assets/Addressables/Sprites/Badges/carpentry.PNG"},
			{ 12, "Assets/Addressables/Sprites/Badges/diplomacy.PNG"},
			{ 13, "Assets/Addressables/Sprites/Badges/farming.png"},
			{ 14, "Assets/Addressables/Sprites/Badges/fishing.PNG"},
			{ 15, "Assets/Addressables/Sprites/Badges/hunting.PNG"},
			{ 16, "Assets/Addressables/Sprites/Badges/mining.png"},
			{ 17, "Assets/Addressables/Sprites/Badges/tailoring.png"},
			{ 18, "Assets/Addressables/Sprites/Faces.png"},
			{ 19, "Assets/Addressables/Prefabs/Units/Mage.prefab"},
			{ 20, "Assets/Addressables/Prefabs/Tiles/Floor.prefab"},
			{ 21, "Assets/Addressables/Prefabs/Tiles/Fog50.prefab"},
			{ 22, "Assets/Addressables/Prefabs/Tiles/FogBlack.prefab"},
			{ 23, "Assets/Addressables/Prefabs/Tiles/FogBorderContainer.prefab"},
			{ 24, "Assets/Addressables/Prefabs/Tiles/Forest.prefab"},
			{ 25, "Assets/Addressables/Prefabs/Tiles/Mountain.prefab"},
			{ 26, "Assets/Addressables/Prefabs/Tiles/Plains.prefab"},
			{ 27, "Assets/Addressables/Prefabs/Tiles/Plane.prefab"},
			{ 28, "Assets/Addressables/Prefabs/Tiles/Tree.prefab"},
			{ 29, "Assets/Addressables/Prefabs/Tiles/Water.prefab"},
			{ 30, "Assets/Addressables/Prefabs/MapObjects/Cursor.prefab"},
			{ 31, "Assets/Addressables/Prefabs/MapObjects/UnitCursor.prefab"},
			{ 32, "Assets/Addressables/Prefabs/Effects/BattleEffect.prefab"},
			{ 33, "Assets/Addressables/Prefabs/Effects/HalfPath.prefab"},
			{ 34, "Assets/Addressables/Prefabs/Buildings/Castle.prefab"},
			{ 35, "Assets/Addressables/Prefabs/Buildings/dungeon.prefab"},
			{ 36, "Assets/Addressables/Prefabs/Buildings/SimpleHouse.prefab"},
			{ 37, "Assets/Addressables/UIScreens/LoginScreen.uxml"},
			{ 38, "Assets/Addressables/UIScreens/PartySelectBar.uxml"},
			{ 39, "Assets/Addressables/UIScreens/UnitActions.uxml"},
			{ 40, "Assets/Addressables/UIScreens/Notifications/BattleWinNotification.uxml"},
			{ 41, "Assets/Addressables/UISettings/PanelSettings.asset"},
			{ 42, "Assets/Addressables/Audio/Sfx/button_click.mp3"},
			{ 43, "Assets/Addressables/Audio/Bgm/alexander-nakarada-tavern-loop-one.mp3"},
			{ 44, "Assets/Addressables/Audio/Bgm/alexander-nakarada-the-vikings.mp3"},
		};
	}
}
