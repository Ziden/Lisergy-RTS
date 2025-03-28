using Godot;
using System.Collections.Generic;
using System.Linq;

namespace LisergyGodotClient.Src.Systems.Visualization
{
    public class PathItem
    {
        public MeshInstance3D Mesh;
        public Vector2 Position;
    }

    /// <summary>
    /// Handles drawing dotted path lines over a tilemap
    /// </summary>
    public partial class PathVisualizer : Node3D
    {
        [Export] public float LineHeight = 0.1f;         // Height above the tilemap
        [Export] public float DotSize = 0.1f;            // Size of each dot
        [Export] public float DotSpacing = 0.4f;         // Space between dots
        [Export] public Color PathColor = Colors.Yellow; // Color of the path

        public int Remaining => _dots.Count;
        private List<PathItem> _dots = new List<PathItem>();

        /// <summary>
        /// Clears the current path visualization
        /// </summary>
        public void ClearPath()
        {
            foreach (var dot in _dots)
            {
                dot.Mesh.QueueFree();
            }
            _dots.Clear();
        }

        /// <summary>
        /// Clears the current path visualization
        /// </summary>
        public void FinishMovement(Vector2 v)
        {
            var finished = _dots.Where(d => d.Position == v).ToArray();
            foreach(var f in finished)
            {
                f.Mesh.QueueFree();
                _dots.Remove(f);
            }
        }

        /// <summary>
        /// Draws a dotted path along the specified points
        /// </summary>
        /// <param name="pathPoints">Array of tile positions in Vector2 format</param>
        public void DrawPath(Vector2[] pathPoints)
        {
            ClearPath();

            if (pathPoints == null || pathPoints.Length < 2)
                return;

            // Create a standard sphere mesh for the dots
            SphereMesh dotMesh = new SphereMesh();
            dotMesh.Radius = DotSize / 2;
            dotMesh.Height = DotSize;

            // Create a material for the dots
            StandardMaterial3D material = new StandardMaterial3D();
            material.AlbedoColor = PathColor;
            material.EmissionEnabled = true;
            material.Emission = PathColor;
            material.EmissionEnergyMultiplier = 1.5f;

            // Connect the points with dotted lines
            for (int i = 0; i < pathPoints.Length - 1; i++)
            {
                var point = pathPoints[i];
                Vector3 start = new Vector3(pathPoints[i].X, LineHeight, pathPoints[i].Y);
                Vector3 end = new Vector3(pathPoints[i + 1].X, LineHeight, pathPoints[i + 1].Y);

                // Calculate direction and distance
                Vector3 direction = (end - start).Normalized();
                float distance = start.DistanceTo(end);

                // Place dots along the line
                int dotsCount = Mathf.FloorToInt(distance / DotSpacing);
                for (int j = 0; j <= dotsCount; j++)
                {
                    // Skip the last dot if we're not at the end of the path
                    if (j == dotsCount && i < pathPoints.Length - 2)
                        continue;

                    // Calculate dot position
                    float t = j * DotSpacing / distance;
                    Vector3 dotPosition = start.Lerp(end, t);

                    // Create and add the dot
                    MeshInstance3D dot = new MeshInstance3D();
                    dot.Mesh = dotMesh;
                    dot.MaterialOverride = material;
                    dot.Position = dotPosition;

                    AddChild(dot);
                    _dots.Add(new PathItem()
                    {
                        Mesh = dot,
                        Position = point
                    });
                }
            }
        }
    }
}
