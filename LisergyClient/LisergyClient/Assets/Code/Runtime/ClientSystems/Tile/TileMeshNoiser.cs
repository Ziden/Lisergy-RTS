using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

/// <summary>
/// Generates the mesh of the tile base.
/// We can do a bit of noise in this mesh so the tile looks more "real" and not just a flag surface
/// </summary>
public class TileMeshNoiser : MonoBehaviour
{
    private static Dictionary<long, Mesh> _cache = new Dictionary<long, Mesh>();

    // TODO: Make this run inside a burst compiled job
    [BurstCompile]
    public Mesh GenerateTileMesh(float[,] heights)
    {
        var xSize = 5;
        var ySize = 5;
        var mesh = new Mesh();
        mesh.name = "Procedural Grid";

        var _verticles = new Vector3[(xSize + 1) * (ySize + 1)];
        var uv = new Vector2[_verticles.Length];
        var vertexBuffer = new byte[6, 6]; 
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertexBuffer[x, y] = (byte)i;
                _verticles[i] = new Vector3(x, heights[x, y], y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
            }
        }
        mesh.vertices = _verticles;
        mesh.uv = uv;

        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.Optimize();
        return mesh;
    }

    public void Adjust(float[,] heights, long hash)
    {
        var size = 1f / 5;
        transform.localScale = new Vector3(size, 1, size);
        transform.localPosition = new Vector3(-0.5f, 0, -0.5f);
        if (!_cache.TryGetValue(hash, out var mesh))
        {
            mesh = GenerateTileMesh(heights);
            _cache[hash] = mesh;
        }
        else
        {
            Debug.Log("Used cached mesh ! ");
        }
        GetComponent<MeshFilter>().mesh = mesh;

    }
}
