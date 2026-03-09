<Query Kind="Statements">
  <Namespace>System.Drawing</Namespace>
</Query>

//var file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-15\smallExample.txt");
var file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-15\example.txt");
//var file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-15\robotPlan.txt");
var part = 1;

var mapLines = new List<string>();
var movementLines = new List<string>();

foreach (var line in file)
{
	if (line.Length == 0) continue;
	if (line.Substring(0, 1) == "#") mapLines.Add(line);
	if (line.Substring(0, 1) == "^" || line.Substring(0, 1) == "<" || line.Substring(0, 1) == ">" || line.Substring(0, 1) == "v")
		movementLines.Add(line);
}

var moveList = movementLines.Select(l => l.ToArray()).SelectMany(a => a).Select(
c => c switch
{
	'^' => (0, -1),
	'<' => (-1, 0),
	'>' => (1, 0),
	'v' => (0, 1)
}).ToArray();

var mapLoad = new Dictionary<Position, MapObject>();
var map = new Map(mapLoad, mapLines[0].Length * part, mapLines.Count);
for (var y = 0; y < mapLines.Count; y++)
{
	var lineArray = mapLines[y].ToArray().Select(c => c.ToString()).ToArray();
	for (var x = 0; x < mapLines[0].Length; x++)
	{
		if (part == 1)
		{
			var currentPosition = new Position(x, y);
			switch (lineArray[x])
			{
				case "@":
					mapLoad.Add(currentPosition, new Robot(moveList, "@", currentPosition, map, part));
					break;
				case "#":
					mapLoad.Add(currentPosition, new Wall(currentPosition, "#", map, part));
					break;
				case "O":
					mapLoad.Add(currentPosition, new Box(currentPosition, "O", map, part));
					break;
				default:
					mapLoad.Add(currentPosition, new Nothing(currentPosition, ".", map, part));
					break;
			};
		}
		else
		{
			var currentPosition = new Position(x * 2, y);
			switch (lineArray[x])
			{
				case "@":
					mapLoad.Add(currentPosition, new Robot(moveList, "@", currentPosition, map, part));
					mapLoad.Add(currentPosition + (1, 0), new Nothing(currentPosition + (1, 0), ".", map, part));
					break;
				case "#":
					mapLoad.Add(currentPosition, new Wall(currentPosition, "#", map, part));
					mapLoad.Add(currentPosition + (1, 0), new Wall(currentPosition, "#", map, part));
					break;
				case "O":
					mapLoad.Add(currentPosition, new Box(currentPosition, "[", map, part));
					mapLoad.Add(currentPosition, new Box(currentPosition, "]", map, part));
					break;
				default:
					mapLoad.Add(currentPosition, new Nothing(currentPosition, ".", map, part));
					mapLoad.Add(currentPosition + (1, 0), new Nothing(currentPosition + (1, 0), ".", map, part));
					break;
			};
		}
	}
}

var robot = (Robot)mapLoad.First(m => m.Value is Robot).Value;

Util.AutoScrollResults = true;
Console.WriteLine(map.Draw());
while (robot.HasInstructions)
{
	robot.Move();
	Console.WriteLine(map.Draw());
}
Console.WriteLine(map.Draw());

var gpsSum = map.Items.Values.Where(v => v is Box).Select(i => i.Location.Y * 100 + i.Location.X).Sum();
gpsSum.Dump();

public class Map(Dictionary<Position, MapObject> map, int maxX, int maxY)
{
	public Dictionary<Position, MapObject> Items = map;
	public MapObject this[int x, int y] { get => Items[new Position(x, y)]; set => Items[new Position(x, y)] = value; }
	public MapObject this[Position p] { get => Items[p]; set => Items[p] = value; }
	public MapObject this[(int x, int y) p] { get => Items[new Position(p.Item1, p.Item2)]; set => Items[new Position(p.Item1, p.Item2)] = value; }
	private int maxX = maxX;
	private int maxY = maxY;

	public string Draw()
	{
		var sb = new StringBuilder();

		for (var y = 0; y < maxY; y++)
		{
			for (var x = 0; x < maxX; x++)
			{
				var currentPosition = new Position(x, y);
				if (Items.ContainsKey(currentPosition))
				{
					var mapObject = Items[currentPosition];
					sb.Append(mapObject.Drawing);
					x += mapObject.Drawing.Length - 1;
				}
			}
			sb.AppendLine();
		}
		return sb.ToString();
	}
}

public class Robot((int, int)[] moveList, string drawing, Position location, Map map, int part) : MapObject(drawing, location, map, part)
{
	private (int, int)[] moves = moveList;
	private int moveIndex = 0;

	private Position NextPosition => Location + moves[moveIndex];

	public bool Move()
	{
		if (moveIndex == moves.Length) return false;
		if (map[NextPosition].Pushable && map[NextPosition].Push(moves[moveIndex]))
		{
			map[Location] = new Nothing(Location, ".", map, part);
			map[NextPosition] = this;
			Location = NextPosition;
			moveIndex++;
			return true;
		}
		moveIndex++;
		return false;
	}

	public bool HasInstructions => moveList.Length - 1 > moveIndex;

	public void Blocked()
	{
		moveIndex++;
	}
}

public class Box(Position location, string drawing, Map map, int part) : MapObject(drawing, location, map, part)
{
	public override bool Push((int, int) direction)
	{
		var nextLocation = Location + direction;

		if (map[nextLocation].Pushable && map[nextLocation].Push(direction))
		{
			map[Location] = new Nothing(Location, ".", map, part);
			map[nextLocation] = this;
			Location = nextLocation;
			return true;
		}
		return false;
	}

	public override bool Pushable => true;
}

public class Wall(Position location, string drawing, Map map, int part) : MapObject(drawing, location, map, part)
{

}

public class Nothing(Position location, string drawing, Map map, int part) : MapObject(drawing, location, map, part)
{
	public override bool Push((int, int) direction)
	{
		return true;
	}

	public override bool Pushable => true;
}

public class MapObject(string drawing, Position location, Map map, int part)
{
	public Position Location = location;
	public string Drawing = drawing;

	internal Map map = map;
	internal int part = part;

	public virtual bool Push((int, int) direction)
	{
		return false;
	}
	public virtual bool Pushable => false;
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