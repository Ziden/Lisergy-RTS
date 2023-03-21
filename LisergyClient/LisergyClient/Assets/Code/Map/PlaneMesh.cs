using Assets.Code.World;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaneMesh : MonoBehaviour
{
    Mesh _mesh;
    Vector3[] _verticles;
    Vector3[] _verticlePositions;
    Matrix4x4 _localToWorld;
    int[] _triangles;

    public static readonly byte[,] VertexArray = new byte[4, 4]
    {
        { 15, 14, 12, 13 },
        { 11, 10, 8, 9 },
        { 1, 3, 5, 7 },
        { 0, 2, 4, 6 },
    };

    public float[,] Heights = new float[4, 4]
    {
        { 0, 0, 0, 0 },
        { 0, 0, 0, 0 },
        { 0, 0, 0, 0 },
        { 0, 0, 0, 0 },
    };

    public void SetHeight(byte x, byte y, float h)
    {
        var index = VertexArray[x, y];
        var v = _verticles[index];
        _verticles[index] = new Vector3(v.x, h, v.z);
        Heights[x, y] = h;

        // DEBUG
        var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        s.transform.position = this.transform.TransformPoint(_verticles[index]);
        s.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        s.gameObject.name = $"{transform.position.x}/{x} - {transform.position.z}/{y}";
    }


    void Start()
    {
        /*
        _mesh = GetComponent<MeshFilter>().mesh;
        _verticles = _mesh.vertices;

        _localToWorld = transform.localToWorldMatrix;

        for (byte vx = 0; vx < VertexArray.GetLength(0); vx++)
        {
            for (byte vy = 0; vy < VertexArray.GetLength(1); vy++)
            {
                var noise = SectorGeneration.GetTileVertexHeight((int)transform.position.x, (int)transform.position.y, (byte)(4-vx), (byte)(vy));
                var height = (noise / 128) - 1;
                SetHeight(vx, vy, height);
            }
        }

        _mesh.vertices = _verticles;
        _mesh.RecalculateNormals();
        */
    }
}
