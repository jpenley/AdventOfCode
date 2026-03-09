<Query Kind="Program" />

class Program
{
	// Your existing keypad layouts are correct
	static readonly char[,] numericKeypad = new char[4, 3]
	{
		{ '7', '8', '9' },
		{ '4', '5', '6' },
		{ '1', '2', '3' },
		{ 'X', '0', 'A' }
	};

	static readonly char[,] directionalKeypad = new char[2, 3]
	{
		{ 'X', '^', 'A' },
		{ '<', 'v', '>' }
	};

	static Dictionary<(char, char), List<List<char>>> numericLookup;
	static Dictionary<(char, char), List<List<char>>> directionalLookup;

	// Add method to find sequence for all three levels
	static int FindFullSequence(string targetCode)
	{
		// First find sequence for numeric keypad
		var numericSequence = FindNumericSequence(targetCode);

		// Then find sequence for second robot
		var secondRobotSequence = numericSequence.SelectMany(s => FindDirectionalSequence(s)).ToList();
		secondRobotSequence = secondRobotSequence.Where(s => s.Length == secondRobotSequence.Min(l => l.Length)).ToList();

		// Finally find sequence for first robot
		var firstRobotSequence = secondRobotSequence.SelectMany(s => FindDirectionalSequence(s)).ToList();

		var minSequence = firstRobotSequence.Min(s => s.Length);

		return minSequence;
	}

	static List<string> FindNumericSequence(string targetCode)
	{
		var npPaths = new List<string>();
		var position = 'A';
		foreach (var c in targetCode)
		{
			var paths = numericLookup[(position, c)].Select(p => new string(p.ToArray())).ToList();
			if (npPaths.Count == 0)
			{
				npPaths = paths.Select(p => p + "A").ToList();
			}
			else
			{
				var newPaths = new List<string>();
				foreach (var path in npPaths)
				{
					foreach (var addition in paths)
					{
						newPaths.Add(path + addition + "A");
					}
				}
				npPaths = newPaths;
			}
			position = c;
		}
		return npPaths;
	}

	static List<string> FindDirectionalSequence(string targetSequence)
	{
		var dpPaths = new List<string>();
		var position = 'A';
		foreach (var c in targetSequence)
		{
			var paths = directionalLookup[(position, c)].Select(p => new string(p.ToArray())).ToList();
			if (dpPaths.Count == 0)
			{
				dpPaths = paths.Select(p => p + "A").ToList();
			}
			else
			{
				var newPaths = new List<string>();
				foreach (var path in dpPaths)
				{
					if (paths.Count == 0)
					{
						newPaths.Add(path + "A");
					}
					else
					{
						foreach (var addition in paths)
						{
							newPaths.Add(path + addition + "A");
						}
					}
				}
				dpPaths = newPaths;
			}
			position = c;
		}

		return dpPaths;
	}

	static Dictionary<(char, char), List<List<char>>> PopulateLookups(char[,] grid)
	{
		var pathFinder = new GridPathFinder();
		var lookup = new Dictionary<(char, char), List<List<char>>>();
		for (var r1 = 0; r1 < grid.GetLength(0); r1++)
		{
			for (var c1 = 0; c1 < grid.GetLength(1); c1++)
			{
				for (var r2 = 0; r2 < grid.GetLength(0); r2++)
				{
					for (var c2 = 0; c2 < grid.GetLength(1); c2++)
					{
						if (grid[r1, c1] == 'X' || grid[r2, c2] == 'X') continue;
						var pair = (grid[r1, c1], grid[r2, c2]);
						if (grid[r1, c1] == grid[r2, c2])
						{
							lookup[pair] = new List<List<char>>();
						}
						else
						{
							lookup[pair] = pathFinder.FindAllShortestPaths(grid, pair);
						}
					}
				}
			}
		}
		return lookup;
	}

	static void Main()
	{
		numericLookup = PopulateLookups(numericKeypad);
		directionalLookup = PopulateLookups(directionalKeypad);

		string[] codes = new string[] { "879A", "508A", "463A", "593A", "189A" };
		int totalComplexity = 0;

		foreach (string code in codes)
		{
			int fullSequenceLength = FindFullSequence(code);
			int numericValue = int.Parse(code.TrimStart('0').TrimEnd('A'));
			int complexity = fullSequenceLength * numericValue;
			totalComplexity += complexity;

			Console.WriteLine($"Code: {code}");
			Console.WriteLine($"Sequence length: {fullSequenceLength}");
			Console.WriteLine($"Numeric value: {numericValue}");
			Console.WriteLine($"Complexity: {complexity}");
			Console.WriteLine();
		}

		Console.WriteLine($"Total complexity: {totalComplexity}");
	}
}

public class GridPathFinder
{
	private static readonly int[] dRow = { -1, 1, 0, 0 }; // Up, Down, Left, Right
	private static readonly int[] dCol = { 0, 0, -1, 1 };

	private int gridRows;
	private int gridCols;
	private char[,] grid;

	public List<List<char>> FindAllShortestPaths(char[,] grid, (char, char) pair)
	{
		gridRows = grid.GetLength(0);
		gridCols = grid.GetLength(1);
		this.grid = grid;

		var start = GetPos(pair.Item1);
		var end = GetPos(pair.Item2);

		Queue<List<((int, int), char)>> queue = new Queue<List<((int, int), char)>>();
		queue.Enqueue(new List<((int, int), char)> { (start, 'O') });

		// Keep track of visited cells for each path separately
		HashSet<(int, int)>[,] levelVisited = new HashSet<(int, int)>[gridRows, gridCols];
		for (int i = 0; i < gridRows; i++)
			for (int j = 0; j < gridCols; j++)
				levelVisited[i, j] = new HashSet<(int, int)>();

		List<List<((int, int), char)>> shortestPaths = new List<List<((int, int), char)>>();
		int shortestLength = int.MaxValue;

		while (queue.Count > 0)
		{
			var path = queue.Dequeue();
			var current = path[^1];

			if (current.Item1.Equals(end))
			{
				if (path.Count <= shortestLength)
				{
					if (path.Count < shortestLength)
					{
						shortestPaths.Clear();
						shortestLength = path.Count;
					}
					shortestPaths.Add(path);
				}
				continue;
			}

			if (path.Count > shortestLength) continue;

			for (int dir = 0; dir < 4; dir++)
			{
				int newRow = current.Item1.Item1 + dRow[dir];
				int newCol = current.Item1.Item2 + dCol[dir];

				if (IsValidMove(newRow, newCol, grid) &&
					!path.Any(p => p.Item1 == (newRow, newCol))) // Check if the position is not already in the current path
				{
					var newPath = new List<((int, int), char)>(path);
					newPath.Add(((newRow, newCol), DirectionToChar(dir)));
					queue.Enqueue(newPath);
				}
			}
		}

		return shortestPaths.Select(ps => ps.Select(p => p.Item2).Where(i => i != 'O').ToList()).ToList();
	}

	private char DirectionToChar(int dir)
	{
		return dir switch
		{
			0 => '^',
			1 => 'v',
			2 => '<',
			3 => '>'
		};
	}

	private (int, int) GetPos(char item)
	{
		for (var r = 0; r < gridRows; r++)
		{
			for (var c = 0; c < gridCols; c++)
			{
				if (grid[r, c] == item) return (r, c);
			}
		}
		return (-1, -1);
	}

	private static bool IsValidMove(int row, int col, char[,] grid)
	{
		return row >= 0 && row < grid.GetLength(0) &&
			   col >= 0 && col < grid.GetLength(1) &&
			   grid[row, col] != 'X';
	}
}

class Position
{
	public int Row { get; set; }
	public int Col { get; set; }

	public Position(int row, int col)
	{
		Row = row;
		Col = col;
	}

	public Position Clone()
	{
		return new Position(Row, Col);
	}
}