<Query Kind="Program" />

class Program
{
	// Point structure to represent coordinates
	public struct Point
	{
		public int Row { get; }
		public int Col { get; }

		public Point(int row, int col)
		{
			Row = row;
			Col = col;
		}
	}

	static void Main(string[] args)
	{
		// Read the maze from file
		string[] maze = File.ReadAllLines(@"E:\source\advent2024\12-20\maze.txt");
		var distances = CalculateDistances(maze);

		// Print the maze with distances
		PrintMazeWithDistances(maze, distances);
	}

	static Dictionary<Point, int> CalculateDistances(string[] maze)
	{
		int rows = maze.Length;
		int cols = maze[0].Length;
		var distances = new Dictionary<Point, int>();
		Point endPoint = FindChar(maze, 'E');

		// Initialize queue for BFS
		var queue = new Queue<Point>();
		queue.Enqueue(endPoint);
		distances[endPoint] = 0;

		// Possible moves: up, right, down, left
		int[] dx = { -1, 0, 1, 0 };
		int[] dy = { 0, 1, 0, -1 };

		while (queue.Count > 0)
		{
			var current = queue.Dequeue();
			int currentDistance = distances[current];

			// Check all four directions
			for (int i = 0; i < 4; i++)
			{
				int newRow = current.Row + dx[i];
				int newCol = current.Col + dy[i];
				var newPoint = new Point(newRow, newCol);

				// Check if the new position is valid and not visited
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
		return row >= 0 && row < maze.Length &&
			   col >= 0 && col < maze[0].Length;
	}

	static void PrintMazeWithDistances(string[] maze, Dictionary<Point, int> distances)
	{
		for (int i = 0; i < maze.Length; i++)
		{
			for (int j = 0; j < maze[i].Length; j++)
			{
				var point = new Point(i, j);
				if (maze[i][j] == '#')
				{
					Console.Write("# ".PadLeft(4));
				}
				else if (distances.ContainsKey(point))
				{
					Console.Write($"{distances[point]} ".PadLeft(4));
				}
				else
				{
					Console.Write("? ".PadLeft(4));
				}
			}
			Console.WriteLine();
		}
	}
}