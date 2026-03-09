<Query Kind="Statements" />

//var map = File.ReadAllLines(@"D:\Paylocity\advent2024\12-12\example.txt").Select(l => l.ToArray()).ToArray();
var map = File.ReadAllLines(@"D:\Paylocity\advent2024\12-12\regions.txt").Select(l => l.ToArray()).ToArray();

var plots = new Dictionary<int, HashSet<Position>>();
var totalPrice = 0;
GetPlots(map, plots);

Console.WriteLine($"There are {plots.Count} plots with a total area of {plots.Select(p => p.Value.Count).Sum()}");
foreach (var plot in plots.Values)
{
	var perimeter = plot.Select(p =>
	{
		var count = 0;
		if (!plot.Contains(p.Up)) count++;
		if (!plot.Contains(p.Down)) count++;
		if (!plot.Contains(p.Left)) count++;
		if (!plot.Contains(p.Right)) count++;
		return count;
	}).Sum();
	var corners = FindCorners(plot);
	totalPrice += corners * plot.Count;
	Console.WriteLine($"A region of {plot.First().Value} plants with {corners} sides and area of {plot.Count} for a price of {corners * plot.Count}");
}
Console.WriteLine($"For a total price of {totalPrice}");

int FindCorners(HashSet<Position> plot)
{
	var corners = 0;
	foreach (var p in plot)
	{
		var left = p.Left?.Value == p.Value;
		var leftup = p.Left?.Up?.Value == p.Value;
		var up = p.Up?.Value == p.Value;
		var upright = p.Up?.Right?.Value == p.Value;
		var right = p.Right?.Value == p.Value;
		var rightdown = p.Right?.Down?.Value == p.Value;
		var down = p.Down?.Value == p.Value;
		var downleft = p.Down?.Left?.Value == p.Value;
		
		if(!left && !up) corners++;
		if(!up && !right) corners++;
		if(!right && !down) corners++;
		if(!down && !left) corners++;

		if (left && !leftup && up) corners++;
		if (up && !upright && right) corners++;
		if (right && !rightdown && down) corners++;
		if (down && !downleft && left) corners++;
	}
	return corners;
}


void GetPlots(char[][] map, Dictionary<int, HashSet<Position>> plots)
{
	int maxRows = map.Length;
	int maxCols = map[0].Length;
	var plotID = 0;

	for (var r = 0; r < maxRows; r++)
	{
		for (var c = 0; c < maxCols; c++)
		{
			var pos = new Position(r, c, map);
			if (plots.Any(p => p.Value.Contains(pos))) continue;
			plots[plotID] = new HashSet<Position>();
			plots[plotID].Add(pos);
			MapPlot(pos, plots[plotID]);
			plotID++;
		}
	}
}

void MapPlot(Position pos, HashSet<Position> plot)
{
	if (pos.Up?.Value == pos.Value && !plot.Contains(pos.Up))
	{
		plot.Add(pos.Up);
		MapPlot(pos.Up, plot);
	}
	if (pos.Right?.Value == pos.Value && !plot.Contains(pos.Right))
	{
		plot.Add(pos.Right);
		MapPlot(pos.Right, plot);
	}
	if (pos.Down?.Value == pos.Value && !plot.Contains(pos.Down))
	{
		plot.Add(pos.Down);
		MapPlot(pos.Down, plot);
	}
	if (pos.Left?.Value == pos.Value && !plot.Contains(pos.Left))
	{
		plot.Add(pos.Left);
		MapPlot(pos.Left, plot);
	}
}

public class Position(int r, int c, char[][] map)
{
	private char[][] Map = map;
	private int maxRows = map.Length;
	private int maxCols = map[0].Length;
	private Position _up, _down, _left, _right;

	public int C = c;
	public int R = r;
	public char Value = map[r][c];

	public Position Up => _up ??= GetDir(R - 1, C);
	public Position Right => _right ??= GetDir(R, C + 1);
	public Position Down => _down ??= GetDir(R + 1, C);
	public Position Left => _left ??= GetDir(R, C - 1);

	private Position GetDir(int r, int c)
	{
		return r > -1 && c > -1 && r < maxRows && c < maxCols ? new Position(r, c, map) : null;
	}

	public override bool Equals(object? other)
	{
		var pos = other as Position;
		return other is null
			? false :
			pos.C == C
		   	&& pos.R == R;
	}

	public override int GetHashCode()
	{
		return (C.ToString("D3") + R.ToString("D3")).GetHashCode();
	}

	public static bool operator ==(Position left, Position right)
	{
		return left is null ? right is null : left.Equals(right);
	}

	public static bool operator !=(Position left, Position right)
	{
		return !(left is null ? right is null : left == right);
	}
}