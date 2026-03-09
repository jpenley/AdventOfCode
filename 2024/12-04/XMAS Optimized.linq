<Query Kind="Statements" />

WordSearch.Main();

public class WordSearch
{
	private static char[,] grid;
	private static int maxRows;
	private static int maxCols;

	public static int CountXMAS(string filePath)
	{
		LoadGrid(filePath);

		int matches = 0;
		for (int r = 0; r < maxRows; r++)
		{
			for (int c = 0; c < maxCols; c++)
			{
				if (grid[r, c] == 'X')
				{
					matches += FindXmas(r, c);
				}
			}
		}

		return matches;
	}

	private static void LoadGrid(string filePath)
	{
		var lines = File.ReadAllLines(filePath);
		maxRows = lines.Length;
		maxCols = lines[0].Length;
		grid = new char[maxRows, maxCols];

		for (int row = 0; row < maxRows; row++)
		{
			for (int col = 0; col < maxCols; col++)
			{
				grid[row, col] = lines[row][col];
			}
		}
	}

	private static int FindXmas(int row, int col)
	{
		var directions = new List<(int, int)>
		{
			(-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)
		};

		int count = 0;
		foreach (var (rowIncrement, colIncrement) in directions)
		{
			string word = "";
			for (int i = 0; i < 4; i++)
			{
				int r = row + i * rowIncrement;
				int c = col + i * colIncrement;
				if (r >= 0 && r < maxRows && c >= 0 && c < maxCols)
				{
					word += grid[r, c];
				}
				else
				{
					break;
				}
			}
			if (word == "XMAS" || word == "SAMX")
			{
				count++;
			}
		}

		return count;
	}

	public static void Main()
	{
		string filePath = @"D:\Paylocity\advent2024\12-04\wordSearch.txt";
		int xmasCount = CountXMAS(filePath);
		Console.WriteLine($"XMAS appears {xmasCount} times.");
	}
}