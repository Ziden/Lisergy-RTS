using System;

namespace GameData.Specs;

[Serializable]
public class ItemSpec
{
	public ArtSpec Art;
	public ushort Id;
	public string Name;
	public ItemType Type;
}