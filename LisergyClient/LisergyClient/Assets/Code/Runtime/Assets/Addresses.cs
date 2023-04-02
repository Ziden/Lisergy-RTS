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
	}


	public enum SoundFX
	{
		Buttonclick = 8,
	}


	public enum BuildingPrefab
	{
		Dungeon = 9,
		SimpleHouse = 10,
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
			{ 8, "Assets/Addressables/Audio/Sfx/button_click.mp3"},
			{ 9, "Assets/Addressables/Prefabs/Buildings/dungeon.prefab"},
			{ 10, "Assets/Addressables/Prefabs/Buildings/SimpleHouse.prefab"},
		};
	}
}
