<Query Kind="Program" />

class ReindeerMazeSolver
{
	// Directions: 0 = East, 1 = North, 2 = West, 3 = South
	private static readonly int[] dx = { 1, 0, -1, 0 };
	private static readonly int[] dy = { 0, -1, 0, 1 };

	class State
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int Direction { get; set; }
		public int Score { get; set; }
		public State Parent { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is State other)
			{
				return X == other.X && Y == other.Y && Direction == other.Direction;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(X, Y, Direction);
		}

		// Create a deep copy of the state
		public State Clone()
		{
			return new State
			{
				X = X,
				Y = Y,
				Direction = Direction,
				Score = Score,
				Parent = Parent
			};
		}
	}

	public static int FindBestPathTiles(string[] maze)
	{
		int rows = maze.Length;
		int cols = maze[0].Length;

		// Find start and end positions
		(int startX, int startY) = FindPosition(maze, 'S');
		(int endX, int endY) = FindPosition(maze, 'E');

		// Track best score and paths
		int bestScore = int.MaxValue;
		var allBestPaths = new HashSet<HashSet<(int x, int y)>>();

		// Data structure to track states at the end tile
		var endStates = new List<State>();

		// Track visited states with their scores
		var visited = new Dictionary<(int x, int y, int direction), int>();

		// Priority queue to store states
		var pq = new PriorityQueue<State, int>();

		// Initial state: start at 'S', facing East
		var startState = new State
		{
			X = startX,
			Y = startY,
			Direction = 0,
			Score = 0,
			Parent = null
		};
		pq.Enqueue(startState, 0);

		while (pq.Count > 0)
		{
			var current = pq.Dequeue();

			// Check if this state is worse than a previously found path to this location
			var stateKey = (current.X, current.Y, current.Direction);
			if (visited.TryGetValue(stateKey, out int previousBestScore) &&
				current.Score > previousBestScore)
			{
				continue;
			}
			visited[stateKey] = current.Score;

			// Found a path to the end
			if (current.X == endX && current.Y == endY)
			{
				// If this is a new best score, reset previous paths
				if (current.Score < bestScore)
				{
					bestScore = current.Score;
					allBestPaths.Clear();
					endStates.Clear();
				}

				// If this path matches the best score, track it
				if (current.Score == bestScore)
				{
					endStates.Add(current);
				}

				// Continue exploring to find all equivalent paths
				continue;
			}

			// Try moving forward
			int newX = current.X + dx[current.Direction];
			int newY = current.Y + dy[current.Direction];

			if (IsValidMove(maze, newX, newY))
			{
				var forward = current.Clone();
				forward.X = newX;
				forward.Y = newY;
				forward.Score += 1;
				forward.Parent = current;
				pq.Enqueue(forward, forward.Score);
			}

			// Try rotating clockwise and counterclockwise
			var clockwise = current.Clone();
			clockwise.Direction = (current.Direction + 1) % 4;
			clockwise.Score += 1000;
			clockwise.Parent = current;
			pq.Enqueue(clockwise, clockwise.Score);

			var counterClockwise = current.Clone();
			counterClockwise.Direction = (current.Direction - 1 + 4) % 4;
			counterClockwise.Score += 1000;
			counterClockwise.Parent = current;
			pq.Enqueue(counterClockwise, counterClockwise.Score);
		}

		// Reconstruct paths for all end states
		foreach (var endState in endStates)
		{
			var pathTiles = ReconstructPath(endState);
			allBestPaths.Add(pathTiles);
		}

		// Merge all best paths
		var uniqueBestPathTiles = new HashSet<(int x, int y)>();
		foreach (var path in allBestPaths)
		{
			uniqueBestPathTiles.UnionWith(path);
		}

		// Print out the maze with best path tiles
		PrintMazeWithBestPaths(maze, uniqueBestPathTiles);

		return uniqueBestPathTiles.Count;
	}

	private static HashSet<(int x, int y)> ReconstructPath(State endState)
	{
		var pathTiles = new HashSet<(int x, int y)>();

		// Traverse back through parent states
		var current = endState;
		while (current != null)
		{
			pathTiles.Add((current.X, current.Y));
			current = current.Parent;
		}

		return pathTiles;
	}

	private static void PrintMazeWithBestPaths(string[] maze, HashSet<(int x, int y)> bestPathTiles)
	{
		for (int y = 0; y < maze.Length; y++)
		{
			for (int x = 0; x < maze[y].Length; x++)
			{
				if (bestPathTiles.Contains((x, y)) &&
					maze[y][x] != '#' &&
					maze[y][x] != 'S' &&
					maze[y][x] != 'E')
				{
					Console.Write('O');
				}
				else
				{
					Console.Write(maze[y][x]);
				}
			}
			Console.WriteLine();
		}
	}

	private static bool IsValidMove(string[] maze, int x, int y)
	{
		return x >= 0 && x < maze[0].Length &&
			   y >= 0 && y < maze.Length &&
			   maze[y][x] != '#';
	}

	private static (int x, int y) FindPosition(string[] maze, char target)
	{
		for (int y = 0; y < maze.Length; y++)
		{
			for (int x = 0; x < maze[y].Length; x++)
			{
				if (maze[y][x] == target)
					return (x, y);
			}
		}
		throw new ArgumentException($"Position '{target}' not found in maze");
	}

	static void Main(string[] args)
	{
		//var map = File.ReadAllLines(@"D:\Paylocity\advent2024\12-16\example1.txt");// Output: 7036
		//var map = File.ReadAllLines(@"D:\Paylocity\advent2024\12-16\example2.txt");// Output: 11048
		var map = File.ReadAllLines(@"D:\Paylocity\advent2024\12-16\maze.txt");

		Console.WriteLine("Maze Best Path Tiles:");
		int bestPathTiles1 = FindBestPathTiles(map);
		Console.WriteLine($"Number of tiles on best paths: {bestPathTiles1}");
	}
}