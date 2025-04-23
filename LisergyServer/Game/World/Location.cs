using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Game.World
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Location
	{
		public ushort X;
		public ushort Y;

		public Location(in ushort x, in ushort y)
		{
			X = x;
			Y = y;
		}

		public Location(in int x, in int y)
		{
			X = (ushort) x;
			Y = (ushort) y;
		}

		public override bool Equals(object obj)
		{
			return obj is Location position &&
			       X == position.X &&
			       Y == position.Y;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(X, Y);
		}

		public override string ToString()
		{
			return $"(X={X} Y={Y})";
		}

		public static bool operator ==(in Location p1, in Location p2)
		{
			return p1.X == p2.X && p1.Y == p2.Y;
		}

		public static bool operator !=(in Location p1, in Location p2)
		{
			return p1.X != p2.X || p1.Y != p2.Y;
		}

		// Convert to Vector2
		public Vector2 ToVector2()
		{
			return new Vector2(X, Y);
		}

		// Convert to Vector3
		public Vector3 ToVector3(float z = 0)
		{
			return new Vector3(X, z, Y);
		}

		// Create Location from Vector2
		public static Location FromVector2(Vector2 vector)
		{
			return new Location((int) vector.X, (int) vector.Y);
		}

		// Create Location from Vector3
		public static Location FromVector3(Vector3 vector)
		{
			return new Location((int) vector.X, (int) vector.Z);
		}
	}
}