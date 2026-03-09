<Query Kind="Statements">
  <Namespace>System.Drawing</Namespace>
</Query>

//var file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-15\smallExample.txt");
//var file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-15\example.txt");
var file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-15\robotPlan.txt");

var mapLines = new List<string>();
var movementLines = new List<string>();

foreach (var line in file)
{
	if (line.Length == 0) continue;
	if (line.Substring(0, 1) == "#") mapLines.Add(line);
	if (line.Substring(0, 1) == "^" || line.Substring(0, 1) == "<" || line.Substring(0, 1) == ">" || line.Substring(0, 1) == "v")
		movementLines.Add(line);
}

var mapLoad = new string[mapLines[0].Length, mapLines.Count];
Position robotStart = null;
for (var y = 0; y < mapLines.Count; y++)
{
	var lineArray = mapLines[y].ToArray().Select(c => c.ToString()).ToArray();
	for (var x = 0; x < mapLines[0].Length; x++)
	{
		if (lineArray[x] == "@")
		{
			robotStart = new Position(x, y);
		}
		mapLoad[x, y] = lineArray[x];
	}
}

var map = new Map(mapLoad);

var moveList = movementLines.Select(l => l.ToArray()).SelectMany(a => a).Select(
c => c switch
{
	'^' => (0, -1),
	'<' => (-1, 0),
	'>' => (1, 0),
	'v' => (0, 1)
}).ToArray();

var robot = new Robot(robotStart, moveList, map);
//Console.WriteLine(map.Draw());
for (var m = 0; m < moveList.Length; m++)
{
	//Console.WriteLine(map.Draw());
	//Console.WriteLine($"{moveList[m] switch{(0, -1) => '^',(-1, 0) => '<',(1, 0) => '>',(0, 1) => 'v'}}");
	var next = robot.NextPosition;
	if (map[next] == "#") robot.Blocked();
	else if (map[next] == "O" && !MoveBox(next, moveList[m], map)) robot.Blocked();
	else robot.Move();
}
//Console.WriteLine(map.Draw());

var gpsSum = 0;
for (var y = 0; y < mapLoad.GetLength(1); y++)
{
	for (var x = 0; x < mapLoad.GetLength(0); x++)
	{
		if(map[x, y]=="O") gpsSum += 100*y+x;
	}
}

gpsSum.Dump();

bool MoveBox(Position next, (int, int) move, Map map)
{
	if (map[next + move] == "#") return false;
	if (map[next + move] == "O" && !MoveBox(next + move, move, map)) return false;
	map[next + move] = "O";
	map[next] = ".";
	return true;
}

public class Map(string[,] map)
{
	private string[,] data = map;

	public string this[int x, int y] { get => data[x, y]; set => data[x, y] = value; }
	public string this[Position p] { get => data[p.X, p.Y]; set => data[p.X, p.Y] = value; }
	public string this[(int x, int y) p] { get => data[p.Item1, p.Item2]; set => data[p.Item1, p.Item2] = value; }

	public string Draw()
	{
		var sb = new StringBuilder();
		for (var y = 0; y < data.GetLength(1); y++)
		{
			for (var x = 0; x < data.GetLength(0); x++)
			{
				sb.Append(data[x, y]);
			}
			sb.AppendLine();
		}
		return sb.ToString();
	}
}

public class Robot(Position startPos, (int, int)[] moveList, Map map)
{
	private (int, int)[] moves = moveList;
	public Position Location = startPos;
	private int moveIndex = 0;
	private Map map = map;

	public Position NextPosition => new Position(Location.X + moves[moveIndex].Item1, Location.Y + moves[moveIndex].Item2);

	public bool Move()
	{
		map[Location] = ".";
		map[NextPosition] = "@";
		Location = NextPosition;
		map.Draw();
		moveIndex++;
		return true;
	}

	public void Blocked()
	{
		moveIndex++;
	}
}

public class Position(int x, int y)
{
	public int X = x;
	public int Y = y;

	public override bool Equals(object? other)
	{
		var pos = other as Position;
		return other is null
			? false :
			pos.X == X
		   	&& pos.Y == Y;
	}

	public override int GetHashCode()
	{
		return (X.ToString("D3") + Y.ToString("D3")).GetHashCode();
	}

	public static bool operator ==(Position left, Position right)
	{
		return left is null ? right is null : left.Equals(right);
	}

	public static bool operator !=(Position left, Position right)
	{
		return !(left is null ? right is null : left == right);
	}

	public static Position operator +(Position p, (int x, int y) dir) => new Position(p.X + dir.Item1, p.Y + dir.Item2);
}