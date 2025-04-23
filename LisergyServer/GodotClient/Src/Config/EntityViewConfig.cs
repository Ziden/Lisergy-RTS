using Game.Entities;
using Godot;
using Godot.Collections;

namespace LisergyGodotClient.Config;

public partial class EntityViewConfig : Resource
{
	[Export] public Dictionary<EntityType, NodePath> ViewConfigs;
}