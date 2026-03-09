<Query Kind="Program" />

class Program
{
	public struct Point : IEquatable<Point>
	{
		public int Row { get; }
		public int Col { get; }

		public Point(int row, int col)
		{
			Row = row;
			Col = col;
		}

		public override string ToString()
		{
			return $"({Row}, {Col})";
		}

		public bool Equals(Point other)
		{
			return Row == other.Row && Col == other.Col;
		}

		public override bool Equals(object obj)
		{
			if (obj is Point other)
				return Equals(other);
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Row, Col);
		}
	}

	public struct ShortcutPair
	{
		public Point First { get; }
		public Point Second { get; }

		public ShortcutPair(Point p1, Point p2)
		{
			// Ensure consistent ordering for comparison
			if (p1.Row < p2.Row || (p1.Row == p2.Row && p1.Col <= p2.Col))
			{
				First = p1;
				Second = p2;
			}
			else
			{
				First = p2;
				Second = p1;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is ShortcutPair other)
			{
				return First.Equals(other.First) && Second.Equals(other.Second);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(First, Second);
		}

		public override string ToString()
		{
			return $"{First} <-> {Second}";
		}
	}

	static void Main(string[] args)
	{
		string[] maze = File.ReadAllLines(@"E:\source\advent2024\12-20\maze.txt");
		var distances = CalculateDistances(maze);
		Point start = FindChar(maze, 'S');
		Point end = FindChar(maze, 'E');

		// Find normal shortest path
		var normalPath = FindShortestPath(maze, distances, start, end);
		int normalDistance = normalPath.Count - 1;

		Console.WriteLine($"Normal shortest path length: {normalDistance}");

		// Find potential shortcuts
		var shortcuts = FindPotentialShortcuts(maze, distances);
		var validShortcuts = new HashSet<ShortcutPair>();

		foreach (var shortcut in shortcuts)
		{
			// Calculate new path length using this shortcut
			int shortcutPathLength = CalculateShortcutPathLength(maze, distances, start, end, shortcut);

			if (shortcutPathLength > 0 && normalDistance - shortcutPathLength >= 100)
			{
				validShortcuts.Add(shortcut);
				Console.WriteLine($"Found valid shortcut: {shortcut}");
				Console.WriteLine($"New path length with shortcut: {shortcutPathLength}");
				Console.WriteLine($"Steps saved: {normalDistance - shortcutPathLength}");
			}
		}

		Console.WriteLine($"\nTotal number of unique shortcuts saving at least 100 steps: {validShortcuts.Count}");
	}

	static Dictionary<Point, int> CalculateDistances(string[] maze)
	{
		int rows = maze.Length;
		int cols = maze[0].Length;
		var distances = new Dictionary<Point, int>();
		Point endPoint = FindChar(maze, 'E');

		var queue = new Queue<Point>();
		queue.Enqueue(endPoint);
		distances[endPoint] = 0;

		int[] dx = { -1, 0, 1, 0 };
		int[] dy = { 0, 1, 0, -1 };

		while (queue.Count > 0)
		{
			var current = queue.Dequeue();
			int currentDistance = distances[current];

			for (int i = 0; i < 4; i++)
			{
				int newRow = current.Row + dx[i];
				int newCol = current.Col + dy[i];
				var newPoint = new Point(newRow, newCol);

				if (IsValidPosition(maze, newRow, newCol) &&
					!distances.ContainsKey(newPoint) &&
					maze[newRow][newCol] != '#')
				{
					distances[newPoint] = currentDistance + 1;
					queue.Enqueue(newPoint);
				}
			}
		}

		return distances;
	}

	static List<Point> FindShortestPath(string[] maze, Dictionary<Point, int> distances, Point start, Point end)
	{
		var path = new List<Point>();
		var current = start;
		path.Add(current);

		int[] dx = { -1, 0, 1, 0 };
		int[] dy = { 0, 1, 0, -1 };

		while (!current.Equals(end))
		{
			Point? bestNext = null;
			int bestDistance = int.MaxValue;

			for (int i = 0; i < 4; i++)
			{
				int newRow = current.Row + dx[i];
				int newCol = current.Col + dy[i];
				var newPoint = new Point(newRow, newCol);

				if (IsValidPosition(maze, newRow, newCol) &&
					distances.ContainsKey(newPoint) &&
					distances[newPoint] < bestDistance)
				{
					bestDistance = distances[newPoint];
					bestNext = newPoint;
				}
			}

			if (!bestNext.HasValue) break;
			current = bestNext.Value;
			path.Add(current);
		}

		return path;
	}

	static HashSet<ShortcutPair> FindPotentialShortcuts(string[] maze, Dictionary<Point, int> distances)
	{
		var shortcuts = new HashSet<ShortcutPair>();
		var points = distances.Keys.ToList();

		for (int i = 0; i < points.Count; i++)
		{
			for (int j = i + 1; j < points.Count; j++)
			{
				var p1 = points[i];
				var p2 = points[j];

				// Calculate Manhattan distance
				int manhattanDistance = Math.Abs(p1.Row - p2.Row) + Math.Abs(p1.Col - p2.Col);

				if (manhattanDistance <= 21 && !p1.Equals(p2))
				{
					shortcuts.Add(new ShortcutPair(p1, p2));
				}
			}
		}

		return shortcuts;
	}

	static int CalculateShortcutPathLength(string[] maze, Dictionary<Point, int> distances,
		Point start, Point end, ShortcutPair shortcut)
	{
		// Calculate distances from start to both shortcut points
		int distToP1 = CalculateDistance(start, shortcut.First, distances);
		int distToP2 = CalculateDistance(start, shortcut.Second, distances);

		// Calculate distances from both shortcut points to end
		int distFromP1ToEnd = CalculateDistance(shortcut.First, end, distances);
		int distFromP2ToEnd = CalculateDistance(shortcut.Second, end, distances);

		// Find the shortest path using the shortcut
		int pathWithShortcut = Math.Min(
			distToP1 + 1 + distFromP2ToEnd,  // Path through p1 to p2
			distToP2 + 1 + distFromP1ToEnd   // Path through p2 to p1
		);

		return pathWithShortcut;
	}

	static int CalculateDistance(Point from, Point to, Dictionary<Point, int> distances)
	{
		if (!distances.ContainsKey(from) || !distances.ContainsKey(to))
			return int.MaxValue;

		return Math.Abs(distances[from] - distances[to]);
	}

	static Point FindChar(string[] maze, char target)
	{
		for (int i = 0; i < maze.Length; i++)
		{
			for (int j = 0; j < maze[i].Length; j++)
			{
				if (maze[i][j] == target)
				{
					return new Point(i, j);
				}
			}
		}
		throw new Exception($"Character {target} not found in maze");
	}

	static bool IsValidPosition(string[] maze, int row, int col)
	{
		return row >= 0 && row < maze.Length && col >= 0 && col < maze[0].Length;
	}
}