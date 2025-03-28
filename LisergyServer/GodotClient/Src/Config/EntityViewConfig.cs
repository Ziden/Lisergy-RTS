using Game.Entities;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisergyGodotClient.Config
{

	public partial class EntityViewConfig : Resource
	{
		[Export]
		public Godot.Collections.Dictionary<EntityType, NodePath> ViewConfigs;
	}
}
