using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Game.DataTypes;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Code.Editor
{
	public class AddressableGeneration
	{
		// Path Contains -> Enum Name
		private static readonly Dictionary<string, string> CONFIG = new Dictionary<string, string>()
		{
			{"Sfx", "SoundFX"},
			{"Tiles", "TilePrefab"},
			{"Buildings", "BuildingPrefab"},
			{"Units", "UnitPrefab"},
			{"Effects", "MapFX"},
			{"Sprites", "SpritePrefab"},
			{"MapObjects", "MapObjectPrefab"},
			{"Screens", "UIScreen"},
			{"UISetting", "UISetting"},
		};
		
		[MenuItem("Lisergy/Generate Addressable Map")]
		private static void GenerateAddressableIds()
		{
			GenerateCode();
			AssetDatabase.Refresh();
		}
		
		private static List<AddressableAssetEntry> GetAddressables()
		{
			var assetList = new List<AddressableAssetEntry>();
			var assetsSettings = AddressableAssetSettingsDefaultObject.Settings;
			foreach (var settingsGroup in assetsSettings.groups)
			{
				if (settingsGroup.ReadOnly) continue;
				settingsGroup.GatherAllAssets(assetList, true, true, false);
			}
			return assetList;
		}
		
		private static void GenerateCode()
		{
			DefaultValueDictionary<string, List<AddressableAssetEntry>> Categorized = new DefaultValueDictionary<string, List<AddressableAssetEntry>>();
			foreach (var a in GetAddressables())
			{
				Debug.Log(a.AssetPath);
				bool fit = false;
				foreach (var cfg in CONFIG)
				{
					if (a.AssetPath.Contains(cfg.Key))
					{
						Categorized[cfg.Value].Add(a);
						fit = true;
					}
				}
				if (!fit)
				{
					Categorized["Generic"].Add(a);
				}
			}
			int id = 0;
			Dictionary<int, string> _idMap = new();
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("/* AUTO GENERATED CODE BY AddressableGeneration.cs */");
			stringBuilder.AppendLine("using System.Collections.Generic;");
			stringBuilder.AppendLine($"namespace GameAssets");
			stringBuilder.AppendLine("{");
			foreach (var kp in Categorized)
			{
				stringBuilder.AppendLine("");
				stringBuilder.AppendLine($"\tpublic enum {kp.Key}");
				stringBuilder.AppendLine("\t{");
				foreach (var a in kp.Value)
				{
					stringBuilder.AppendLine($"\t\t{Format(a.MainAsset.name)} = {id},");
					_idMap[id] = a.address;
					id++;
				}
				stringBuilder.AppendLine("\t}");
				stringBuilder.AppendLine("");
			}
			stringBuilder.AppendLine($"\tpublic static class AddressIdMap");
			stringBuilder.AppendLine("\t{");
			stringBuilder.AppendLine("\t\tpublic static IReadOnlyDictionary<int, string> IdMap = new Dictionary<int, string>() {");
			foreach (var kp in _idMap)
				stringBuilder.AppendLine($"\t\t\t{{ {kp.Key}, \"{kp.Value}\"}},");
			stringBuilder.AppendLine("\t\t};");
			stringBuilder.AppendLine("\t}");
			stringBuilder.AppendLine("}");
			File.WriteAllText("Assets/Code/Runtime/Assets/Addresses.cs", stringBuilder.ToString());
		}

		private static string Format(string s)
		{
			var x = s.Replace(" ", "");
			x = x.Replace("-", "_");
			if (x.Length == 0) return "null";
			x = Regex.Replace(x, "([A-Z])([A-Z]+)($|[A-Z])",
				m => m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value);
			var camel = char.ToLower(x[0]) + x.Substring(1);
			return char.ToUpper(camel[0]) + camel.Substring(1);
		}
	}
}