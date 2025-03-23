using System.Numerics;
using System;
using System.Threading.Tasks;
using Game.World;
using GameData.Specs;

public interface IGameObject
{
    public string Name { get; set; }
    public Location Location { get; set; }
}

public interface IAssetService 
{
    public Task<IGameObject> CreateArt(ArtSpec art, Location loc);
}

