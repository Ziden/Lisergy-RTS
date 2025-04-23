using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace LisergyGodotClient.Src.Systems.Visualization;

public class PathItem
{
	public MeshInstance3D Mesh;
	public Vector2 Position;
}

/// <summary>
///     Handles drawing dotted path lines over a tilemap
/// </summary>
public partial class PathVisualizer : Node3D
{
	private readonly List<PathItem> _dots = new();
	private TimeSpan _delay;

	private StandardMaterial3D _mat;
	private SphereMesh _mesh;

	private Task _removal;
	[Export] public float DotSize = 0.05f; // Size of each dot
	[Export] public float DotSpacing = 0.2f; // Space between dots
	[Export] public float LineHeight = 0.1f; // Height above the tilemap
	[Export] public Color PathColor = Colors.Yellow; // Color of the path

	public int Remaining => _dots.Count;

	/// <summary>
	///     Clears the current path visualization
	/// </summary>
	public void ClearPath()
	{
		foreach (var dot in _dots) dot.Mesh.QueueFree();
		_dots.Clear();
		_removal?.Dispose();
		_removal = null;
	}

	/// <summary>
	///     Clears the current path visualization
	/// </summary>
	public void FinishMovement(Vector2 v)
	{
		var finished = _dots.Where(d => d.Position == v).ToArray();
		foreach (var f in finished)
		{
			f.Mesh.QueueFree();
			_dots.Remove(f);
		}
	}

	/// <summary>
	///     Removes dots that the entity has passed through based on its current position
	/// </summary>
	public void UpdatePathProgress(Vector2 currentPosition, float threshold = 0.3f)
	{
		if (_dots.Count == 0)
			return;

		var dotsToRemove = _dots
			.Where(d => d.Position.DistanceTo(currentPosition) < threshold)
			.ToArray();

		foreach (var dot in dotsToRemove)
		{
			dot.Mesh.QueueFree();
			_dots.Remove(dot);
		}
	}

	private async Task RemovalTask()
	{
		while (_dots.Count > 0)
		{
			var last = _dots.First();
			last.Mesh.QueueFree();
			_dots.Remove(last);
			await Task.Delay(_delay);
		}
	}


	public void StartMovement()
	{
		if (_removal == null) _removal = RemovalTask();
	}

	/// <summary>
	///     Draws a dotted path along the specified points
	/// </summary>
	/// <param name="pathPoints">Array of tile positions in Vector2 format</param>
	public void DrawPath(Vector2[] pathPoints, TimeSpan moveDelay)
	{
		ClearPath();
		if (pathPoints == null || pathPoints.Length < 2)
			return;

		// Create a standard sphere mesh for the dots
		if (_mesh == null)
		{
			_mesh = new SphereMesh();
			_mesh.Radius = DotSize / 2;
			_mesh.Height = DotSize;
		}

		// Create a material for the dots
		if (_mat == null)
		{
			_mat = new StandardMaterial3D();
			_mat.AlbedoColor = PathColor;
			_mat.EmissionEnabled = true;
			_mat.Emission = PathColor;
			_mat.EmissionEnergyMultiplier = 1.5f;
		}


		// Connect the points with dotted lines
		for (var i = 0; i < pathPoints.Length - 1; i++)
		{
			var point = pathPoints[i];
			var start = new Vector3(pathPoints[i].X, LineHeight, pathPoints[i].Y);
			var end = new Vector3(pathPoints[i + 1].X, LineHeight, pathPoints[i + 1].Y);

			// Calculate direction and distance
			var direction = (end - start).Normalized();
			var distance = start.DistanceTo(end);

			// Place dots along the line
			var dotsCount = Mathf.FloorToInt(distance / DotSpacing);
			for (var j = 0; j <= dotsCount; j++)
			{
				// Skip the last dot if we're not at the end of the path
				if (j == dotsCount && i < pathPoints.Length - 2)
					continue;


				// Calculate dot position
				var t = j * DotSpacing / distance;
				var dotPosition = start.Lerp(end, t);

				// Create and add the do
				var o = new MeshInstance3D();
				o.Mesh = _mesh;
				o.MaterialOverride = _mat;
				o.Position = dotPosition;

				AddChild(o);
				_dots.Add(new PathItem
				{
					Mesh = o,
					Position = point
				});
			}
		}

		var totalTime = pathPoints.Length * moveDelay - moveDelay;
		var timePerDot = totalTime / _dots.Count;
		_delay = timePerDot;
	}
}