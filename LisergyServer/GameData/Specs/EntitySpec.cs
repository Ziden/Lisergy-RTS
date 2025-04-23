using System;

namespace GameData.Specs;

[Serializable]
public class EntitySpec
{
	public byte[] Components;
	public ArtSpec Icon;
	public string Name;
	public int Type;
}