<Query Kind="Statements" />

//string[] file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-08\example.txt");
string[] file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-08\map.txt");

int MaxRows = file.Length;
int MaxCols = file[0].Length;

var stations = new Dictionary<char, List<Position>>();
for (int r = 0; r < file.Length; r++)
{
	var cols = file[r].ToArray();
	for (int c = 0; c < cols.Length; c++)
	{
		if (cols[c] != '.')
		{
			if (!stations.ContainsKey(cols[c]))
			{
				stations[cols[c]] = new List<Position>();
			}
			stations[cols[c]].Add(new Position(r, c));
		}
	}
}

var antinodes = GetAntiNodes(stations, MaxRows, MaxCols);
Console.WriteLine($"There are {antinodes.Count} antinodes on the map.");

List<Position> GetAntiNodes(Dictionary<char, List<Position>> stations, int maxRows, int maxCols)
{
	var antinodes = new HashSet<Position>();
	foreach (var key in stations.Keys)
	{
		foreach (var station1 in stations[key])
		{
			antinodes.Add(station1);
			foreach (var station2 in stations[key])
			{
				antinodes.Add(station2);
				if (station1 == station2) continue;
				var rdist = station1.R - station2.R;
				var cdist = station1.C - station2.C;

				var onMap = true;
				var currentPostion = station1;
				while (onMap)
				{
					var antinode = new Position(currentPostion.R + rdist, currentPostion.C + cdist);
					if (antinode.R < maxRows && antinode.R > -1 && antinode.C < maxCols && antinode.C > -1)
					{
						antinodes.Add(antinode);
						currentPostion = antinode;
					}
					else
					{
						onMap = false;
					}
				}
				onMap = true;
				currentPostion = station2;
				while (onMap)
				{
					var antinode = new Position(currentPostion.R - rdist, currentPostion.C - cdist);
					if (antinode.R < maxRows && antinode.R > -1 && antinode.C < maxCols && antinode.C > -1)
					{
						antinodes.Add(antinode);
						currentPostion = antinode;
					}
					else
					{
						onMap = false;
					}
				}
			}
		}
	}
	return antinodes.ToList();
}

public struct Position(int r, int c)
{
	public int C = c;
	public int R = r;

	public override bool Equals(object? other)
	{
		return other is Position pos
			   && pos.C == C
			   && pos.R == R;
	}

	public override int GetHashCode()
	{
		return (C.ToString("D3") + R.ToString("D3")).GetHashCode();
	}

	public static bool operator ==(Position left, Position right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Position left, Position right)
	{
		return !(left == right);
	}
}