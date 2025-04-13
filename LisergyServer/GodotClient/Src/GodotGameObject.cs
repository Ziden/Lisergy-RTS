using ClientSDK;
using Game.World;
using Godot;
using GodotClient.Services;
using LisergyGodotClient.Src;
using System;

namespace GodotClient
{
	public class GodotGameObject : IGameObject
	{
		public Node Node { get; private set; }
		public CompressedTexture2D Texture { get; private set; }

		public GodotGameObject(Node n)
		{
			Node = n;
		}

		public GodotGameObject(CompressedTexture2D n)
		{
			Texture = n;
		}

		public string Name
		{
			get => Node.Name;
			set => Node.Name = value;
		}

		public Location Location
		{
			get => Node switch
			{
				Node3D node3D => node3D.GlobalPosition.ToLocation(),
				Node2D node2D => node2D.GlobalPosition.ToLocation(),
				_ => throw new NotImplementedException("Node type not supported for Location property.")
			};
			set
			{
				switch (Node)
				{
					case Node3D node3D:
						var transform = node3D.GlobalTransform;
						transform.Origin = value.ToGodotVector3();
						node3D.GlobalTransform = transform;
						break;
					case Node2D node2D:
						node2D.GlobalPosition = value.ToGodotVector2();
						break;
					default:
						throw new NotImplementedException("Node type not supported for Location property.");
				}
			}
		}

		public bool Visible
		{
			get => Node switch
			{
				Node3D node3D => node3D.Visible,
				Node2D node2D => node2D.Visible,
				Control ctrl => ctrl.Visible,
				_ => throw new NotImplementedException("Node type not supported for Location property.")
			};
			set
			{
				switch (Node)
				{
					case Node3D node3D:
						node3D.Visible = value;
						break;
					case Node2D node2D:
						node2D.Visible = value;
						break;
					case Control ctrl:
						ctrl.Visible = value;
						break;
					default:
						throw new NotImplementedException("Node type not supported for Location property.");
				}
			}
		}

		public void AddChild(IGameObject child)
		{
			var otherNode = ((GodotGameObject)child).Node;
			if(otherNode.GetParent() != null)
			{
				otherNode.Reparent(Node, false);
			} else
			{
				Node.AddChild(otherNode);
			}
		}

		public void DestroyChild(IGameObject child)
		{
			var n = ((GodotGameObject)child).Node;
			Node.RemoveChild(n);
			n.QueueFree();
		}

		public T Get<T>() where T : class
		{
			if (Node is T scriptInstance)
			{
				return scriptInstance;
			}
			return Node.FindFirstOfType<T>();
		}

		public void Destroy()
		{
			var parent = Node.GetParent();
			if(parent != null)
			{
				parent.RemoveChild(Node);
			}
			Node.QueueFree();
		}
	}
}
