using Godot;
using LisergyGodotClient.Src;
using System.Collections.Generic;

public class CachedFogMaterials
{
    public StandardMaterial3D WithFog;
    public StandardMaterial3D WithoutFog;
}

public class CachedFogSprites
{
    public Sprite3D WithFog;
    public Sprite3D WithoutFog;

    public CachedFogMaterials Materials;
}

public class TileFog
{
    public Dictionary<string, CachedFogMaterials> _originals = new Dictionary<string, CachedFogMaterials>();
    public Dictionary<string, CachedFogSprites> _spriteOriginals = new Dictionary<string, CachedFogSprites>();


    public void SetVisible(Node n, bool visible)
    {
        var sprite = n as Sprite3D;
        if(sprite != null)
        {
            if (visible)
            {
                //sprite.MaterialOverride = cached.WithoutFog.MaterialOverride;
                sprite.Modulate = new Color(1, 1, 1, 1);
            }
            else
            {
                //sprite.MaterialOverride = cached.WithFog.MaterialOverride; ;
                sprite.Modulate = new Color(0.25f, 0.25f, 0.25f, 1);
            }
        }

        var mesh = n as MeshInstance3D;
        if (mesh != null)
        {
            var mat = mesh.GetActiveMaterial(0) as StandardMaterial3D;
            var key = mat.AlbedoTexture.ResourcePath;
            if (!_originals.TryGetValue(key, out var cached))
            {
                var prevColor = mat.AlbedoColor;
                cached = new CachedFogMaterials()
                {
                    WithoutFog = mat,
                    WithFog = mat.Duplicate() as StandardMaterial3D
                };
                cached.WithFog.AlbedoColor = new Color(0.25f, 0.25f, 0.25f, 1);
                cached.WithoutFog.AlbedoColor = prevColor;
                _originals[key] = cached;
            }
            if (visible)
            {
                mesh.MaterialOverride = cached.WithoutFog;
            }
            else
            {
                mesh.MaterialOverride = cached.WithFog;
            }
        }
        foreach(var child in n.GetChildren())
        {
            SetVisible(child, visible);
        }
    }

    public TileFog(int mapWidth, int mapHeight, Node parent)
    {

    }
}
