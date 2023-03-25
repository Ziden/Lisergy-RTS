using Unity.Burst;
using UnityEngine;

public class PlaneMesh : MonoBehaviour
{
    public static byte[,] VertexArray = new byte[6, 6];
    private static Vector3[] _verticles;

    private Mesh _mesh;
    public float[,] Heights = new float[6, 6];

    // TODO: Make this run inside a burst compiled job
    [BurstCompile]
    public Mesh GenerateTileMesh(float[,] heights)
    {
        var xSize = 5;
        var ySize = 5;
        var mesh = new Mesh();
        mesh.name = "Procedural Grid";

        _verticles = new Vector3[(xSize + 1) * (ySize + 1)];
        var uv = new Vector2[_verticles.Length];
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                VertexArray[x, y] = (byte)i;
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
        mesh.RecalculateBounds();
        return mesh;
    }

    public void Render()
    {
        _mesh = GenerateTileMesh(Heights);
        GetComponent<MeshFilter>().mesh = _mesh;
        transform.localScale = new Vector3(0.2f, 1, 0.2f);
        _mesh.vertices = _verticles;
        _mesh.Optimize();
        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        transform.localPosition = new Vector3(-0.5f, 0, -0.5f);
    }
}
