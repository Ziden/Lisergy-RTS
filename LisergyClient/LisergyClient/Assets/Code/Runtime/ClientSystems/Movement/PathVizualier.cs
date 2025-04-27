using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PathVisualizer 
{
    private class PathItem
    {
        public GameObject DotObject;
        public Vector2 Position;
    }

    private readonly List<PathItem> _dots = new();
    private float _delaySeconds;
    private UniTask _removalTask;
    public float DotSize = 0.05f;
    public float DotSpacing = 0.2f;
    public float LineHeight = 0.1f;
    public Color PathColor = Color.yellow;

    private Material _material;
    private Mesh _sphereMesh;

    public int Remaining => _dots.Count;

    public PathVisualizer()
    {
        // Create shared sphere mesh and material once
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _sphereMesh = sphere.GetComponent<MeshFilter>().sharedMesh;
        UnityEngine.Object.Destroy(sphere); // Just keep the mesh reference, destroy the temp object

        _material = new Material(Shader.Find("Standard"));
        _material.color = PathColor;
        _material.EnableKeyword("_EMISSION");
        _material.SetColor("_EmissionColor", PathColor * 1.5f);
    }

    public void ClearPath()
    {
        foreach (var dot in _dots)
        {
            if (dot.DotObject != null)
                UnityEngine.Object.Destroy(dot.DotObject);
        }
        _dots.Clear();

        _removalTask.AsTask()?.Dispose();
        _removalTask = default;
    }

    public void FinishMovement(Vector2 position)
    {
        var finished = _dots.Where(d => d.Position == position).ToArray();
        foreach (var f in finished)
        {
            UnityEngine.Object.Destroy(f.DotObject);
            _dots.Remove(f);
        }
    }

    public void UpdatePathProgress(Vector2 currentPosition, float threshold = 0.3f)
    {
        if (_dots.Count == 0)
            return;

        var toRemove = _dots
            .Where(d => Vector2.Distance(d.Position, currentPosition) < threshold)
            .ToArray();

        foreach (var dot in toRemove)
        {
            UnityEngine.Object.Destroy(dot.DotObject);
            _dots.Remove(dot);
        }
    }

    private async UniTask RemovalTask()
    {
        while (_dots.Count > 0)
        {
            var first = _dots[0];
            if (first.DotObject != null)
                UnityEngine.Object.Destroy(first.DotObject);

            _dots.RemoveAt(0);
            await UniTask.Delay(TimeSpan.FromSeconds(_delaySeconds));
        }
    }

    public void StartMovement()
    {
        if(_removalTask.Status != UniTaskStatus.Pending)
            _removalTask = RemovalTask();
    }

    public void DrawPath(Vector2[] pathPoints, TimeSpan moveDelay)
    {
        ClearPath();

        if (pathPoints == null || pathPoints.Length < 2)
            return;

        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            Vector3 start = new Vector3(pathPoints[i].x, LineHeight, pathPoints[i].y);
            Vector3 end = new Vector3(pathPoints[i + 1].x, LineHeight, pathPoints[i + 1].y);

            Vector3 direction = (end - start).normalized;
            float distance = Vector3.Distance(start, end);

            int dotsCount = Mathf.FloorToInt(distance / DotSpacing);
            for (int j = 0; j <= dotsCount; j++)
            {
                if (j == dotsCount && i < pathPoints.Length - 2)
                    continue;

                float t = (j * DotSpacing) / distance;
                Vector3 dotPosition = Vector3.Lerp(start, end, t);

                GameObject dot = new GameObject("PathDot");
                var meshFilter = dot.AddComponent<MeshFilter>();
                var meshRenderer = dot.AddComponent<MeshRenderer>();

                meshFilter.sharedMesh = _sphereMesh;
                meshRenderer.sharedMaterial = _material;
                dot.transform.localScale = Vector3.one * DotSize;
                dot.transform.position = dotPosition;
                _dots.Add(new PathItem
                {
                    DotObject = dot,
                    Position = pathPoints[i]
                });
            }
        }

        float totalTimeSeconds = (float)(pathPoints.Length * moveDelay.TotalSeconds - moveDelay.TotalSeconds);
        _delaySeconds = totalTimeSeconds / _dots.Count;
    }
}
