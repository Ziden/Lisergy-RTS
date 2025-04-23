using System;
using System.Collections.Generic;
using Godot;
using LisergyGodotClient.Src;

public class CachedFogMaterials
{
	public BaseMaterial3D WithFog;
	public BaseMaterial3D WithoutFog;
}

public class TileFog
{
	public Dictionary<string, Color> _colors = new();
	public Dictionary<string, CachedFogMaterials> _originals = new();
	public Dictionary<BaseMaterial3D, BaseMaterial3D> _shaded = new();

	public TileFog(int mapWidth, int mapHeight, Node parent)
	{
	}

	private void HandleMaterial(MeshInstance3D mesh, BaseMaterial3D mat, bool visible, int surface)
	{
		if (_shaded.TryGetValue(mat, out var original) && visible)
		{
			mesh.MaterialOverride = original;
			return;
		}

		var key = mat.ResourcePath;
		if (!_originals.TryGetValue(key, out var cached))
		{
			var prevColor = mat.AlbedoColor;
			cached = new CachedFogMaterials
			{
				WithoutFog = mat,
				WithFog = mat.Duplicate() as StandardMaterial3D
			};
			cached.WithFog.AlbedoColor = new Color(0.25f, 0.25f, 0.25f);
			cached.WithoutFog.AlbedoColor = prevColor;
			_originals[key] = cached;
			_shaded[cached.WithFog] = cached.WithoutFog;
		}

		if (visible)
		{
			if (mesh.MaterialOverride is BaseMaterial3D std)
			{
				std.AlbedoColor = cached.WithoutFog.AlbedoColor;
			}
			else if (cached.WithoutFog != null)
			{
				if (surface == 1)
					mesh.MaterialOverride = cached.WithoutFog;
				else
					mesh.SetSurfaceOverrideMaterial(surface, cached.WithoutFog);
			}
		}
		else
		{
			mesh.MaterialOverride = cached.WithFog;
		}
	}

	public void SetVisible(Node n, bool visible)
	{
		var sprite = n as Sprite3D;
		if (sprite != null)
		{
			if (visible)
			{
				if (!_colors.TryGetValue(sprite.Name, out var color))
				{
					color = sprite.Modulate;
					_colors[sprite.Name] = color;
				}

				sprite.Modulate = _colors[sprite.Name];
			}
			else
			{
				if (!_colors.TryGetValue(sprite.Name, out var color))
				{
					color = sprite.Modulate;
					_colors[sprite.Name] = color;
				}

				sprite.Modulate = new Color(0.25f, 0.25f, 0.25f);
			}
		}

		var mesh = n as MeshInstance3D;
		if (mesh != null)
			try
			{
				var ct = mesh.GetSurfaceOverrideMaterialCount();
				for (var c = 0; c < ct; c++)
				{
					var activeMat = mesh.GetActiveMaterial(c) as BaseMaterial3D;
					HandleMaterial(mesh, activeMat, visible, c);
				}
			}
			catch (Exception e)
			{
				ClientServices.Analytics.TrackError(e);
			}

		foreach (var child in n.GetChildren()) SetVisible(child, visible);
	}
}